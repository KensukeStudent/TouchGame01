using System.Text.RegularExpressions;
using UnityEngine;
#pragma warning disable 649

enum State
{
    nomal,
    goal
}

/// <summary>
/// プレイヤーを制御するクラス
/// </summary>
public class PlayerContoller : MonoBehaviour, IAudio
{
    State currentState = State.nomal;
    const float moveSpeed = 10.0f;
    /// <summary>
    /// 敵を倒せるモードである
    /// </summary>
    public static bool AttackMode { private set; get; } = false;
    /// <summary>
    /// ジャンプ移動した手数
    /// </summary>
    int jumpCount = 0;

    /// <summary>
    /// 当たるべきレイヤーを指定します
    /// </summary>
    [SerializeField] LayerMask hitLayer;
    /// <summary>
    /// 壁であるかを判別します
    /// </summary>
    [SerializeField] LayerMask obstaclesLayer;
    /// <summary>
    /// イベント壁かを判別します
    /// </summary>
    [SerializeField] LayerMask evWallLayer;

    /// <summary>
    /// 現在移動中
    /// </summary>
    bool move = false;
    /// <summary>
    /// 親オブジェクト
    /// </summary>
    GameObject moveObj;
    /// <summary>
    /// ジャンプオブジェクトに当たっている
    /// </summary>
    GameObject hitJumpObj;
    /// <summary>
    /// ヒントオブジェクトに当たっている
    /// </summary>
    GameObject hintObj;

    [SerializeField] GameObject dustEffect;

    Animator anim;

    /// <summary>
    /// 特定のアクションができる時表示されます
    /// </summary>
    [SerializeField] GameObject actionEv;
    /// <summary>
    /// ブロックと触れている
    /// </summary>
    bool touch;

    /// <summary>
    /// 爆弾を所持しているときに表示します
    /// 表示中は攻撃不可
    /// </summary>
    [SerializeField] GameObject bakudan;
    /// <summary>
    /// 爆弾を投げる時に生成します(マウス方向に投げます)
    /// 壁か敵に当たったら爆破します
    /// ダメージを受けたら自分も即死
    /// </summary>
    [SerializeField] GameObject throwB;
    /// <summary>
    /// プレイヤー状態管理
    /// </summary>
    PlayerInventory pi;

    AudioSource aud;
    #region サウンド表
    //0 ----> jump
    //1 ----> eat
    //2 ----> itemGet
    //3 ----> damage
    //4 ----> gameOver
    #endregion
    [SerializeField] AudioClip[] clip;

    private void Start()
    {
        //アイテム管理クラス
        pi = new PlayerInventory();
        anim = GetComponent<Animator>();

        aud = GetComponent<AudioSource>();

        //初期位置
        moveObj = GameObject.FindGameObjectWithTag("InitPos");
        transform.SetParent(moveObj.transform);
    }

    private void Update()
    {
        if (currentState == State.goal) return;

        //マウス座標を取得しながらアクションを処理していきます
        Ray();

        //移動モードの時に処理します
        if (move) CheckMoveDistance();
    }

    #region Ray

    /// <summary>
    /// レイでマウス座標にあるものを取得します
    /// </summary>
    void Ray()
    {
        var pos = Input.mousePosition;
        pos.z = 10f;
        var mouseRay = Camera.main.ScreenPointToRay(pos);
        var hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction, Mathf.Infinity, hitLayer);

        //右クリックでバクダンをもっているならマウス方向に爆弾を投げる

        ThrowBakudan(pos);

        //ヒットしたものの状態を更新します
        Object(hit);

        //何も当たってない又は移動中なら判定しません
        if (!hit || move) return;

        //ヒット先がイベントの壁ならこちらを処理します
        //※ただし現在ジャンプオブジェクト先にマウス座標はいない
        if (!CheckObstacles(hit, evWallLayer) && touch && Input.GetMouseButtonDown(0) && SameLayer(hit,"EventWall"))
        {
            //クリックして鍵があれば、ブロックを破壊する
            pi.UseKindKey(hit.transform.gameObject);
        }

        //移動開始
        //障害物が当たるまでの距離、載っているジャンプ台でない、レイヤーがJumpPosである
        if (!CheckObstacles(hit, obstaclesLayer) && !SameJumpObj(hit.transform.gameObject) && SameLayer(hit, "JumpPos"))
        {
            //レイがジャンプオブジェクトに当たっている時にJumpObjの色を変更
            ChangeJumpObjColor(hit);

            //ジャンプ先に移動します
            ClickMoveToJumpObj(hit);
        }
    }

    /// <summary>
    /// 目的地の距離が一定になるまで移動します
    /// </summary>
    void CheckMoveDistance()
    {
        //二点間の距離を求めます
        float dis = Vector2.Distance(transform.position, moveObj.transform.position);

        //現在の地点を求めます
        float currentPos = (Time.deltaTime * moveSpeed) / dis;

        //目標地点まで移動
        transform.position = Vector2.Lerp(transform.position, moveObj.transform.position, currentPos);

        //二点間の距離が一定になったら移動を終了
        if (dis < 0.05f)
        {
            transform.SetParent(moveObj.transform);
            var q = transform.localRotation;
            q.z = 0;
            transform.localRotation = q;
            move = false;

            //Camera移動があるオブジェクトかを判別
            var camMove = moveObj.GetComponent<FloorMoveObj>();
            camMove?.CamSetVec();
        }
    }

    /// <summary>
    /// レイを出す先に障害物があるかどうかを判定します
    /// </summary>
    /// <returns></returns>
    bool CheckObstacles(RaycastHit2D hit,LayerMask layer)
    {
        var hitObj = hit.transform.gameObject;

        //物体からプレイヤーへのベクトル方向を取得
        var direction = (hitObj.transform.position - transform.position).normalized;
        //距離を求めます(長さを指定することで無作為に障害物に当たることを防ぐ)
        var dis = Vector2.Distance(hitObj.transform.position, transform.position);
        //レイに方向を入れ飛ばします
        var ray = new Ray(transform.position, direction);
        //レイ方向へ放射したものを検知
        var hitL = Physics2D.Raycast(ray.origin, ray.direction, dis, layer);
        //Debug.DrawRay(ray.origin, ray.direction, Color.green, Mathf.Infinity);
        if (hitL) return true;

        return false;
    }

    #endregion

    #region オブジェクト

    /// <summary>
    /// 今のマウスカーソルが載っているジャンプ台ではない
    /// </summary>
    /// <returns></returns>
    bool SameJumpObj(GameObject hitObj)
    {
        //比較する初期値
        var result = 0;
        if (moveObj) result = moveObj.GetInstanceID();

        return result == hitObj.GetInstanceID();
    }

    /// <summary>
    /// ジャンプオブジェクトにレイが当たっている時にジャンプオブジェクトの色を変更します
    /// </summary>
    /// <param name="hit"></param>
    void ChangeJumpObjColor(RaycastHit2D hit)
    {
        //ジャンプ先のオブジェクトの色を変更します
        //ヒット先の足場を現在の足場に代入
        hitJumpObj = hit.transform.gameObject;
        var jumpObj = hit.transform.GetComponent<JumpObj>();
        jumpObj?.NowRay();
    }

    /// <summary>
    /// クリックしたときにジャンプオブジェクトの飛び移ります
    /// </summary>
    void ClickMoveToJumpObj(RaycastHit2D hit)
    {
        if (Input.GetMouseButtonDown(0))
        {
            //ジャンプ手数を1増やします
            jumpCount++;
            //テキストを同期させます


            //SEを鳴らします
            PlaySE(0);

            //ヒットした物を入れます
            moveObj = hit.transform.gameObject;

            //parentを解除します
            transform.SetParent(null);

            //角度設定
            var q = transform.localEulerAngles;
            //-----角度を親の角度にします-----//
            q.z = GetJumpAngle(moveObj.transform);
            transform.localEulerAngles = q;

            //ジャンプエフェクトを入れます
            Instantiate(dustEffect, transform.position, transform.localRotation);
            
            //現在移動中
            move = true;

            //クリック先のJumpObjを捨てます
            RemoveJumpObj();
        }
    }

    /// <summary>
    /// ジャンプ先の角度を求めます
    /// </summary>
    float GetJumpAngle(Transform child)
    {
        //子要素
        var c = child;
        //角度
        var angle = c.transform.localEulerAngles.z;

        //親要素のすべての角度を取得   
        do
        {
            var parent = c.transform.parent;
            //一番上のfloorを管理している親(0や1)の名前であればbreakします
            //それまで角度を取得しつづけます
            if (!OnlyNum(parent.name))
            {
                var q = parent.localEulerAngles.z;
                angle += q;

                c = parent;
            }
            else break;

        } while (true);
        
        return angle;
    }

    /// <summary>
    /// 数値のみの取得
    /// </summary>
    bool OnlyNum(string s)
    {
        //数値でない文字があるかを判別それを反転させて数値のみかを取得
        return _ = !Regex.IsMatch(s, @"[^0-9]");
    }

    /// <summary>
    /// Rayにヒットした、していた物の状態を更新します
    /// </summary>
    void Object(RaycastHit2D hit)
    {
        //Jumpオブジェクト
        //ジャンプ先のオブジェクトの色を元に戻します
        if (hitJumpObj && !hit) RemoveJumpObj();

        //Hintオブジェクト
        HintObj(hit);
    }

    /// <summary>
    /// マウス座標がジャンプオブジェクトから外れたら処理します
    /// </summary>
    void RemoveJumpObj()
    {
        var jumpObj = hitJumpObj.GetComponent<JumpObj>();
        jumpObj?.RemoveNowRay();
        hitJumpObj = null;
    }

    /// <summary>
    /// ヒントオブジェクトに当たったら処理します
    /// 猫の手アニメーションを開始フラグを立てます
    /// </summary>
    /// <param name="hit"></param>
    void HintObj(RaycastHit2D hit)
    {
        //hintObjがない、ヒットしている
        if (hintObj == null && hit && SameLayer(hit, "Hint"))
        {
            hintObj = hit.transform.gameObject;
            var hint = hintObj.GetComponent<HintCat>();
            hint?.SetHint();//レイがhintLayer上を通ってしまった時は通過してしまいます
        }
        else if(hintObj && !hit)
        {
            var hU = GameObject.Find("HintCat").GetComponent<HintUI>();
            hU.ReturnFlag();
            hintObj = null;
        }
    }

    /// <summary>
    /// ヒットしたオブジェクトと比較するレイヤーを比較します
    /// </summary>
    bool SameLayer(RaycastHit2D hit,string layerName)
    {
        var hitObj = hit.transform.gameObject;
        return hitObj.layer == LayerMask.NameToLayer(layerName);
    }

    /// <summary>
    /// バクダンを持っているときに投げることができます
    /// </summary>
    void ThrowBakudan(Vector2 mousePos)
    {
        if (Input.GetMouseButtonDown(1))
        {
            var pos = Camera.main.ScreenToWorldPoint(mousePos);

            //プレイヤーからマウスまでの角度方向を取得
            //Z成分を加味しないベクトル座標を正規化して角度を求めます
            var direction = Vector3.Scale((pos - transform.position), new Vector3(1, 1, 0)).normalized;

            var go = Instantiate(throwB, transform.position, Quaternion.identity);
            var b = go.GetComponent<Bakudan>();

            //ベクトル方向を代入
            b.SetVec(direction);
        }
    }

    #endregion

    /// <summary>
    /// 効果音を鳴らします
    /// </summary>
    public void PlaySE(int clipNo, float vol = 1.0f)
    {
        aud.PlayOneShot(clip[clipNo], vol);
    }

    #region 当たり判定

    private void OnTriggerEnter2D(Collider2D col)
    {
        //敵に当たったら処理
        if (col.gameObject.CompareTag("Enemy")) HitEnemy(col);

        //敵弾に当たったら処理
        if (col.gameObject.CompareTag("EnemyShot")) HitEnemyShot();

        //アイテムを取得し解析します
        if (col.CompareTag("Item")) GetItemSwitch(col);

        //ゴールのアイテムをとったらゲームクリア
        if (col.gameObject.CompareTag("Goal")) GoalClear(col);
    }

    /// <summary>
    /// 敵に衝突
    /// </summary>
    void HitEnemy(Collider2D col)
    {
        //攻撃モードの時に敵を破壊できます
        if (AttackMode)
        {
            //攻撃アニメーション
            anim.SetTrigger("Attack");

            //当たった敵に死亡フラグを立てます
            var enemy = col.GetComponent<Enemy>();
            enemy.SetDie();

            //攻撃モード解除
            var sprite = GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
            AttackMode = false;
        }
        else Debug.Log("ダメージを受ける");
    }

    /// <summary>
    /// 敵弾に衝突
    /// </summary>
    void HitEnemyShot()
    {
        Debug.Log("ダメージを受ける");
        PlaySE(3);
    }

    /// <summary>
    /// あるアイテムに触れたら処理します
    /// </summary>
    void GetItemSwitch(Collider2D col)
    {
        //解析したアイテム名
        switch (ItemName(col.name))
        {
            //攻撃
            case "Food":
                //敵を倒せるようになります
                NowAttackMode(col);
                break;
            
            //鍵
            case "Key":
                //鍵を取得　
                //ステージ内の特定の場所へいけるようになります
                HaveKey(col);
                break;
            
            //道具
            case "Bakudan":
                //爆弾に触れた時に処理します
                Bakudan(col);
                break;
        }
    }

    /// <summary>
    /// 名前を解析します
    /// </summary>
    /// <param name="name">アイテム名</param>
    string ItemName(string name)
    {
        return _ = Regex.Match(name, @"(.+)_").Groups[1].Value;
    }

    /// <summary>
    /// 現在アタックモード
    /// </summary>
    void NowAttackMode(Collider2D col)
    {
        //攻撃モードになります
        AttackMode = true;
        //取得したアイテムを削除します
        Destroy(col.gameObject);
        //攻撃モードの色に変えます
        var sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 0.6f, 0, 1);
        //SEを鳴らします
        PlaySE(1);
    }

    /// <summary>
    /// 鍵を取得
    /// </summary>
    void HaveKey(Collider2D col)
    {
        //鍵の種類に応じてカウントを所字数を上げます
        pi.GetKindKey(col.gameObject.name);

        //サウンドを鳴らします
        PlaySE(2);

        //取得した鍵を削除します
        Destroy(col.gameObject);
    }

    /// <summary>
    /// ゴール
    /// </summary>
    void GoalClear(Collider2D col)
    {
        anim.SetBool("Goal", true);

        //もし攻撃モードだった場合のため色を変更処理します
        var sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.white;
        
        //ステートを変更します
        currentState = State.goal;

        //取得したゴールオブジェクトを削除します
        Destroy(col.gameObject);

        //ステージの状態を更新します
        var sm = GameObject.Find("StageManager").GetComponent<StageManager>();
        //ジャンプした数を評価します(----> 各ステージのスコアに反映されます)
        sm.StageUpdate(GameManager.Instance.StageNo, jumpCount);

        //sceneステートを変更します
        ScreenTransition.Instance.ChangeState(SceneState.gameOverMode);

        //Goalアニメーションが終わったら遷移を開始します
        const float goalLag = 1.2f;//---> 1.2f 秒後に遷移開始
        ScreenTransition.Instance.TimeST(goalLag);

        //ゲーム開始フラグを切ります
        GameManager.Instance.SetGame(false);
    }

    /// <summary>
    /// 爆弾がイベントものかどうかで処理を分けます
    /// </summary>
    /// <param name="name">爆弾名</param>
    void Bakudan(Collider2D col)
    {
        //爆弾の状態
        //nomalかev
        var condition = Regex.Match(col.name, @"_(.+)").Groups[1].Value;

        switch (condition)
        {
            //通常の爆弾
            case "Nomal":
                //bakudanを表示
                //投げること可能
                TakeBakudan(col.gameObject);
                break;

            //イベント式の爆弾
            case "Ev":
                //イベント開始
                BakudanEvent(col);
                break;
        }
    }

    /// <summary>
    /// 爆弾を取得したときの処理
    /// </summary>
    void TakeBakudan(GameObject b)
    {
        //bakudanをプレイヤーの頭の上に表示します
        bakudan.SetActive(true);
        //爆弾を削除します
        Destroy(b);
    }

    /// <summary>
    /// 爆弾イベントの時一回限り呼ばれます
    /// </summary>
    void BakudanEvent(Collider2D col)
    {
        #region 処理流れ

        //黒いeventnの爆弾を取る

        //イベントパート
        //fontの変更
        //----> 「なんにゃー、このくだもの？？」(ネコのセリフ)

        //テキストの方でWaitを書けます(animationの時間分)
        // 爆弾の色を徐々に元の色に戻していきます

        //----> 「にゃ！にゃ！？　ばくだんにゃーーー！！！」

        //解説
        //---> fontを変更
        //----> 「ジャンプ位置でない所で右クリックすることで投げることができます」
        //----> 「敵を倒すこともできるので快感を味わってみてください。
        //        ただし、自分もまき壊れることもあるのでお気をつけて。」
        //----> 「爆弾は何度も生成されるので、何回でも使うことができます」

        #endregion

        //タイムスケールで時間を止めます
        Time.timeScale = 0;

        //表示用テキストCanvas
        var cavas = GameObject.Find("DescriptionCanvas");
        //親から表示用テキストを取得
        var tm = cavas.transform.Find("NovelFrame").GetComponent<TextManager>();
        //Ev爆弾のノベルパートを取得
        var b = col.GetComponent<Bakudan>();

        //読み込むテキストを表示用UIの方に格納します
        tm.SetEvText(b.EvBakudan());

        //NovelFrameを徐々に表示します
        tm.GetImage();

        tm.SetAction(b.Actions());
    }

    /// <summary>
    /// イベント用で爆弾を表示します
    /// </summary>
    public void ActiveBakudan()
    {
        bakudan.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        //特定の鍵を持っている場合、それに応じたブロックを破壊できます
        if (col.gameObject.CompareTag("Block"))
        {
            if (!touch)
            {
                touch = true;
                actionEv.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        //イベントブロックと離れた
        if (col.gameObject.CompareTag("Block"))
        {
            touch = false;
            actionEv.SetActive(false);

            var bS = col.GetComponent<BlocksScript>();
            bS.InActive();
        }
    }

    #endregion
}

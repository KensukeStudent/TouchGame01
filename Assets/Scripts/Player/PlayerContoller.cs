using System;
using System.Collections.Generic;
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
public class PlayerContoller : MonoBehaviour
{
    State currentState = State.nomal;
    const float moveSpeed = 10.0f;

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
    GameObject hintObj;
    [SerializeField] GameObject dustEffect;

    public static bool AttackMode { private set; get; } = false;

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
    /// プレイヤー状態管理
    /// </summary>
    PlayerInventory pi;

    private void Start()
    {
        pi = new PlayerInventory();
        anim = GetComponent<Animator>();
        moveObj = GameObject.FindGameObjectWithTag("InitPos");
        transform.SetParent(moveObj.transform);
    }

    private void Update()
    {
        if (currentState == State.goal) return;

        Ray();
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

        //ヒットしたものの状態を更新します
        Object(hit);

        //何も当たってない又は移動中なら判定しません
        if (!hit || move) return;

        //ヒット先がイベントの壁ならこちらを処理します
        //※ただし現在ジャンプオブジェクト先にマウス座標はいない
        if (!CheckObstacles(hit, evWallLayer) && touch && Input.GetMouseButtonDown(0) && !hitJumpObj)
        {
            //クリックして鍵があれば、ブロックを破壊する
            pi.UseKindKey(hit.transform.name, hit.transform.gameObject);
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
            //ヒットした物を入れます
            moveObj = hit.transform.gameObject;

            transform.SetParent(null);

            //角度設定
            var q = transform.localEulerAngles;
            //-----角度を親の角度にします-----//
            q.z = GetJumpAngle(moveObj.transform);
            transform.localEulerAngles = q;

            //ジャンプエフェクトを入れます
            Instantiate(dustEffect, transform.position, transform.localRotation);
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

    #endregion

    #region 当たり判定

    private void OnTriggerEnter2D(Collider2D col)
    {
        //敵に当たったら処理
        if (col.gameObject.CompareTag("Enemy")) HitEnemy(col);

        //敵弾に当たったら処理
        if (col.gameObject.CompareTag("EnemyShot")) HitEnemyShot();

        //敵を倒せるようになります
        if (col.gameObject.CompareTag("ItemAttack")) NowAttackMode(col);

        //鍵を取得　
        //ステージ内の特定の場所へいけるようになります
        if (col.gameObject.CompareTag("Key")) HaveKey(col);

        //ゴールのアイテムをとったらゲームクリア
        if (col.gameObject.CompareTag("Goal")) GoalClear(col);
    }

    /// <summary>
    /// 敵に衝突
    /// </summary>
    void HitEnemy(Collider2D col)
    {
        if (AttackMode)
        {
            anim.SetTrigger("Attack");

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
    }

    /// <summary>
    /// 現在アタックモード
    /// </summary>
    void NowAttackMode(Collider2D col)
    {
        AttackMode = true;
        Destroy(col.gameObject);
        var sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 0.6f, 0, 1);
    }

    /// <summary>
    /// 鍵を取得
    /// </summary>
    void HaveKey(Collider2D col)
    {
        //鍵の種類に応じてカウントを所字数を上げます
        pi.GetKindKey(col.gameObject.name);
        //サウンドを鳴らします

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

        Destroy(col.gameObject);

        //sceneステートを変更します
        ScreenTransition.Instance.ChangeState(SceneState.gameOverMode);

        //Goalアニメーションが終わったら遷移を開始します
        const float goalLag = 1.2f;
        ScreenTransition.Instance.TimeST(goalLag);
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

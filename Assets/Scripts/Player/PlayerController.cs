using System.Text.RegularExpressions;
using UnityEngine;
#pragma warning disable 649

public  enum State
{
    nomal,
    goal,
    die
}

/// <summary>
/// プレイヤーを制御するクラス
/// </summary>
public partial class PlayerController : MonoBehaviour, IAudio
{
    public static State currentState = State.nomal;

    /// <summary>
    /// 移動速度
    /// </summary>
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
    //4 ----> hpCure
    #endregion
    [SerializeField] AudioClip[] clip;

    private void Start()
    {
        //ステートを初期化
        currentState = State.nomal;

        //アイテム管理クラス
        pi = new PlayerInventory();

        //アニメーション
        anim = GetComponent<Animator>();

        //サウンド
        aud = GetComponent<AudioSource>();

        //初期位置
        moveObj = GameObject.FindGameObjectWithTag("InitPos");
        //初期親を指定
        transform.SetParent(moveObj.transform);
    }

    private void Update()
    {
        if (currentState == State.goal || currentState == State.die) return;

        //マウス座標を取得しながらアクションを処理していきます
        Ray();

        //移動モードの時に処理します
        if (move) CheckMoveDistance();
    }

    //------レイやサウンド、死亡フラグについて記述------//

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

    /// <summary>
    /// 効果音を鳴らします
    /// </summary>
    public void PlaySE(int clipNo, float vol = 1.0f)
    {
        aud.PlayOneShot(clip[clipNo], vol);
    }

    /// <summary>
    /// プレイヤーが倒れるアニメーション
    /// </summary>
    public void DiePlayer()
    {
        anim.SetBool("Die", true);
        //当たり判定の
        GetComponent<BoxCollider2D>().enabled = false;
        //倒れるステートに変更
        currentState = State.die;

        //アニメーション終了後シーン遷移
        var sm = GameObject.Find("StageManager").GetComponent<StageManager>();
        //シーンを遷移させます
        //Dieアニメーション時間分のタイムラグを与えてから遷移
        sm.GameEnd(anim, "Die");
    }
}

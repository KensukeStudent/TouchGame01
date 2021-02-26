 using UnityEngine;
#pragma warning disable 649

/// <summary>
/// 敵クラス(どくろ)攻撃:近づく
/// </summary>
public class DokuroMove : Enemy
{
    public float MoveSpeed { set; get; } = 1.0f;

    GameObject player;
    /// <summary>
    /// プレイヤーを検知する距離
    /// </summary>
    const float chaseRadius = 3.5f;
    /// <summary>
    /// ゲームが始まった時の位置記録
    /// </summary>
    Vector2 initPos;
    /// <summary>
    /// 初期位置からreturnRadius分離れたら初期位置に戻ります
    /// </summary>
    const float returnRadius = 3f;
    /// <summary>
    /// 初期位置を一定距離離れた時に生成するエフェクト
    /// </summary>
    [SerializeField] GameObject returnEffect;
    [SerializeField] GameObject instantEffect;

    Animator anim;
    Rigidbody2D rb2D;
    /// <summary>
    /// 障害物オブジェクト
    /// </summary>
    [SerializeField] LayerMask obstaclesLayer;

 
    protected override void Start()
    {
        base.Start();

        DefeatThisEnemy(true);
        player = GameObject.FindWithTag("Player");
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        initPos = transform.position;
    }

 
    void Update()
    {
        //プレイヤーの座標を取得
        var playerPos = player.transform.position;
        //プレイヤーとこの敵との距離を取得
        Distance(playerPos);
    }

    /// <summary>
    /// プレイヤ－の距離を取得
    /// </summary>
    void Distance(Vector2 playerPos)
    {
        //判定->プレイヤーとの距離がcahseRadius以下でレイの先に障害物が無い
        if (player && Vector2.Distance(playerPos, transform.position) <= chaseRadius &&
            CheckObstacles(player))
        {
            Vector2 pos = transform.position;

            //プレイヤーから逃げる
            if (PlayerContoller.AttackMode)
            {
                //向きを取得
                ChnageAnim(initPos - pos);
                RunMove();
                anim.SetBool("Mark", true);
            }
            //プレイヤーを追う
            else
            {
                //向きを取得
                ChnageAnim(playerPos - pos);
                Move(playerPos);
                anim.SetBool("Mark", false);
            }
            anim.SetBool("Move", true);
        }
        //その位置で止まる
        else if (Vector2.Distance(playerPos, transform.position) >= chaseRadius)
        {
            anim.SetBool("Move", false);
            anim.SetBool("Mark", false);
            RetrunPosition();
        }
    }

    /// <summary>
    /// レイを出す先に障害物があるかどうかを判定します
    /// </summary>
    /// <returns></returns>
    bool CheckObstacles(GameObject hitObj)
    {
        //物体からプレイヤーへのベクトル方向を取得
        var direction = (hitObj.transform.position - transform.position).normalized;
        //距離を求めます(長さを指定することで無作為に障害物に当たることを防ぐ)
        var dis = Vector2.Distance(hitObj.transform.position, transform.position);
        //レイに方向を入れます
        var ray = new Ray(transform.position, direction);
        //レイ方向へ放射したものを検知
        var hit = Physics2D.Raycast(ray.origin, ray.direction, dis, obstaclesLayer);
        //Debug.DrawRay(ray.origin, ray.direction, Color.green, Mathf.Infinity);
        if (hit) return false;

        return true;
    }

    /// <summary>
    /// 初期位置から一定距離離れた場合元の位置に戻ります
    /// </summary>
    void RetrunPosition()
    {
        if (Vector2.Distance(initPos, transform.position) >= returnRadius)
        {
            //その位置に🔥を配置
            var effectF = Instantiate(returnEffect, transform.position, Quaternion.identity);
            var effectR = effectF.GetComponent<DokuroReturnEffect>();
            //初期位置に炎陣を非アクティブで生成
            var effectI = Instantiate(instantEffect, initPos, Quaternion.identity);
            //アクティブ状態にするオブジェクトを入れます
            effectR.dokuroInstantEffect = effectI;
            var dokuroActive = effectI.GetComponent<DokuroInstantEffect>();
            dokuroActive.ThisDokuro = gameObject;
            //初期位置に戻します
            transform.position = initPos;
            //非アクティブにします
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// プレイヤー方向に移動
    /// </summary>
    void Move(Vector2 playerVec)
    {
        var movePos = Vector2.MoveTowards(transform.position, playerVec, MoveSpeed * Time.deltaTime);
        rb2D.MovePosition(movePos);
    }

    /// <summary>
    /// 初期位置方向に移動
    /// </summary>
    void RunMove()
    {
        var movePos = Vector2.MoveTowards(transform.position, initPos, MoveSpeed * Time.deltaTime);
        rb2D.MovePosition(movePos);
    }

    /// <summary>
    /// アニメーション
    /// </summary>
    void SetAnimFloat(Vector2 setVector)
    {
        anim.SetFloat("MoveX", setVector.x);
        anim.SetFloat("MoveY", setVector.y);
    }

    /// <summary>
    /// 自身とプレイヤーの向きに合わせてアニメーションを変更します
    /// </summary>
    void ChnageAnim(Vector2 direction)
    {
        //xのほうが大きいとき(左右)
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0) SetAnimFloat(Vector2.right);
            else if (direction.x < 0) SetAnimFloat(Vector2.left);
        }
        //yのほうが大きいとき(上下)
        else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if (direction.y > 0) SetAnimFloat(Vector2.up);
            else if (direction.y < 0) SetAnimFloat(Vector2.down);
        }
    }
}

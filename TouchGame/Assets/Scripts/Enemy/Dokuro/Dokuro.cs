using UnityEngine;
using System.Text.RegularExpressions;

#pragma warning disable 649

public enum Mode
{
    shotMode,
    moveMode
}

/// <summary>
/// 敵クラス(どくろ)
/// 攻撃:・弾を出す ・近づく
/// </summary>
public class Dokuro : Enemy
{
    public Mode CurrentMode { set; get; }

    //---------------------ShotMode--------------------//
    [SerializeField] GameObject shotObj;
    public float ShotSpeed { private set; get; } = 5.0f;
    /// <summary>
    /// 弾を出す位置
    /// </summary>
    readonly float[] shotAngle = { 0, 90, 180, 270 };
    /// <summary>
    /// フラグに応じてどの角度から生成するかを決めます
    /// </summary>
    public bool[] Flag { private set; get; } = { true, true, true, true }; 
    
    /// <summary>
    /// 弾の発射時間
    /// </summary>
    public float ShotTime { private set; get; } = 5f;
    float timer = 0;
    /// <summary>
    /// 弾のインターバル時間
    /// </summary>
    bool intervalMode = false;
    // 画像のサイズ
    float sizeX;
    float sizeY;

    //---------------------MoveMode--------------------//
    public float MoveSpeed { set; get; } = 1.0f;

    GameObject player;
    /// <summary>
    /// プレイヤーを検知する距離
    /// </summary>
    const float chaseRadius = 3.5f;
    /// <summary>
    /// ゲームが始まった時の位置
    /// </summary>
    Vector2 initPos;
    const float returnRadius = 3f;
    /// <summary>
    /// 初期位置を一定距離離れた時に生成するエフェクト
    /// </summary>
    [SerializeField] GameObject returnEffect;
    [SerializeField] GameObject instantEffect;
    
    Animator anim;
    Rigidbody2D rb2D;
    [SerializeField] LayerMask obstaclesLayer;

    private void Start()
    {
        DefeatThisEnemy(true);

        anim = GetComponent<Animator>();

        switch (CurrentMode)
        {
            case Mode.shotMode:
                var sr = GetComponent<SpriteRenderer>();
                sizeX = sr.size.x;
                sizeY = sr.size.y;
                break;

            case Mode.moveMode:
                player = GameObject.FindWithTag("Player");
                rb2D = GetComponent<Rigidbody2D>();
                initPos = transform.position;
                break;
        }
    }

    private void Update()
    {
        switch (CurrentMode)
        {
            case Mode.shotMode:
                ShotInstant();
                break;

            case Mode.moveMode:
                var playerPos = player.transform.position;
                Distance(playerPos);
                break;
        }
    }

    #region Shot

    /// <summary>
    /// 弾の生成
    /// </summary>
    void ShotInstant()
    {
        timer += Time.deltaTime;
        if (intervalMode)
        {
            //弾を生成してから3秒間のインターバルを設ける
            if (timer > ShotTime * 1.6f)
            {
                anim.SetBool("Shot", false);
                intervalMode = false;
                timer = 0;
            }
        }
        else
        {
            if (timer > ShotTime)
            {
                ShotFlag();
                anim.SetBool("Shot", true);
                intervalMode = true;
            }
        }
    }

    /// <summary>
    /// 弾を出す位置のフラグの確認
    /// </summary>
    void ShotFlag()
    {
        for (int i = 0; i < shotAngle.Length; i++)
            if (Flag[i])
            {
                var shot = Instantiate(shotObj, InstantPos(shotAngle[i]), Quaternion.Euler(0, 0, shotAngle[i]));
                //指定した速度を代入
                var dShot = shot.GetComponent<DShot>();
                dShot.ShotSpeed = ShotSpeed;
            }
    }

    /// <summary>
    /// 初期値でステータスを入れます
    /// </summary>
    /// <param name="s"></param>
    /// <param name="count"></param>
    public void SetShotStates(DokuroFloor d,D_Shot s,int count, string name)
    {
        //速度,発射位置,発射時間,名前
        ShotSpeed = s.shotSpeed[count];
        Flag = EnemyMan.SetDirect(s, count);
        ShotTime = s.shotTime[count];
        //名前+state名
        this.name = name + d.state[count];

        if (!d.state[count].Contains("ev")) return;

        var evName = Regex.Match(d.state[count], @"(.+)\d+").Groups[1].Value;

        switch (evName)
        {
            case "ev":
            case "evP":
                //セットする子の敵を入れます
                if (evName == "evP") SetChildEnemy();
                SetEnemyKind(EnemyKind.ev);
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player") && Die)
        {
            //親がいる場合処理
            EventParent();
            //削除処理
            Explosion();
        }
    }

    /// <summary>
    /// 親を持っている場合親の弾発射時間増やします
    /// </summary>
    protected override void EventParent()
    {
        if (!MyP) return;
        HaveParent();
        var d = MyP.GetComponent<Dokuro>();
        d.ShotTime++;
    }

    /// <summary>
    /// 生成する位置をフラグに応じて変更します
    /// </summary>
    Vector2 InstantPos(float angle)
    {
        var offset = transform.position;
        switch (angle)
        {
            //右
            case 0:
                offset.x = transform.position.x + sizeX / 2;
                //Debug.Log(offset.x);
                break;
            //上
            case 90:
                offset.y = transform.position.y + sizeY / 2;
                break;
            //左
            case 180:
                offset.x = transform.position.x - sizeX / 2;
                break;
            //下
            case 270:
                offset.y = transform.position.y - sizeY / 2;
                break;
        }
        return offset;
    }

    #endregion

    #region Move
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
        else if(Vector2.Distance(playerPos, transform.position) >= chaseRadius)
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
        if(Vector2.Distance(initPos,transform.position) >= returnRadius)
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
    #endregion
}

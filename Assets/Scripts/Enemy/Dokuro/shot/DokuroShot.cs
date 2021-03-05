using UnityEngine;
using System.Text.RegularExpressions;
#pragma warning disable 649

/// <summary>
/// 敵クラス(どくろ)攻撃:弾を出す
/// </summary>
public class DokuroShot : Enemy
{
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

    Animator anim;

    protected override void Start()
    {
        base.Start();

        DefeatThisEnemy(true);

        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        ShotInstant();
    }

    /// <summary>
    /// 弾の生成
    /// </summary>
    void ShotInstant()
    {
        timer += Time.deltaTime;
        //インターバルモード
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
                //指定方向に弾をだします
                ShotFlag();
                anim.SetBool("Shot", true);
                //発射後インターバルを設けます
                intervalMode = true;
                //SEを鳴らします
                PlaySE(1, 0.4f);
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
                var dShot = shot.GetComponent<EnemyShot>();
                dShot.SetSpeed(ShotSpeed);
            }
    }

    /// <summary>
    /// 初期値でステータスを入れます
    /// </summary>
    /// <param name="count">stateが入っている配列</param>
    public void SetShotStates(DokuroFloor d, D_Shot s, int count, string name)
    {
        //速度,発射位置,発射時間,名前
        ShotSpeed = s.shotSpeed[count];
        Flag = EnemyMan.SetDirect(s, count);
        ShotTime = s.shotTime[count];
        //名前+state名
        this.name = name + d.state[count];

        //この敵がev(イベント)であれば処理ます
        if (!d.state[count].Contains("ev")) return;

        //イベント番号を取得
        var evName = Regex.Match(d.state[count], @"(.+)\d+").Groups[1].Value;

        switch (evName)
        {
            case "ev":
            case "evP":
                //セットする子の敵を入れます
                if (evName == "evP") SetChildEnemy();

                //ステートevに変更します
                SetEnemyKind(EnemyKind.ev);
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player") && Die)
        {
            //親がいる場合処理
            //EventParent();
            //削除処理
            Explosion();
        }
    }

    /// <summary>
    /// 親を持っている場合、親の弾発射時間増やします
    /// </summary>
    protected override void EventParent()
    {
        if (!MyP) return;
        HaveParent();
        var d = MyP.GetComponent<DokuroShot>();
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
}

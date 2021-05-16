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
    public bool[] DirectFlag { private set; get; } = { true, true, true, true };

    /// <summary>
    /// 弾の発射時間
    /// </summary>
    public float ShotTime { private set; get; } = 5f;
    float timer = 0;
    /// <summary>
    /// 弾のインターバル時間
    /// </summary>
    bool intervalMode = false;

    /// <summary>
    /// 画像サイズ   
    /// </summary>
    Vector2 size;

    Animator anim;

    protected override void Start()
    {
        base.Start();
        //倒せる敵
        DefeatThisEnemy(true);

        anim = GetComponent<Animator>();
        //画像サイズ取得
        size = GetComponent<SpriteRenderer>().size;
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
        if (intervalMode && timer > ShotTime * 1.6f)
        {
            //弾を生成してから3秒間のインターバルを設ける
            anim.SetBool("Shot", false);
            intervalMode = false;
            timer = 0;
        }
        else if(!intervalMode && timer > ShotTime)
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

    /// <summary>
    /// 弾を出す位置のフラグの確認
    /// </summary>
    void ShotFlag()
    {
        for (int i = 0; i < shotAngle.Length; i++)
        {
            //発射フラグの方向
            if (DirectFlag[i])
            {
                var shot = Instantiate(shotObj, InstantPos(shotAngle[i]), Quaternion.Euler(0, 0, shotAngle[i]));
                //指定した速度を代入
                var dShot = shot.GetComponent<EnemyShot>();
                dShot.SetSpeed(ShotSpeed);
            }
        }
    }

    /// <summary>
    /// 初期値でステータスを入れます
    /// </summary>
    /// <param name="count">stateが入っている配列</param>
    public void SetShotStates(DokuroFloor d, D_Shot s, int count, string name)
    {
        //速度
        ShotSpeed = s.shotSpeed[count];
        //発射方向フラグ
        DirectFlag = EnemyMan.SetDirect(s, count);
        //発射時間
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

    /// <summary>
    /// 親を持っている場合、親の弾発射時間増やします
    /// </summary>
    protected override void EventParent()
    {
        //親イベントがある場合のみ処理します
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
                offset.x = transform.position.x + size.x / 2;
                Debug.Log(offset.x);
                break;
            //上
            case 90:
                offset.y = transform.position.y + size.y / 2;
                break;
            //左
            case 180:
                offset.x = transform.position.x - size.x / 2;
                break;
            //下
            case 270:
                offset.y = transform.position.y - size.y / 2;
                break;
        }
        return offset;
    }
}

using System.Collections;
using UnityEngine;

/// <summary>
/// 大砲スクリプト
/// </summary>
public class Cannon : MonoBehaviour,IAudio
{
    [SerializeField] GameObject shot;

    /// <summary>
    /// 大砲を出す時間
    /// </summary>
    float shotTimer = 0;
    /// <summary>
    /// 計測時間
    /// </summary>
    float time = 0;

    /// <summary>
    /// 弾を出した後の待ちを作ります
    /// </summary>
    bool shotWait = false;

    /// <summary>
    /// 一回に弾を出す回数
    /// </summary>
    [Range(1, 3)]
    int count = 1;
    float[] interval;

    /// <summary>
    /// 大砲の速度
    /// </summary>
    float shotSpeed = 0;

    float instantY = 0;

    AudioSource aud;
    [SerializeField] AudioClip clip;

    private void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //発射時間を計測し、
        //時間になったら弾を間隔で生成します
        TakeTimer();
    }

    void Debg()
    {
        var shotSpeed = 5;
        var shotTimer = 1.0f;
        var count = 5;
        var interval = new float[5]
        {
            0.5f,
            0.1f,
            0.5f,
            0.1f,
            0.5f
        };

        Init(false, shotSpeed, shotTimer, count, interval);
    }

    /// <summary>
    /// 初期値を入れます
    /// </summary>
    /// <param name="direction">Y軸方向の向き</param>
    /// <param name="speed">弾の速度</param>
    /// <param name="shotTime">弾の発射時間</param>
    /// <param name="shotCount">一回に出す弾の数</param>
    /// <param name="shotCountInterval">一回に出す弾の間隔</param>
    public void Init(bool direction, float speed,float shotTime,int shotCount,float[] shotCountInterval)
    {
        //画像コンポーネント取得
        var sr = GetComponent<SpriteRenderer>();

        //上向きか下向きか
        sr.flipY = direction;

        var srC = shot.GetComponent<SpriteRenderer>();

        //生成位置を計算します
        instantY = sr.size.y / 2 + srC.size.y / 1.5f;

        //向きに応じて速度を変更
        //flipYがtrueなら下向きに速度を正します
        if (direction)
        {
            speed = -speed;
            instantY = -instantY;
        }

        shotSpeed = speed;

        //発射時間
        shotTimer = shotTime;

        //一回に出す弾の数
        count = shotCount;

        //一回に出す弾の間隔
        interval = shotCountInterval;
    }

    /// <summary>
    /// 弾を出すフラグを計測します
    /// </summary>
    void TakeTimer()
    {
        //shotWaitがtrueの時には計測しません
        if (shotWait) return;

        time += Time.deltaTime;

        //発射時間になったら弾を生成します
        if(time > shotTimer)
        {
            //計測を停止します
            shotWait = true;

            //計測時間の初期化
            time = 0;

            //間隔を置いて弾を生成開始します
            StartCoroutine(ShotInterval());
        }
    }

    /// <summary>
    /// 間隔を置いて弾を生成します
    /// </summary>
    /// <returns></returns>
    IEnumerator ShotInterval()
    {
        for (int i = 0; i < count; i++)
        {
            //一つ一つの時間が異なります
            yield return new WaitForSeconds(interval[i]);

            var pos = transform.position;
            pos.y += instantY;

            //弾の生成
            var goS = Instantiate(shot, pos, Quaternion.identity);
            //弾の速度を入れます
            goS.GetComponent<CannonShot>().SetSpeed(shotSpeed);

            //効果音を鳴らします
            PlaySE(0, 0.2f);
        }

        shotWait = false;
    }

    /// <summary>
    /// 効果音を鳴らします
    /// </summary>
    public void PlaySE(int clipNo, float vol = 1.0f)
    {
        aud.PlayOneShot(clip, vol);
    }
}

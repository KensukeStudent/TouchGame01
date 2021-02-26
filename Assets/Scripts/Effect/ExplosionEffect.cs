using UnityEngine;

/// <summary>
/// エフェクト後の処理クラス
/// </summary>
public class ExplosionEffect : MonoBehaviour
{
    protected float DesTime { set; get; } = 0;

    Animation animt;
    AudioSource aud;

    /// <summary>
    /// エフェクトのSEを鳴らします
    /// </summary
    public void PlaySE(AudioClip clip,float vol = 1.0f)
    {
        aud = GetComponent<AudioSource>();
        aud.PlayOneShot(clip, vol);

        //破壊時間を指定
        var desTime = DestoryEffectTime(1.0f, clip.length);
        Destroy(gameObject, desTime);
    }

    /// <summary>
    /// 破棄する時間を取得
    /// </summary>
    public float DestoryEffectTime(float addTime = 1.0f , float effectTime = 0.0f)
    {
        animt = GetComponent<Animation>();
        return _ = animt.clip.length * addTime + effectTime;
    }
}

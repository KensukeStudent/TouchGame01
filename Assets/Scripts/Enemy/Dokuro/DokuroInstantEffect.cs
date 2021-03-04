using UnityEngine;

/// <summary>
/// エフェクト後、どくろを初期位置にセットしなすクラス
/// </summary>
public class DokuroInstantEffect : ExplosionEffect
{
    float timer = 0;
    public GameObject ThisDokuro { set; get; }

    void Start()
    {
        const float addTime = 1;
        //削除時間を指定
        DesTime = DestoryEffectTime(addTime);
    }

    void Update()
    {
        SetDokuro();
    }

    /// <summary>
    /// 指定時間後初期位置にどくろを表示しこのエフェクトを削除します
    /// </summary>
    void SetDokuro()
    {
        timer += Time.deltaTime;
        if (timer > DesTime)
        {
            ThisDokuro.SetActive(true);
            Destroy(gameObject);
        }
    }
}

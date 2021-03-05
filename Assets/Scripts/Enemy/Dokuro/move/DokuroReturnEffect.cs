using UnityEngine;

/// <summary>
/// どくろ(移動)初期位置に戻る時に出すエフェクト
/// </summary>
public class DokuroReturnEffect : ExplosionEffect
{
    public GameObject dokuroInstantEffect { set; get; }
    float timer = 0;

    void Start()
    {
        const float addTime = 2;
        //削除時間を指定
        DesTime = DestoryEffectTime(addTime);
    }

    void Update()
    {
        ActiveEffect();
    }

    /// <summary>
    /// どくろ召喚エフェクトをアクティブにする
    /// </summary>
    void ActiveEffect()
    {
        timer += Time.deltaTime;
        if (timer > DesTime)
        {
            dokuroInstantEffect.SetActive(true);
            Destroy(gameObject);
        }
    }
}

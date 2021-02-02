using UnityEngine;

/// <summary>
/// エフェクト後、どくろを初期位置にセットしなすクラス
/// </summary>
public class DokuroInstantEffect : ExplosionEffect
{
    float timer = 0;
    public GameObject ThisDokuro { set; get; }

    override protected void Start()
    {
        DestoryEffectTime(addTime);
    }

    override protected void Update()
    {
        SetDokuro();
    }

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

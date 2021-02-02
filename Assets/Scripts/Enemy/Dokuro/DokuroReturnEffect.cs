using UnityEngine;

public class DokuroReturnEffect : ExplosionEffect
{
    public GameObject dokuroInstantEffect { set; get; }
    float timer = 0;

    override protected void Start()
    {
        addTime = 2;
        DestoryEffectTime(addTime);
    }

    override protected void Update()
    {
        ActiveEffect();
    }

    /// <summary>
    /// エフェクトをアクティブにする
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

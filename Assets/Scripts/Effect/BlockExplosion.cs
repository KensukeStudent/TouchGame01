using UnityEngine;

/// <summary>
///爆破ブロック
/// </summary>
public class BlockExplosion : BlocksScript
{
    /// <summary>
    /// 爆破エフェクト
    /// </summary>
    [SerializeField] GameObject exEffect;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// エフェクトを表示し削除します
    /// </summary>
    public override void Destroy()
    {
        //エフェクトを表示
        var go = Instantiate(exEffect, transform.position, Quaternion.identity);
        var effect = go.GetComponent<ExplosionEffect>();
        //アニメーション再生後削除します
        Destroy(go.gameObject, effect.DestoryEffectTime());

        base.Destroy();
    }
}

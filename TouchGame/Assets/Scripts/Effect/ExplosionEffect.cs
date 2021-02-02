using UnityEngine;

/// <summary>
/// エフェクト後の処理クラス
/// </summary>
public class ExplosionEffect : MonoBehaviour
{
    /// <summary>
    /// 破壊時間
    /// </summary>
    protected float DesTime;
    Animation animt;
    /// <summary>
    /// エフェクトを破棄する合計時間に加算する時間(基本は1秒)
    /// </summary>
    protected float addTime = 1;

    virtual protected void Start()
    {
        DestoryEffectTime(addTime);
        DestoryObj(DesTime);
    }

    virtual protected void Update()
    {
        
    }

    /// <summary>
    /// 破棄する時間を取得
    /// </summary>
    protected void DestoryEffectTime(float addTime)
    {
        animt = GetComponent<Animation>();
        DesTime = animt.clip.length * addTime;
    }

    /// <summary>
    /// 時間でオブジェクトを破壊します
    /// </summary>
    protected void DestoryObj(float desTime)
    {
        Destroy(gameObject, desTime);
    }
}

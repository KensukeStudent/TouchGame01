[System.Serializable]
public class CannonJson : ID
{
    public CannonFloor[] floor;
}

/// <summary>
/// 大砲
/// </summary>
[System.Serializable]
public class CannonFloor
{
    /// <summary>
    /// 上向きか下向き
    /// trueなら下向き
    /// </summary>
    public bool[] direction;
    /// <summary>
    /// 弾の速度
    /// </summary>
    public float[] speed;
    /// <summary>
    /// 弾の発射時間
    /// </summary>
    public float[] shotTime;
    /// <summary>
    /// 一回に出す弾の数
    /// </summary>
    public int[] onceCount;
    /// <summary>
    /// 一回に出す弾の間隔
    /// </summary>
    public float[][] onceCountInterval;
}
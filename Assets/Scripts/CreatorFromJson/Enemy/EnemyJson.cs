[System.Serializable]
//どくろ(敵)
public class DokuroEnemy
{
    /// <summary>
    /// ステージId
    /// </summary>
    public string id;
    public DokuroFloor[] floor;
}

[System.Serializable]
public class DokuroFloor
{
    /// <summary>
    /// ステイトの設定
    /// </summary>
    public string[] state;
    /// <summary>
    /// どくろ(弾)
    /// </summary>
    public D_Shot[] shot;
    /// <summary>
    /// どくろ(追う)
    /// </summary>
    public D_Move[] move;
}

[System.Serializable]
public class D_Shot
{
    public string name;
    /// <summary>
    /// 弾の速度
    /// </summary>
    public float[] shotSpeed;
    /// <summary>
    /// 弾の方向
    /// R->右,U->上,L->左,D->下
    /// </summary>
    public string[] shotPos;
    /// <summary>
    /// 弾のタイムラグ
    /// </summary>
    public float[] shotTime;
}
[System.Serializable]
public class D_Move
{
    public string name;
    /// <summary>
    /// 移動速度
    /// </summary>
    public float[] moveSpeed;
}
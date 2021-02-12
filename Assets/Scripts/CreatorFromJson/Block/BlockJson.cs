[System.Serializable]
//ブロックの説明
public class Block:ID
{
    public BlockFloor[] floor;
}

/// <summary>
/// フロア内のブロック
/// </summary>
[System.Serializable]
public class BlockFloor
{
    public string[] name;
    /// <summary>
    /// 次の面に移動するフラグを持つブロック
    /// MeatObjとCatArrowを表示させる
    /// </summary>
    public string[] jumpFloor;
    /// <summary>
    /// イベントブロック
    /// </summary>
    public string[] ev;
    /// <summary>
    /// ゴールブロック
    /// </summary>
    public string[] goal;
    public string GetName(string type, int num)
    {
        string ret = null;
        switch (type)
        {
            case "ev":
                ret = ev[num];
                break;
            case "goal":
                ret = goal[num];
                break;
        }
        return ret;
    }
}

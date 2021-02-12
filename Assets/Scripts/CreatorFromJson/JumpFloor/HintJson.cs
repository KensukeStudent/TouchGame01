[System.Serializable]
public class Hint:ID
{
    public HintFloor[] floor;
}

/// <summary>
/// フロア内ヒント
/// </summary>
public class HintFloor
{
    public string detail;
}
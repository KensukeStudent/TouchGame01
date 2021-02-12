using System.Collections.Generic;

/// <summary>
/// ブロックの説明を格納
/// </summary>
[System.Serializable]
public class BlockList
{
    public List<Block> blocks = new List<Block>();
}

/// <summary>
/// 敵どくろを格納
/// </summary>
[System.Serializable]
public class DokuroList
{
    public List<DokuroEnemy> dokuros = new List<DokuroEnemy>();
}

/// <summary>
/// Jumpフロア情報を格納
/// </summary>
public class JumpFloorList
{
    public List<JumpingFloor> floors = new List<JumpingFloor>();
}

/// <summary>
/// Hint情報を格納
/// </summary>
public  class HintList
{
    public List<Hint> hints = new List<Hint>();
}
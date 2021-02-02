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

public class JumpFloorList
{
    public List<JumpingFloor> floors = new List<JumpingFloor>();
}
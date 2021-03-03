using System.Collections.Generic;

#region シナリオ

/// <summary>
/// シナリオを格納
/// </summary>
[System.Serializable]
public class ScenarioList
{
    public List<Scenario> stories = new List<Scenario>();
}

#endregion

#region オブジェクト
/// <summary>
/// ブロックの説明を格納
/// </summary>
[System.Serializable]
public class BlockList
{
    public List<Block> blocks = new List<Block>();
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

# endregion

# region 敵

/// <summary>
/// 敵どくろを格納
/// </summary>
[System.Serializable]
public class DokuroList
{
    public List<DokuroEnemy> dokuros = new List<DokuroEnemy>();
}

/// <summary>
/// 大砲を格納
/// </summary>
[System.Serializable]
public class CannonList
{
    public List<CannonJson> cannons = new List<CannonJson>();
}

#endregion
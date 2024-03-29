﻿using UnityEngine;

/// <summary>
/// Jsonから読み出しを行うクラス
/// </summary>
public class JsonInfo
{
    #region マップ生成用Json
    //-------オブジェクト-------//
    public BlockList blockList;
    public JumpFloorList floorList;
    public HintList hintList;

    //-------    敵     -------//
    public DokuroList dokuroList;
    public CannonList cannonList;

    /// <summary>
    /// マップ生成コンストラクター
    /// </summary>
    public JsonInfo() { }

    /// <summary>
    /// マップ生成読み出し用
    /// </summary>
    public void MapJson()
    {
        var load = new SaveLoad();

        load.Load(load.BlockPath, ref blockList);
        load.Load(load.FloorPath, ref floorList);
        load.Load(load.HintPath, ref hintList);
        load.Load(load.EnemyPath, ref dokuroList);
        load.Load(load.EnemyPath, ref cannonList);
    }

    #region オブジェクト

    /// <summary>
    /// 指定のブロックに値を代入
    /// </summary>
    /// <param name="stageId">ステージ名</param>
    /// <param name="blockName">ブロック名(ev,goal)</param>
    /// <param name="num">生成カウント(何番目か)</param>
    /// <param name="block"></param>
    /// <param name="floorNum">現在のフロア数</param>
    public void SetBlock(string stageId, string blockName, BlocksScript block, int num, int floorNum)
    {
        //ブロッククラスを定義
        var b = ID.GetDataNo(blockList.blocks, stageId).floor[floorNum];

        //ブロックの説明を取得
        var des = b.GetName(blockName, num);
        //移動はあるか
        var f = b.jumpFloor[num];
        //名前
        var name = b.name[num];

        //値の代入
        block.SetBlock(des, name, f);
    }

    /// <summary>
    /// 生成された移動フロアオブジェクトに値を代入
    /// </summary>
    /// <param name="floor">代入するクラス</param>
    /// <param name="num">何番目の床か</param>
    /// <param name="floorNum">現在のフロア数</param>
    public void SetFloorVec(string stageId, FloorMoveObj floor, int num, int floorNum)
    {
        //ジャンプフロアクラスを定義
        var f = ID.GetDataNo(floorList.floors, stageId).floor[floorNum];
        //値の代入
        floor.SetMaxMinInit(f.xMax[num], f.yMax[num], f.xMin[num], f.yMin[num], f.direction[num]);
    }

    /// <summary>
    /// 生成されたヒントオブジェクト値を代入
    /// </summary>
    public void SetHint(string stageId, HintCat hC, int num, int floorNum)
    {
        //ヒントクラスを定義
        var h = ID.GetDataNo(hintList.hints, stageId).floor[floorNum];
        //値を代入
        hC.SetInit(h.detail[num]);
    }

    #endregion

    #region 敵

    /// <summary>
    /// 生成されたどくろに値を代入
    /// </summary>
    /// <param name="num">何番目の敵</param>
    public void SetDokuro(string stageId, GameObject gd, int num, int floorNum, string tileNum)
    {
        //どくろクラスの定義
        var d = ID.GetDataNo(dokuroList.dokuros, stageId).floor[floorNum];

        switch (tileNum)
        {
            case "0":
                var dS = gd.GetComponent<DokuroShot>();
                //どくろ(弾)値の代入
                EnemyMan.DokuroShot(dS, d, num);
                break;

            case "1":
                var dm = gd.GetComponent<DokuroMove>();
                //どくろ(移動)値の代入
                dm.SetInit(d.move[0].moveSpeed[num]);
                break;
        }
    }

    /// <summary>
    /// 生成された大砲に値を代入
    /// </summary>
    /// <param name="stageId">ステージID</param>
    /// <param name="c">Cannon</param>
    /// <param name="num">何番目の大砲か</param>
    /// <param name="floorNum">フロア番号</param>
    public void SetCannon(string stageId,Cannon cannon,int num,int floorNum)
    {
        //stageIDとfloor番号の配列を引き出します
        var c = ID.GetDataNo(cannonList.cannons, stageId).floor[floorNum];

        //向き
        var dire = c.direction[num];
        //速度
        var speed = c.speed[num];
        //発射時間
        var shotTime = c.shotTime[num];
        //一回に出す弾の数
        var onceCount = c.onceCount[num];
        //一回に出す弾の間隔
        var inv = c.onceCountInterval[num];

        //大砲に値を入れます
        cannon.Init(dire, speed, shotTime, onceCount, inv);
    }

    #endregion

    #endregion

    #region シナリオ用Json

    public ScenarioList scenarioList;

    /// <summary>
    /// シナリオを読み出し用
    /// </summary>
    public void ScenarioJson()
    {
        var load = new SaveLoad();
        load.Load(load.ScenarioPath, ref scenarioList);
    }

    /// <summary>
    /// シナリオに台本を渡すクラス
    /// </summary>
    /// <param name="tm"></param>
    /// <param name="storyId"></param>
    public void SetScenario(ScenarioReader sr, string storyId)
    {
        //格納されたリストを取得
        var s = ID.GetDataNo(scenarioList.stories, storyId).scenario;
        
        //srのリストにシナリオを入れます
        sr.SetArray(s);
    }
    #endregion

}
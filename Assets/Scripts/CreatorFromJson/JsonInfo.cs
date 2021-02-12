using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

/// <summary>
/// Jsonから読み出しを行うクラス
/// </summary>
public class JsonInfo
{
    public BlockList blockList;
    public DokuroList dokuroList;
    public JumpFloorList floorList;
    public HintList hintList;

    public JsonInfo()
    {
        Load("Block", ref blockList);
        Load("EnemyState", ref dokuroList);
        Load("JumpFloor", ref floorList);
        Load("Hint", ref hintList);
    }

    /// <summary>
    /// ステージごとのJsonを読み込み
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <param name="data"></param>
    void Load<T>(string fileName, ref T data) where T : class
    {
        try
        {
            using (StreamReader sr = new StreamReader(Application.dataPath + "/Resources/Json/"+ fileName + ".json"))
            {
                //ファイルの読み込み
                string json = sr.ReadToEnd();
                //読み込んだデータをオブジェクト化します
                data = JsonMapper.ToObject<T>(json);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("データがありません。");
        }
    }

    /// <summary>
    /// 指定のブロックに値を代入
    /// </summary>
    /// <param name="stageId">ステージ名</param>
    /// <param name="blockName">ブロック名(ev,goal)</param>
    /// <param name="num">生成カウント(何番目か)</param>
    /// <param name="block"></param>
    /// <param name="floorNum">現在のフロア数</param>
    public void SetBlock(string stageId, string blockName, BlocksScript block, int num,int floorNum)
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
    /// 生成されたどくろに値を代入
    /// </summary>
    /// <param name="num">何番目の敵</param>
    public void SetDokuro(string stageId,DokuroShot dS, int num,int floorNum)
    {
        //どくろクラスの定義
        var d = ID.GetDataNo(dokuroList.dokuros, stageId);

        //値の代入
        EnemyMan.DokuroShot(dS, d, floorNum, num);
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
    public void SetHint(string stageId,HintCat hC,int floorNum)
    {
        //ヒントクラスを定義
        var h = ID.GetDataNo(hintList.hints, stageId).floor[floorNum];
        //値を代入
        hC.SetInit(h.detail);
    }
}
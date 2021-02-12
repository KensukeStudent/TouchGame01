using System;
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

    public JsonInfo()
    {
        Load("Block", ref blockList);
        Load("EnemyState", ref dokuroList);
        Load("JumpFloor", ref floorList);
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
    public void SetBlock(string stageId, string blockName, int num, BlocksScript block,int floorNum)
    {
        //ステージ番号のブロックリストの設定
        var blocks = blockList.blocks.Find(bs => bs.id == stageId);

        if (blocks == null)
        {
            Debug.LogError("sceneId:名前が違う,id名が配列外エラー");
            Debug.LogError("scenarioName:名前が違う,Resourcesの中にあるJsonファイルを参照");
        }

        //そのステージ名が何番目にあるかを取得
        var listNum = blockList.blocks.IndexOf(blocks);
        //ブロッククラスを定義
        var b = blockList.blocks[listNum].floor[floorNum];
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
    /// 生成されたどくろのステータス場所を参照
    /// </summary>
    /// <param name="stageId"></param>
    /// <param name="num">何番目の敵</param>
    public void SetDokuro(string stageId,DokuroShot dokuro,int floorNum,int num)
    {
        //ステージ番号の敵を指定
        var d = dokuroList.dokuros.Find(ds => ds.id == stageId);

        if (d == null)
        {
            Debug.LogError("sceneId:名前が違う,id名が配列外エラー");
            Debug.LogError("scenarioName:名前が違う,Resourcesの中にあるJsonファイルを参照");
        }
        //そのステージ名が何番目にあるかを取得
        var listNum = dokuroList.dokuros.IndexOf(d);

        //値の代入
        EnemyMan.DokuroShot(dokuro, dokuroList, listNum, floorNum, num);
    }

    /// <summary>
    /// 生成された移動フロアオブジェクトの最大最小のベクトルを参照
    /// </summary>
    public void SetFloorVec(string stageId,FloorMoveObj floor,int num,int floorNum)
    {
        //ステージ番号の敵を指定
        var floorJ = floorList.floors.Find(fs => fs.id == stageId);

        if (floorJ == null)
        {
            Debug.LogError("sceneId:名前が違う,id名が配列外エラー");
            Debug.LogError("scenarioName:名前が違う,Resourcesの中にあるJsonファイルを参照");
        }

        //そのステージ名が何番目にあるかを取得
        var listNum = floorList.floors.IndexOf(floorJ);
        //ブロッククラスを定義
        var f = floorList.floors[listNum].floor[floorNum];
        //値の代入
        floor.SetMaxMinInit(f.xMax[num], f.yMax[num], f.xMin[num], f.yMin[num], f.direction[num]);
    }
}
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全てのJsonが持つStageID
/// </summary>
[System.Serializable]
public class ID
{
    public string id;

    /// <summary>
    /// StageIdから指定のJsonListに格納された要素を見つけます
    /// </summary>
    /// <typeparam name="T">IDクラスを継承しているJsonのクラスT</typeparam>
    /// <param name="dataId">現在のstage番号</param>
    /// <param name="data">Jsonクラス</param>
    public static T GetDataNo<T>(List<T> data, string dataId) where T : ID
    {
        //ステージIDのリスト配列番号を取得
        var dataNo = data.Find(dt => dt.id == dataId);

        if (dataNo == null)
        {
            Debug.LogError("sceneId:名前が違う,id名が配列外エラー");
            Debug.LogError("scenarioName:名前が違う,Resourcesの中にあるJsonファイルを参照");
        }

        //そのステージ名が何番目にあるかを取得
        var index = data.IndexOf(dataNo);

        return data[index];
    }

}
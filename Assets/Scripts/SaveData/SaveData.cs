using LitJson;
using System;
using System.IO;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="sm"></param>
    public SaveData(StageManager sm)
    {
        if (Save(sm))
        {
            //save時の演出
            Debug.Log("セーブ完了");
        }
    } 

    /// <summary>
    /// ステージのデータをセーブします
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="data">StageManagerクラス</param>
    /// <returns></returns>
    bool Save(StageManager sm)
    {
        bool ret = false;

        //セーブ情報を入れます
        var data = new StageData(sm);

        //データをJson化させます
        string str = JsonMapper.ToJson(data);

        const string dataPath = "save.json";

        try
        {
            //ファイルに書き込みします
            using (StreamWriter sw = new StreamWriter(dataPath))
            {
                sw.Write(str);
            }
            ret = true; //成功
        }
        catch (UnauthorizedAccessException e)
        {
            Debug.Log("ファイルが書き込み禁止です");
        }
        catch (IOException e)
        {
            Debug.Log("ファイルの書き込みに失敗しました");
        }

        return ret;
    }
}

/// <summary>
/// StageDataクラス
/// </summary>
public class StageData
{
    /// <summary>
    /// 各ステージの達成度
    /// </summary>
    public int[][] scoreData;
    /// <summary>
    /// 各ステージ達成時のアニメーション
    /// </summary>
    public bool[][] animData;
    /// <summary>
    /// 各ステージのクリア
    /// </summary>
    public bool[] clearData;

    /// <summary>
    /// コンストラクター
    /// </summary>
    /// <param name="sm">参照先のデータ</param>
    public StageData(StageManager sm)
    {
        SetValue(sm);
    }

    /// <summary>
    /// 値を代入します
    /// </summary>
    public void SetValue(StageManager sm)
    {
        scoreData = sm.scoreMan;
        animData = sm.scoreAnimMan;
        clearData = sm.stageClearMan;
    }
}


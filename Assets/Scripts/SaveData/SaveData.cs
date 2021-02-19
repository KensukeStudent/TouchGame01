using LitJson;
using System;
using System.IO;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    #region FilePath
    public string SavePath { get; } = "SaveData/save.json";
    public string BlockPath { get; } = "Json/Block.json";
    public string DokuroPath { get; } = "Json/EnemyState.json";
    public string FloorPath { get; } = "Json/JumpFloor.json";
    public string HintPath { get; } = "Json/Hint.json";
    public string ScenarioPath { get; } = "Scenario/Story.json";
    #endregion

    /// <summary>
    /// コンストラクター
    /// </summary>
    public SaveLoad() { }

    /// <summary>
    /// コンストラクター(Save付き)
    /// </summary>
    /// <param name="sm"></param>
    public SaveLoad(StageManager sm)
    {
        if (Save(sm))
        {
            //save時の演出
            Debug.Log("セーブ完了");
            return;
        }
    } 

    /// <summary>
    /// ステージのデータをセーブします
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="data">StageManagerクラス</param>
    /// <returns></returns>
    public bool Save(StageManager sm)
    {
        bool ret = false;

        //セーブ情報を入れます
        var data = new StageData(sm);

        string dataPath = Application.dataPath + "/Resources/" + SavePath;

        try
        {
            //ファイルに書き込みします
            using (StreamWriter sw = new StreamWriter(dataPath))
            {
                //データをJson化させます
                string str = JsonMapper.ToJson(data);

                //書き込み
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

    /// <summary>
    /// ステージごとのJsonを読み込み
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <param name="data"></param>
    public bool Load<T>(string file,ref T data) where T : class
    {
        bool ret = false;

        try
        {
            using (var sr = new StreamReader(Application.dataPath + "/Resources/" + file))
            {
                //ファイルの読み込み
                string json = sr.ReadToEnd();
                //読み込んだデータをオブジェクト化します
                data = JsonMapper.ToObject<T>(json);
                ret = true;
            }
        }
        catch (Exception e)
        {
            Debug.Log("データを新しく作成します");
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
    public int[][] ScoreData { private set; get; }
    /// <summary>
    /// 各ステージ達成時のアニメーション
    /// </summary>
    public bool[][] AnimData_S { private set; get; }
    /// <summary>
    /// 各ステージのクリア
    /// </summary>
    public bool[] ClearData { private set; get; }
    /// <summary>
    /// 各ステージのアニメーション
    /// </summary>
    public bool[] AnimData_C { private set; get; }
    /// <summary>
    /// コンストラクター
    /// </summary>
    public StageData() { }

    /// <summary>
    /// コンストラクター(セーブ)
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
        ScoreData = sm.ScoreMan;
        AnimData_S = sm.ScoreAnimMan;
        ClearData = sm.StageClearMan;
        AnimData_C = sm.ClearAnimMan;
    }
}


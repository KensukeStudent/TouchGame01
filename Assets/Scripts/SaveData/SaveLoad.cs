using LitJson;
using System;
using System.IO;
using UnityEngine;

public class SaveLoad
{
    #region FilePath
    /// <summary>
    /// セーブデータ
    /// </summary>
    public string SavePath { get; } = "/save.json";
    /// <summary>
    /// ブロック
    /// </summary>
    public string BlockPath { get; } = "Json/Block";
    /// <summary>
    /// 敵一覧
    /// </summary>
    public string EnemyPath { get; } = "Json/EnemyState";
    /// <summary>
    /// フロア
    /// </summary>
    public string FloorPath { get; } = "Json/JumpFloor";
    /// <summary>
    /// ヒント
    /// </summary>
    public string HintPath { get; } = "Json/Hint";
    /// <summary>
    /// シナリオ
    /// </summary>
    public string ScenarioPath { get; } = "Scenario/Story";

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
        //セーブ
        Save(sm);
    } 

    /// <summary>
    /// ステージのデータをセーブします
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="data">StageManagerクラス</param>
    /// <returns></returns>
    public void Save(StageManager sm)
    {
        //セーブ情報を入れます
        var data = new StageData(sm);
        //セーブpath
        string dataPath = Application.persistentDataPath + SavePath;
        try
        {
            //書き込みます
            Write(dataPath, data);
        }
        catch (UnauthorizedAccessException e)
        {
            Debug.Log("ファイルが書き込み禁止です");
        }
        catch (IOException e)
        {
            Debug.Log("ファイルの書き込みに失敗しました");
            //ファイルの作成
            File.Create(dataPath);
            //書き込みます
            //Write(dataPath, data);
        }
    }

    /// <summary>
    /// 書き込み
    /// </summary>
    /// <param name="dataPath">パス</param>
    /// <param name="data">saveData</param>
    void Write(string dataPath, StageData data)
    {
        //ファイルに書き込み(なければ作成)します
        using (var sw = new StreamWriter(dataPath))
        {
            //データをJson化させます
            string str = JsonMapper.ToJson(data);
            //書き込み
            sw.Write(str);
            //ファイルを閉じます
            sw.Close();//しなくてもよい
        }
    }

    /// <summary>
    /// セーブデータのロード
    /// </summary>
    /// <returns></returns>
    public bool SaveDataLoad(ref StageData data)
    {
        //読み込み成功したらtrueを返します
        bool ret = false;
        var path = Application.persistentDataPath + SavePath;
        try
        {
            //データがあれば読み取る
            using (var sr = new StreamReader(path))
            {
                //json読み込み
                var json = sr.ReadToEnd();
                //読み込んだデータをオブジェクト化します
                data = JsonMapper.ToObject<StageData>(json);
                ret = true;
            }
        }
        catch (Exception e)
        {
            //Debug.Log("データがありません");
        }

        return ret;
    }

    /// <summary>
    /// ステージごとのJsonを読み込み
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <param name="data"></param>
    public bool Load<T>(string file , ref T data) where T : class
    {
        //読み込み成功したらtrueを返します
        bool ret = false;

        //読み込みはすべてResouse.Loadから読み込み

        try
        {
            var jsonFile = Resources.Load<TextAsset>(file);

            if (jsonFile)
            {
                //ファイルの読み込みました
                string json = jsonFile.text;
                //読み込んだデータをオブジェクト化します
                data = JsonMapper.ToObject<T>(json);
                //オブジェクト化成功
                ret = true;
            }
        }
        catch (Exception e)
        {
            //Debug.Log("データがありません");
        }

        return ret;
    }

    /// <summary>
    /// ファイルの削除を行います
    /// </summary>
    public void FileDelete(StageManager sm)
    {
        //指定のファイルパスのデータを削除します
        string path = Application.persistentDataPath + SavePath;

        if(File.Exists(path))
        {
            //ファイルの削除  
            File.Delete(path);
            //Debug.Log("存在する");
        }

        //新たにデータを作成します
        sm.InitData();
    }
}

/// <summary>
/// StageDataクラス
/// </summary>
public class StageData
{
    /// <summary>
    /// ステージセレクトに入ったのは初めてか
    /// </summary>
    public bool IsFirst { private set; get; }
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
        IsFirst = StageManager.IsSelectScene;
        ScoreData = sm.ScoreMan;
        AnimData_S = sm.ScoreAnimMan;
        ClearData = sm.StageClearMan;
        AnimData_C = sm.ClearAnimMan;
    }
}


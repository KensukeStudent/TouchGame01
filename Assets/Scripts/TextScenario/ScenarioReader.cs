using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シナリオの読み込みリスト
/// </summary>
public class ScenarioReader : MonoBehaviour
{
    /// <summary>
    /// 出力するシナリオを格納します
    /// </summary>
    string[] listScenario;
    /// <summary>
    /// 現在の出力するリストのIndex番号
    /// </summary>
    int index = -1;
    /// <summary>
    /// JsonInfoを入れます
    /// </summary>
    JsonInfo ji;

    /// <summary>
    /// コンストラクター
    /// </summary>
    public ScenarioReader()
    {
        ji = new JsonInfo();
        //シナリオを読み込みます
        ji.ScenarioJson();

        //storyのIDをセットしてその配列を読み込みます
        SetStoriesID("opeing");
    }

    /// <summary>
    /// 読み込むstoryのIDをセットします
    /// </summary>
    /// <param name="id"></param>
    public void SetStoriesID(string id)
    {
        ji.SetScenario(this, id);
    }

    /// <summary>
    /// シナリオ配列に値を入れます
    /// </summary>
    public void SetArray(string[] scenarioArray) 
    {
        listScenario = scenarioArray;
    }

    /// <summary>
    /// Index番号を上げます
    /// </summary>
    public string IncreaseIndex()
    {
        index++;
        return _ = listScenario[index];
    }

    /// <summary>
    /// シナリオを最後まで読み切りました
    /// </summary>
    /// <returns></returns>
    public bool FinishScenario()
    {
        return _ = index == listScenario.Length - 1;
    }
}

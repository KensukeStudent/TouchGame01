/// <summary>
/// シナリオの読み込みリスト
/// </summary>
public class ScenarioReader
{
    /// <summary>
    /// シナリオの読み込むID
    /// </summary>
    public static string scenarioId = "opeing";

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
    /// コンストラクターシナリオ用
    /// </summary>
    /// <param name="id">初期値 = "opeing"</param>
    public ScenarioReader() { }

    /// <summary>
    /// シナリオではこちらを処理します
    /// </summary>
    public void SinarioModeInit()
    {
        ji = new JsonInfo();
        
        //シナリオを読み込みます
        ji.ScenarioJson();      

        //storyのIDをセットしてその配列を読み込みます
        SetStoriesID(scenarioId);
    }

    /// <summary>
    /// 読み込むstoryのIDをセットします
    /// </summary>
    /// <param name="id"></param>
    void SetStoriesID(string id)
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

    /// <summary>
    /// indexが0以上なら読み始めます
    /// </summary>
    public bool IsReading()
    {
        return _ = index > -1;
    }
}

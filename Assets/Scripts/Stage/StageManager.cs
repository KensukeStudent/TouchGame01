using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// 各ステージを管理するクラス
/// </summary>
public class StageManager : MonoBehaviour
{
    /// <summary>
    /// 現在の全ステージ数
    /// </summary>
    const int stageCount = 4;

    /// <summary>
    /// 各ステージ背景
    /// </summary>
    Sprite[] backGroundMan = new Sprite[stageCount];

    /// <summary>
    /// 各ステージの達成度
    /// </summary>
    public int[][] scoreMan { private set; get; } = new int[stageCount][];//saveData記憶
    /// <summary>
    /// 各ステージ達成時のアニメーション
    /// </summary>
    public bool[][] scoreAnimMan { private set; get; } = new bool[stageCount][];//saveData記憶(上が1(クリア)でanimがfalseなら再生します)

    /// <summary>
    /// 各ステージのクリア
    /// </summary>
    public bool[] stageClearMan { private set; get; } = new bool[stageCount];//saveData記憶

    private void Awake()
    {
        //各ステージ初期値を設定します ----->SaveDataから参照
        InitData();

        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// ゲーム開始時のデータの動き
    /// </summary>
    void InitData()
    {
        var load = new SaveLoad();

        var data = new StageData();

        //セーブデータがあるなら
        if (load.Load(load.SavePath,ref data))
        {
            //ロードします
            SetBack();
            //セーブデータを参照します
            SetData(data);
        }
        else
        {
            //無ければ初期データを作りセーブします
            SetBack();
            InitStageSet();
            load.Save(this);
        }
    }

    /// <summary>
    /// データをロードします
    /// </summary>
    public void SetData(StageData data)
    {
        scoreMan = data.ScoreData;
        scoreAnimMan = data.AnimData;
        stageClearMan = data.ClearData;
    }

    /// <summary>
    /// 背景を取得します
    /// </summary>
    void SetBack()
    {
        //ステージ背景のパス
        const string backPath = "StageBack/StageBack_";

        for (int b = 0; b < backGroundMan.Length; b++)
        {
            //パスから背景画像を取得し入れます
            var sprite = Resources.Load<Sprite>(backPath + b);
            backGroundMan[b] = sprite;
        }
    }

    /// <summary>
    /// ステージの初期値をセットします
    /// </summary>
    void InitStageSet()
    {
        //ここから下はセーブデータがなければ初めに作成

        //初期値未クリア値を宣言
        int[] initClear = { 0, 0, 0 };
        bool[] animFlag = { false, false, false };

        //クリア管理に設定します
        for (int c = 0; c < scoreMan.Length; c++) scoreMan[c] = initClear;//saveData

        //アニメーションフラグを管理します
        for (int a = 0; a < scoreAnimMan.Length; a++) scoreAnimMan[a] = animFlag;//saveData

        //ステージクリアを初期値で設定します
        for (int s = 0; s < stageClearMan.Length; s++) stageClearMan[s] = false;//saveData
    }

    /// <summary>
    /// 各ステージに情報を生成時に入れます
    /// </summary>
    public void SetStageValue(StageContent sc, int num)
    {
        //Stage名を数字の値にします
        var stageNo = "Stage" + (num + 1);
        //ステージ名、そのステージのスコア、背景、クリア済みかの情報を入れます
        sc.SetContent(stageNo, scoreMan[num], backGroundMan[num], stageClearMan[num]);

        //Buttonスクリプトを持っている子要素にアクセスします
        var b = sc.transform.Find("Button").GetComponent<Button>();
        b.onClick.AddListener(() => StageScene(num));
    }

    /// <summary>
    /// 各ステージの要素をButtonに割り当てます
    /// </summary>
    void StageScene(int stageNo)
    {
        //生成するステージを指定します
        GameManager.Instance.StageSet(stageNo);

        //スクールやスクロールボタンフラグを消して操作できなくします

        //シーン遷移を開始します
        ScreenTransition.Instance.TimeST(0);
    }
}

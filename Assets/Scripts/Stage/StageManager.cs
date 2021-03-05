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
    /// 各ステージの手数
    /// </summary>
    readonly int[,] jumpCount = new int[stageCount, 3] //全部がクリアしたらバグる---->ScoreManがすべてクリア状態になる(ことがあった)
    {
        { 50, 35, 28 },
        { 50, 35, 28 },
        { 50, 35, 28 },
        { 50, 35, 28 }
    };

    /// <summary>
    /// 各ステージ背景
    /// </summary>
    public Sprite[] BackGroundMan { private set; get; } = new Sprite[stageCount];

    /// <summary>
    /// 各ステージの達成度
    /// </summary>
    public int[][] ScoreMan { private set; get; } = new int[stageCount][];//saveData記憶
    /// <summary>
    /// 各ステージ達成時のアニメーション
    /// </summary>
    public bool[][] ScoreAnimMan { private set; get; } = new bool[stageCount][];//saveData記憶(上が1(クリア)でanimがfalseなら再生します)

    /// <summary>
    /// 各ステージのクリア
    /// </summary>
    public bool[] StageClearMan { private set; get; } = new bool[stageCount];//saveData記憶

    /// <summary>
    /// 各ステージのクリアアニメーション
    /// </summary>
    public bool[] ClearAnimMan { private set; get; } = new bool[stageCount];

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    #region データ管理・更新
    
    /// <summary>
    /// ゲーム開始時のデータの動き
    /// </summary>
    public void InitData()
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
        ScoreMan = data.ScoreData;
        ScoreAnimMan = data.AnimData_S;
        StageClearMan = data.ClearData;
        ClearAnimMan = data.AnimData_C;
    }

    /// <summary>
    /// 背景を取得します
    /// </summary>
    void SetBack()
    {
        //ステージ背景のパス
        const string backPath = "StageBack/StageBack_";

        for (int b = 0; b < BackGroundMan.Length; b++)
        {
            //パスから背景画像を取得し入れます
            var sprite = Resources.Load<Sprite>(backPath + b);
            BackGroundMan[b] = sprite;
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
        bool[] flag = { false, false, false };

        //クリア管理に設定します
        for (int c = 0; c < ScoreMan.Length; c++) ScoreMan[c] = initClear;//saveData

        //アニメーションフラグを管理します
        for (int a = 0; a < ScoreAnimMan.Length; a++) ScoreAnimMan[a] = flag;//saveData

        //ステージクリアを初期値で設定します
        for (int s = 0; s < StageClearMan.Length; s++)
        {
            StageClearMan[s] = false;//saveData
            ClearAnimMan[s] = false;//saveData
        }
    }

    /// <summary>
    /// ステージの進行度を更新します
    /// </summary>
    public void StageUpdate(int stage, int jumpSum)
    {
        //指定のjumpCount以下ならscoreを更新します

        int score0 = ScoreMan[stage][0];
        int score1 = ScoreMan[stage][1];
        int score2 = ScoreMan[stage][2];

        int[] scores = { score0, score1, score2 };

        for (int i = 0; i < jumpCount.GetLength(1); i++)
        {
            if (jumpSum <= jumpCount[stage, i] && !ScoreAnimMan[stage][i])
            {
                scores[i] = 1;
            }
        }

        ScoreMan[stage] = scores;

        if (!StageClearMan[stage]) StageClearMan[stage] = true;
    }
   
    /// <summary>
    /// ゲーム内でプレイヤーがクリア又は倒れた際に処理します
    /// </summary>
    public void GameEnd(Animator anim,string clipName)
    {
        //アニメーションクリップ取得
        var clip = TimeZeroAnim.GetAnimTime(anim, clipName);

        //クリップの長さ
        var waitLag = clip.length;

        //sceneステートを変更します
        ScreenTransition.Instance.ChangeState(SceneState.gameOverMode);

        //プレイヤーのクリア又は倒れるアニメーションが終わったら遷移を開始します
        ScreenTransition.Instance.TimeST(waitLag);

        //ゲーム開始フラグを切ります
        GameManager.Instance.SetGame(false);
    }

    #endregion

    #region ステージに情報の割り当て

    /// <summary>
    /// 各ステージに情報を生成時に入れます
    /// </summary>
    public void SetStageValue(StageContent sc, int num)
    {
        //Stage名を数字の値にします
        var stageNo = "Stage" + (num + 1);
        //ステージ名、そのステージのスコア、背景、クリア済みかの情報を入れます
        sc.SetContent(stageNo, this);

        //Buttonスクリプトを持っている子要素にアクセスします
        var b = sc.transform.Find("Button").GetComponent<Button>();

        b.onClick.RemoveAllListeners();//ボタンに何回も関数を入れないようにするため削除処理を入れます
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
        var scroll = GameObject.Find("Content").GetComponent<ScrollSelect>();
        scroll.SetSelect();
        //シーン遷移を開始します
        ScreenTransition.Instance.TimeST(0);
    }

    #endregion

    #region クリアアニメーション管理

    /// <summary>
    /// アニメーションを再生したフラグを立てます
    /// </summary>
    /// <param name="stageNo">現在のStage番号</param>
    /// <param name="anim_S">anim -> stageAnim</param>
    public void SetAnimFlag_S(int stageNo, bool[] anim_S)
    {
        ScoreAnimMan[stageNo] = anim_S;
    }

    /// <summary>
    /// アニメーションを再生したフラグを立てます
    /// </summary>
    /// <param name="stageNo">現在のStage番号</param>
    /// <param name="anim_C">anim -> clearAnim</param>
    public void SetAnimFlag_C(int stageNo, bool anim_C)
    {
        ClearAnimMan[stageNo] = anim_C;
    }

    /// <summary>
    /// ステージのアニメーションが終わった後に処理します
    /// </summary>
    public void StageAnimFinish()
    {
        //アニメーションが終了後セーブします
        var sm = GameObject.Find("StageManager").GetComponent<StageManager>();
        var save = new SaveLoad(sm);

        ////終わり次第ステージを選択、クリック可能にします----->アニメシーンのラグを作成
        var scroll = GameObject.Find("Content").GetComponent<ScrollSelect>();
        scroll.SetSelect();
    }

    #endregion
}

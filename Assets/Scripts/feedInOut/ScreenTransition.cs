using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public enum SceneState
{
    stageSelect,//ステージを選択後遷移開始->selectEnd
    selectEnd,//遷移が終了後GameSceneを変更(stageへ)->gameView
    touch,//クリック可能　クリック後(左右から猫の手が出る遷移開始)終わったら->touchEnd
    touchEnd,//遷移終了後ゲーム開始 ->gameMode
    gameMode,//ゲームクリアしプレイヤーにアニメーション終了後gameOverMode
    gameOverMode,//閉じる遷移を開始,終了後->stageSelect
    reState,//元のステートに戻します
}


/// <summary>
/// ステージ選択ステージクリア時のシーン遷移クラス
/// </summary>
public class ScreenTransition : MonoBehaviour
{
    /// <summary>
    /// 遷移のステート
    /// </summary>
    SceneState state = SceneState.stageSelect;
    /// <summary>
    /// Scene遷移クラス
    /// </summary>
    public static ScreenTransition Instance { private set; get; } 
    /// <summary>
    /// 親canvasのRect
    /// </summary>
    [SerializeField] RectTransform canvas;
    /// <summary>
    /// 生成するtile
    /// </summary>
    [SerializeField] GameObject tilePrefab;

    /// <summary>
    /// 遷移の関数を格納
    /// </summary>
    /// <param name="wCounter"></param>
    /// <param name="hCounter"></param>
    /// <param name="tileSizeW"></param>
    /// <param name="tileSizeH"></param>
    delegate void Calc(int wCounter, int hCounter, float tileSizeW, float tileSizeH);
    Calc[] calcList;
    Calc calc;

    //AnchorSize
    Vector2 upLeftAnchor = new Vector2(0, 1);
    Vector2 upRightAnchor = new Vector2(1, 1);
    Vector2 upCenterAnchor = new Vector2(0.5f, 1);
    Vector2 bottomCenterAncor = new Vector2(0.5f, 0);

    /// <summary>
    /// 動作終わらせる秒数
    /// </summary>
    const int second = 1;

    /// <summary>
    /// 生成したタイルを保存
    /// </summary>
    List<FeedInOut> tileList = new List<FeedInOut>();

    /// <summary>
    /// シーンを遷移オート
    /// </summary>
    bool autoFlag = false;

    /// <summary>
    /// 遷移のパターン
    /// </summary>
    public int Pattern { private set; get; } = 0;

    /// <summary>
    /// GameStart前に遷移中にクリック不可能にするためのフラグ
    /// </summary>
    public bool TouchClick { private set; get; } = false;

    /// <summary>
    /// 遷移をオートで始めるタイムラグ
    /// </summary>
    const float tLag = 1.0f;

    private void Awake()
    {
        //Sceneをまたいでもこのオブジェクトを引き継ぎます
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        //使う関数を格納します
        calcList = new Calc[]
        {
            UpDown,
            Center,
            RightAndLeft
        };
    }

    #region 遷移をさせるための関数

    /// <summary>
    /// 共通の初期値
    /// </summary>
    void InitInstant(out GameObject tile, out RectTransform rt, out FeedInOut feed, Vector2 anchor)
    {
        //綺麗に並べる
        tile = Instantiate(tilePrefab);
        rt = tile.transform as RectTransform;
        //アンカーを指定します
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        //canvasを親指定します
        tile.transform.SetParent(canvas);
        
        feed = tile.GetComponent<FeedInOut>();
        //生成するtileを格納します
        tileList.Add(feed);
    }

    /// <summary>
    /// 上と下に画面の半分分の大きさを生成
    /// </summary>
    void UpDown(int wCounter, int hCounter, float tileSizeW, float tileSizeH)
    {
        //アンカーの位置を配列にしまいます
        Vector2[] anchorPos = { upCenterAnchor, bottomCenterAncor };

        for (int i = 0; i < 2; i++)
        {
            GameObject tile = null;
            RectTransform rt = null;
            FeedInOut feed = null;
            var initPosY = 0.0f;//y軸の初期位置
            var goalRtY = 0.0f; ;//最終到着地点
            var speedY = 0.0f; ;//到着地点までの速度

            var direct = "";

            //アンカーポジションに応じて生成位置を指定します
            InitInstant(out tile, out rt, out feed, anchorPos[i]);

            switch (i)
            {
                //どちらもサイズの二倍、上と下に生成

                //上に生成
                case 0: 
                    //初期位置
                    initPosY = tileSizeH / 2 + tileSizeH; 
                    //移動方向を入れます
                    direct = "DOWN"; 
                    break;

                //下に生成
                case 1:
                    //初期位置
                    initPosY = -tileSizeH / 2 - tileSizeH;
                    //移動方向
                    direct = "UP";
                    break;
            }
            //タイルのサイズや目的位置を求めます
            TileInfo(rt, initPosY, wCounter, tileSizeW, out goalRtY);
            //0の時は下方向に移動したいのでマイナスに変換します
            if (i == 0) goalRtY = -goalRtY;
            //速度を「はじき」で計算 距離 / second秒で移動する速さ
            speedY = (goalRtY - rt.anchoredPosition.y) / second;
            //情報をタイルへ伝えます
            feed.SetPosKey(goalRtY, speedY, direct);
        }
    }

    /// <summary>
    /// サイズを指定の大きさに、目的位置を設定
    /// </summary>
    void TileInfo(RectTransform rt, float initPosY, int wCounter, float tileSizeW, out float goalRtY)
    {
        //生成したtileRectの位置座標を取得
        rt.anchoredPosition = new Vector2(0, initPosY);

        //サイズを設定します
        var size = rt.sizeDelta;
        //横の長さを横に出せる分の数でサイズを取得します ※---->canvas.sizeDlta.xでも同じこと
        size.x = tileSizeW * wCounter;
        size.y = canvas.sizeDelta.y / 2;
        rt.sizeDelta = size;

        //ゴール位置、画面中心の座標
        goalRtY = rt.sizeDelta.y / 2;
    }

    /// <summary>
    /// 中心にタイルを貼ります
    /// </summary>
    void Center(int wCounter, int hCounter, float tileSizeW, float tileSizeH)
    {
        //アンカーの位置を配列にしまいます
        Vector2[] anchorPos = { upCenterAnchor, bottomCenterAncor };

        for (int i = 0; i < 2; i++)
        {
            GameObject tile = null;
            RectTransform rt = null;
            FeedInOut feed = null;
            var initPosY = 0.0f;//y軸の初期位置
            var goalRtY = 0.0f; ;//最終到着地点
            var speedY = 0.0f; ;//到着地点までの速度

            var direct = "";

            //アンカーポジションに応じて生成位置を指定します
            InitInstant(out tile, out rt, out feed, anchorPos[i]);

            rt.sizeDelta = new Vector2(canvas.sizeDelta.x, canvas.sizeDelta.y / 2);

            switch (i)
            {
                //上の中心
                case 0:
                    initPosY = -canvas.sizeDelta.y / 2 + rt.sizeDelta.y / 2;
                    direct = "DOWN";
                    break;

                //下の中心
                case 1:
                    initPosY = canvas.sizeDelta.y / 2 - rt.sizeDelta.y / 2;
                    direct = "UP";
                    break;
            }
            //中心にタイルを生成します
            CenterSizePos(rt, initPosY, out goalRtY);
            //速度を「はじき」で計算 距離 / second秒で移動する速さ
            speedY = (goalRtY - rt.anchoredPosition.y) / second;
            //情報をタイルへ伝えます
            feed.SetPosKey(goalRtY, speedY, direct);
        }
    }

    /// <summary>
    /// 中心から生成する時に呼ぶ処理
    /// </summary>
    void CenterSizePos(RectTransform rt,float initPosY,out float goalRtY)
    {
        //位置指定
        rt.anchoredPosition = new Vector2(0, initPosY);

        //ゴール位置、上下画面外
        goalRtY = -rt.anchoredPosition.y;
    }

    /// <summary>
    /// 左右に生成
    /// </summary>
    void RightAndLeft(int wCounter, int hCounter, float tileSizeW, float tileSizeH)
    {
        for (int i = 0; i < hCounter * 2; i++)
        {
            GameObject tile = null;
            RectTransform rt = null;
            //高さのマージン
            var hLag = tileSizeH * (i % hCounter);
            float wPos = 0;
            float goalPos = 0;
            FeedInOut feed = null;

            if (i < hCounter && i % 2 == 0)
            {
                InitInstant(out tile, out rt, out feed, upLeftAnchor);
                SetSize(rt, wCounter, tileSizeW, out wPos, out goalPos);
                rt.anchoredPosition = new Vector2(-wPos, -tileSizeH / 2 - hLag);
                var speed = (goalPos - rt.anchoredPosition.x) / second;
                feed.SetSpeed(rt.sizeDelta.x / 2, speed, "RIGHT");
            }
            //hCounter偶数奇数かで処理が分かれる
            else if (hCounter % 2 == 0 && i >= hCounter && i % 2 != 0)
            {
                InitInstant(out tile, out rt, out feed, upRightAnchor);
                SetSize(rt, wCounter, tileSizeW, out wPos, out goalPos);
                rt.anchoredPosition = new Vector2(wPos, -tileSizeH / 2 - hLag);
                var speed = (-goalPos - rt.anchoredPosition.x) / second;
                feed.SetSpeed(-rt.sizeDelta.x / 2, speed, "LEFT");
            }
            else if (hCounter % 2 != 0 && i >= hCounter && i % 2 == 0)
            {
                InitInstant(out tile, out rt, out feed, upRightAnchor);
                SetSize(rt, wCounter, tileSizeW, out wPos, out goalPos);
                rt.anchoredPosition = new Vector2(wPos, -tileSizeH / 2 - hLag);
                var speed = (-goalPos - rt.anchoredPosition.x) / second;
                feed.SetSpeed(-rt.sizeDelta.x / 2, speed, "LEFT");
            }
        }
    }

    /// <summary>
    /// サイズと目標地点までの速度を指定
    /// </summary>
    void SetSize(RectTransform rt, int wCounter, float tileSizeW, out float wPos, out float goalPos)
    {
        var size = rt.sizeDelta;
        size.x = tileSizeW * wCounter;
        rt.sizeDelta = size;
        wPos = size.x / 2;
        goalPos = rt.sizeDelta.x / 2;
    }

    #endregion

    #region 遷移を呼び出す関数

    /// <summary>
    /// 生成するタイルの大きさとキャンバス内に生成できる幅から生成数を求める
    /// </summary>
    /// <param name="pattern">0か1</param>
    public void SetSizeWHCount(int pattern)
    {
        //キャンバスサイズ
        var canvasSizeX = canvas.sizeDelta.x;
        var canvasSizeY = canvas.sizeDelta.y;
        //タイルのサイズ
        var tileSizeW = (tilePrefab.transform as RectTransform).sizeDelta.x;
        var tileSizeH = (tilePrefab.transform as RectTransform).sizeDelta.y;

        //このキャンバス内に出せる数を切り上げ計算
        var wCounter = Mathf.CeilToInt(canvasSizeX / tileSizeW);
        var hCounter = Mathf.CeilToInt(canvasSizeY / tileSizeH);
        //パターンを入れます
        calc = calcList[pattern];

        //情報から生成するサイズや個数が決まります
        calc(wCounter, hCounter, tileSizeW, tileSizeH);
    }

    /// <summary>
    ///格納したタイルの削除 
    /// </summary>
    public void ClearList()
    {
        //tileの削除を行います
        for (int i = tileList.Count - 1; i > -1; i--)
        {
            var obj = tileList[i];
            Destroy(obj.gameObject);
            tileList.Remove(obj);
        }
    }

    #endregion

    #region ステートに応じての遷移

    /// <summary>
    /// 時間指定で遷移を始めます
    /// </summary>
    /// <param name="time">遷移を始める時間</param>
    public void TimeST(float time)
    {
        Invoke(nameof(SceneTransition), time);
    }

    /// <summary>
    /// 遷移を状態に応じて変えます
    /// </summary>
    void SceneTransition()
    {
        switch (state)
        {
            //ステージ選択(ボタン選択で呼ばれます)
            case SceneState.stageSelect:
                //幕の遷移
                SelectMode();

                break;

            //stageSlect遷移後,オートで遷移します
            case SceneState.selectEnd:
                //幕が開く遷移
                SelectEnd();

                break;

            //クリック可能モード中にクリックしたら時間差(別の場所で処理)で呼ばれます
            case SceneState.touch:
                //左右から猫の手
                Touch();

                break;

            //touch遷移後,オートで遷移します
            case SceneState.touchEnd:
                //幕が開く遷移---->その後gameModeStateへ
                TouchEnd();
                break;

            //プレイヤーの方でステートは変更します
            case SceneState.gameOverMode:
                //幕が閉じる----->その後reStartへ
                GameOver();
                break;

            //最初のステートに戻します
            case SceneState.reState:
                //幕が開く---->その後stageSelectへ
                ReStart();
                break;
        }
    }

    /// <summary>
    /// 状態stageSelect時の処理
    /// ステージが選択されたときにボタンに処理を入れます
    /// </summary>
    void SelectMode()
    {
        //初期化としてリストの中をクリアします
        Pattern = 0;//幕が閉じる
        SetTile(Pattern);

        //遷移処理
        //終了後selectEndへ変更
        ChangeState(SceneState.selectEnd);
    }

    /// <summary>
    /// 状態selectEnd時の処理
    /// </summary>
    void SelectEnd()
    {
        //Scene遷移
        const string stage = "Stage";
        ChangeScene(stage);

        Pattern = 1;
        SetTile(Pattern); //-->開ける遷移開始

        ChangeState(SceneState.touch);
    }

    /// <summary>
    /// 状態touch時の処理
    /// クリックして処理が一通り終えたらこの処理を呼びます
    /// </summary>
    void Touch()  //クリック可能モードでクリックしたら処理
    {
        Pattern = 2;//--->クリック後遷移開始(左右から猫の手)
        SetTile(Pattern);

        //終了後touchEnd
        ChangeState(SceneState.touchEnd);
        TouchClick = false;
    }

    /// <summary>
    /// 状態TouchEnd時の処理
    /// </summary>
    void TouchEnd()  //ゲーム画面に移行
    {
        Pattern = 1;//-->開ける遷移開始
        SetTile(Pattern);

        ChangeState(SceneState.gameMode);
    }

    /// <summary>
    /// 状態GameOverModeの処理 ゲームクリア時に処理
    /// </summary>
    void GameOver()
    {
        //tileの作成
        Pattern = 0;
        SetSizeWHCount(Pattern);//幕が閉じる

        ChangeState(SceneState.reState);
    }

    /// <summary>
    /// 状態ReStartの処理 //StateSelectへ戻る時の処理
    /// </summary>
    void ReStart()
    {
        Pattern = 1; //-->開ける遷移開始
        SetTile(Pattern);

        ChangeState(SceneState.stageSelect);
    }

    /// <summary>
    /// タイルの遷移が終了したら呼ぶ関数
    /// </summary>
    /// <returns></returns>
    public void FinishScene()
    {
        //tileのフラグ完了数
        var result = 0;

        for (int i = 0; i < tileList.Count; i++)
        {
            if (!tileList[i].Flag) result++;
        }

        //全てのtileが遷移完了しオートで処理するステートを呼びます
        if (result == tileList.Count)
        {
            SetState();
        }
    }

    /// <summary>
    /// ステートに応じて処理をします
    /// </summary>
    void SetState()
    {
        switch (state)
        {
            //ステージをクリアした際に呼ばれます
            case SceneState.stageSelect:
                //やりこみ要素でクリアしたものがあれば処理開始 ----> 別のスクリプトから呼び出します
                //終わり次第ステート変更します

                break;

            case SceneState.selectEnd:
                //オートで遷移を呼びます
                TimeST(tLag);
                break;

            case SceneState.touch:
                //クリック可能フラグを立てます
                TouchClick = true;
                break;

            case SceneState.touchEnd:
                //Stageフロアを隠していたUIを非表示にします
                var hideUI = GameObject.Find("StageBacCanvas");
                hideUI.SetActive(false);

                //オートで遷移を呼びます
                TimeST(tLag);
                break;

            case SceneState.gameMode:
                //プレイヤーを操作可能にします

                //初めにこのフロアの目標を画面にUIで表示します

                break;

            case SceneState.reState:
                //オートで遷移を呼びます
                TimeST(tLag);

                //Scene遷移
                const string stage = "StageSelect";
                ChangeScene(stage);
                break;
        }
    }

    /// <summary>
    /// tileを貼ります
    /// </summary>
    /// <param name="pattern">貼り方</param>
    void SetTile(int pattern)
    {
        //タイル削除
        ClearList();
        SetSizeWHCount(pattern);
    }

    /// <summary>
    /// Scene変更します
    /// </summary>
    void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// ステージクリア時に呼びます
    /// </summary>
    public void ChangeState(SceneState nextState)
    {
        state = nextState;
    }

    #endregion
}

using UnityEngine;

/// <summary>
/// 遷移のアニメーションをするクラス
/// </summary>
public class FeedInOut : MonoBehaviour
{
    public bool Flag { set; get; } = false;

    //大きいサイズへ
    RectTransform rt;

    ///最大サイズ・位置・拡大とそれぞれの速度///
    float goalRectX;
    float speedX;
    float goalRectY;
    float speedY;

    //どの方向へ移動するか
    string direction;

    /// <summary>
    /// 1フレームで進む速度
    /// </summary>
    float framePos;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        framePos = Time.deltaTime * speedX;
        ChildPos();

        //test
        Flag = true;
    }

    private void Update()
    {
        UpDown(SceneState.selectEnd);
    }

    #region ステートに応じての遷移

    /// <summary>
    /// 遷移を状態に応じて変えます
    /// </summary>
    void SceneTransition()
    {
        switch (UITest.Instant.State)
        {
            //ステージ選択(Flagがtrueになった時に処理)
            case SceneState.stageSelect:
                
                SelectMode();
                
                break;

            //遷移終了
            case SceneState.selectEnd:
                
                SelectEnd();
                
                break;

            //クリック可能モード中
            case SceneState.touch:
                
                Touch();
                
                break;

            //遷移終了
            case SceneState.touchEnd:
                //遷移開始(幕が開ける)
                //終了後GameModeに変更
                break;

            //プレイヤーの方でステートは変更します
            case SceneState.gameOverMode:

                break;
        }
    }

    /// <summary>
    /// 状態stageSelect時の処理
    /// </summary>
    void SelectMode()
    {
        if (Input.GetMouseButtonDown(0) && !Flag)
        {
            Flag = true;
            //tileの作成
            UITest.Instant.SetSizeWHCount(0);
        }
        //遷移処理
        //終了後selectEndへ変更
        UpDown(SceneState.selectEnd);
    }

    /// <summary>
    /// 状態selectEnd時の処理
    /// </summary>
    void SelectEnd()
    {
        if (Input.GetMouseButtonDown(0) && !Flag) Flag = true;
        //数秒後Scene変更

        //開ける遷移開始
        ReturnUpDown(SceneState.touch);
        //終了後クリック可能操作
        //->touchへ
        if (UITest.Instant.State == SceneState.touch)
        {
            //タイルの削除
            UITest.Instant.ClearList();
            //クリック後遷移開始(左右から猫の手)
            UITest.Instant.SetSizeWHCount(1);
        }
    }

    /// <summary>
    /// 状態touch時の処理
    /// </summary>
    void Touch()
    {
        RightLeft(SceneState.touchEnd);
        //終了後touchEnd
        if (UITest.Instant.State == SceneState.touch)
        {
            //ゲーム画面に移行

            //新しいタイルを貼りなおす

            //猫の手の遷移をすべて消す
            UITest.Instant.ClearList();
        }
    }

    #endregion

    #region 遷移中のアニメーションのための関数

    /// <summary>
    /// 子要素の位置を指定します
    /// </summary>
    void ChildPos()
    {
        //子のサイズ
        var child = transform.GetChild(0).GetComponent<RectTransform>();
        child.sizeDelta = new Vector2(190, 190);
        //間隔
        var marine = 22;

        //位置を指定します
        var childPos = child.anchoredPosition;
        switch (direction)
        {
            case "RIGHT":
                childPos.x = rt.sizeDelta.x / 2 + child.sizeDelta.x / 2 - marine;
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - child.sizeDelta.x, rt.anchoredPosition.y);
                break;

            case "LEFT":
                childPos.x = -rt.sizeDelta.x / 2 + -child.sizeDelta.x / 2 + marine;
                var l = child.localRotation;
                l.z = 180;
                child.localRotation = l;
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + child.sizeDelta.x, rt.anchoredPosition.y);
                break;
        }
        child.anchoredPosition = childPos;
    }

    /// <summary>
    /// 目標位置と速度とキーを取得
    /// </summary>
    public void SetPosKey(float goalRt,float speed,string direct)
    {
        goalRectY = goalRt;
        speedY = speed;
        direction = direct;
    }

    /// <summary>
    /// 幕のようなアニメーション
    /// </summary>
    void UpDown(SceneState state)
    {
        var pos = rt.anchoredPosition;
        switch (direction)
        {
            case "DOWN":
                if (rt.anchoredPosition.y >= goalRectY)
                    pos.y += Time.deltaTime * speedY;
                else
                    Flag = false;
                
                break;

            case "UP":
                if (rt.anchoredPosition.y <= goalRectY)
                    pos.y += Time.deltaTime * speedY;
                else
                    Flag = false;

                break;
        }

        if (!Flag)
        {
            pos.y = goalRectY;
            UITest.Instant.StateChange(state);
        }

        rt.anchoredPosition = pos;
    }

    /// <summary>
    /// 幕が上がる
    /// </summary>
    void ReturnUpDown(SceneState state)
    {
        var pos = rt.anchoredPosition;
        switch (direction)
        {
            case "DOWN":
                if (rt.anchoredPosition.y <= -goalRectY)
                    pos.y -= Time.deltaTime * speedY;
                else
                    Flag = false;

                break;

            case "UP":
                if (rt.anchoredPosition.y >= -goalRectY)
                    pos.y -= Time.deltaTime * speedY;
                else
                    Flag = false;

                break;
        }

        if (!Flag)
        {
            pos.y = -goalRectY;
            UITest.Instant.StateChange(state);
        }
        rt.anchoredPosition = pos;
    }


    /// <summary>
    /// 目的位置とX軸の移動速度を取得
    /// </summary>
    /// <param name="goalRt"></param>
    /// <param name="speed"></param>
    public void SetSpeed(float goalRt,float speed,string direct)
    {
        goalRectX = goalRt;
        speedX = speed;
        direction = direct;
    }

    /// <summary>
    /// 右から左へ、左から右へ
    /// </summary>
    void RightLeft(SceneState state)
    {
        var pos = rt.anchoredPosition;
        switch (direction)
        {
            case "RIGHT":
                //1フレームで進む値をgoalRectに代入することでピッタリにする
                if (rt.anchoredPosition.x <= goalRectX - framePos)
                    pos.x += Time.deltaTime * speedX;
                else Flag = false;
                break;

            case "LEFT":
                if (rt.anchoredPosition.x >= goalRectX - framePos)
                    pos.x += Time.deltaTime * speedX;
                else
                    Flag = false;
                break;
        }

        if (!Flag) UITest.Instant.StateChange(state);

        rt.anchoredPosition = pos;
    }

    #endregion
}

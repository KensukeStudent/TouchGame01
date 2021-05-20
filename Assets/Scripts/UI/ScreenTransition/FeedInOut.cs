using UnityEngine;

/// <summary>
/// 遷移のアニメーションをするクラス
/// </summary>
public class FeedInOut : MonoBehaviour
{
    public bool Flag { set; get; } = true;

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

    /// <summary>
    /// 遷移の関数を格納
    /// </summary>
    delegate void SceneTransiton();
    SceneTransiton[] sts;
    SceneTransiton st;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        framePos = Time.deltaTime * speedX;
        ChildPos();

        sts = new SceneTransiton[]
        {
            UpDown,
            ReturnUpDown,
            RightLeft
        };

        //遷移の種類の番号
        var no = ScreenTransition.Instance.Pattern;

        //Patternの遷移動作を入れます
        st = sts[no];

        //番号が0,1なら猫の手を非表示にします
        if(no != 2)
        {
            var child = transform.GetChild(0).gameObject;
            child.SetActive(false);
        }

        rt.localScale = new Vector3(1, 1, 1);
    }

    private void Update()
    {
        //Patternの遷移動作をさせます
        if(Flag) st();
    }

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
                
                //canvasXサイズとこの画像の半分の大きさ分,右にずらします(ちょうど画面端)
                childPos.x = rt.sizeDelta.x / 2 + child.sizeDelta.x / 2 - marine;
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - child.sizeDelta.x, rt.anchoredPosition.y);
                
                break;

            case "LEFT":
                
                //canvasXサイズとこの画像の半分の大きさ分,左にずらします(ちょうど画面端)
                childPos.x = -rt.sizeDelta.x / 2 + -child.sizeDelta.x / 2 + marine;
                
                //ネコの手が右向きになるように回転させます
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
        //目的位置
        goalRectY = goalRt;
        //目的位置までの速度
        speedY = speed;
        //方向
        direction = direct;
    }

    /// <summary>
    /// 幕のような上下から閉じるアニメーション
    /// </summary>
    void UpDown()
    {
        var pos = rt.anchoredPosition;
        switch (direction)
        {
            case "DOWN":
                //下向きに移動
                if (rt.anchoredPosition.y >= goalRectY)
                    pos.y += Time.unscaledDeltaTime * speedY; // TimeScale０の時に処理することがあるためunsclaedDeltaTimeで取る
                else
                    Flag = false;
                
                break;

            case "UP":
                //上向きに移動
                if (rt.anchoredPosition.y <= goalRectY)
                    pos.y += Time.unscaledDeltaTime * speedY;
                else
                    Flag = false;

                break;
        }

        if (!Flag)
        {
            pos.y = goalRectY;
            //全てのtileが終了していれば、state変更フラグを立てます
            ScreenTransition.Instance.FinishScene();
        }

        rt.anchoredPosition = pos;
    }

    /// <summary>
    /// 幕が上がる
    /// </summary>
    void ReturnUpDown()
    {
        var pos = rt.anchoredPosition;
        switch (direction)
        {
            case "DOWN":
                if (rt.anchoredPosition.y <= goalRectY)
                    pos.y += Time.deltaTime * speedY;
                else
                    Flag = false;
                
                break;

            case "UP":
                if (rt.anchoredPosition.y >= goalRectY)
                    pos.y += Time.deltaTime * speedY;
                else
                    Flag = false;

                break;
        }

        if (!Flag)
        {
            pos.y = goalRectY;
            //全てのtileが終了していれば、state変更フラグを立てます
            ScreenTransition.Instance.FinishScene();
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
    void RightLeft()
    {
        var pos = rt.anchoredPosition;

        switch (direction)
        {
            case "RIGHT":
                //1フレームで進む値をgoalRectに代入することでピッタリにする
                if (rt.anchoredPosition.x <= goalRectX)
                    pos.x += Time.deltaTime * speedX;
                else
                {
                    Flag = false;
                    pos.x = goalRectX;
                }
                break;

            case "LEFT":
                if (rt.anchoredPosition.x >= goalRectX)
                    pos.x += Time.deltaTime * speedX;
                else
                {
                    Flag = false;
                    pos.x = goalRectX;
                }
                    
                break;
        }

        if (!Flag)
        {
            //全てのtileが終了していれば、state変更フラグを立てます
            ScreenTransition.Instance.FinishScene();
        }

        rt.anchoredPosition = pos;
    }

    #endregion
}

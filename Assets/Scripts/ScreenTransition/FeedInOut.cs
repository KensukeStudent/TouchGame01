﻿using UnityEngine;

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

        //Patternの遷移動作を入れます
        st = sts[ScreenTransition.Instance.Pattern];
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
    void UpDown()
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

        if (!Flag)
        {
            //全てのtileが終了していれば、state変更フラグを立てます
            ScreenTransition.Instance.FinishScene();
        }

        rt.anchoredPosition = pos;
    }

    #endregion
}
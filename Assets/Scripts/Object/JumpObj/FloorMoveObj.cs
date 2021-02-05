using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子オブジェクトにプレイヤーがいる時にカメラを指定方向に移動させるクラス
/// </summary>
public class FloorMoveObj : MonoBehaviour
{
    /// 左がRかU 右がLかD

    /// <summary>
    /// カメラのMax位置に代入
    /// </summary>
    Vector2[] max = { new Vector2(18, 0), new Vector2(0, 0) };
    /// <summary>
    /// カメラのMin位置に代入
    /// </summary>
    Vector2[] min = { new Vector2(18, 0), new Vector2(0, 0) };
    #region 例
    ///// <summary>
    ///// カメラのMax位置に代入
    ///// </summary>
    //Vector2[] max = { new Vector2(18, 0), new Vector2(0, 0) };
    ///// <summary>
    ///// カメラのMin位置に代入
    ///// </summary>
    //Vector2[] min = { new Vector2(18, 0), new Vector2(0, 0) };
    ///// <summary>
    ///// カメラのMax位置に代入
    ///// </summary>
    //Vector2[] max = { new Vector2(0, 0), new Vector2(0, -10) };
    ///// <summary>
    ///// カメラのMin位置に代入
    ///// </summary>
    //Vector2[] min = { new Vector2(0, 0), new Vector2(0, -10) };
    #endregion

    /// <summary>
    /// カメラが移動する方向(左右か上下)指定
    /// </summary>
    string direction = "RL";

    /// <summary>
    /// カメラに値を代入
    /// </summary>
    public void CamSetVec()
    {
        var cam = GameObject.Find("Main Camera").GetComponent<CameraContorller>();
        SetDirectVec(cam);
    }

    /// <summary>
    /// 移動方向を判別
    /// </summary>
    void SetDirectVec(CameraContorller cam)
    {
        //向き
        var dire = "";
        //max,minの配列番号
        var num = 0;

        switch (direction)
        {
            //左右
            case "RL":
                //右
                if (cam.MinC.x < min[0].x) dire = "R";
                else if(cam.MinC.x > min[1].x) dire = "L";
                break;

        　　//上下
            case "UD":
                if (cam.MinC.y < min[0].y) dire = "U";
                else if (cam.MinC.y > min[1].y) dire = "D";
                break;
        }

        //方向が左か下の時にnum = 1に変更
        switch (dire)
        {
            case "L":
            case "D":
                num = 1;
                break;
        }

        SetFloorObj(dire);
        cam.SetDirectVec(dire, max[num], min[num]);
    }

    /// <summary>
    /// プレイヤーがフロアの移動時にフロアのセッティングします
    /// </summary>
    void SetFloorObj(string dire)
    {
        var floor = GameObject.Find("FloorManager").GetComponent<FloorManager>();
        var c = GameObject.Find("StageCreator").GetComponent<StageCreator>();

        var nowFloor = int.Parse(transform.root.name);
    
        //位置を求めます
        var stageH = nowFloor / c.FloorCount;
        //移動時どのステージフロアにいるかを求める
        switch (dire)
        {
            case "R":
                floor.Floors[stageH, nowFloor + 1].ActiveFloor();//右のフロアを表示
                floor.Floors[stageH, nowFloor].InActiveFloor();//左のフロアを非表示(今いたフロア)
                break;

            case "U":
                floor.Floors[stageH + 1, nowFloor].ActiveFloor();//上のフロアを表示
                floor.Floors[stageH, nowFloor].InActiveFloor();//下のフロアを非表示(今いたフロア)
                break;

            case "L"://左か下
                floor.Floors[stageH, nowFloor].ActiveFloor();//左のフロアを表示
                floor.Floors[stageH, nowFloor].InActiveFloor();//右のフロアを非表示(今いたフロア)
                break;

            case "D":
                floor.Floors[stageH - 1, nowFloor].ActiveFloor();//下のフロアを表示
                floor.Floors[stageH, nowFloor].InActiveFloor();//上のフロアを非表示(今いたフロア)
                break;
        }
    }

    /// <summary>
    /// このオブジェクトの最大最小を代入
    /// </summary>
    public void SetThisVec(float[] x, float[] y)
    {
        for (int i = 0; i < 2; i++)
        {
            max[i].x = x[i];
            max[i].y = y[i];
        }
    }

    /// <summary>
    /// 最大最小を指定
    /// </summary>
    /// <param name="xMax"></param>
    /// <param name="yMax"></param>
    /// <param name="xMin"></param>
    /// <param name="yMin"></param>
    /// <param name="direct"></param>
    public void SetMaxMinInit(float[] xMax, float[] yMax, float[] xMin, float[] yMin,string direct)
    {
        for (int i = 0; i < 2; i++)
        {
            max[i].x = xMax[i];
            max[i].y = yMax[i];

            min[i].x = xMin[i];
            min[i].y = yMin[i];
        }
        direction = direct;
    }
}

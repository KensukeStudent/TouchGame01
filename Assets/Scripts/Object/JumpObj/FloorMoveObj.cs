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
        //指定の方向に移動します
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

        //フロアのオブジェクトの表示非表示をセットします
        SetFloorObj(dire);
        //カメラが動ける最小、最大をセットします
        cam.SetDirectVec(dire, max[num], min[num]);
    }

    /// <summary>
    /// プレイヤーがフロアの移動時にフロアのセッティングします
    /// </summary>
    void SetFloorObj(string dire)
    {
        var floor = GameObject.Find("FloorManager").GetComponent<FloorManager>();
        var stageX = StageCreator.StageX;

        //現在のフロア(プレイヤーのいるフロア)を取得します
        var nowFloor = floor.PlayerFloor;
    
        //縦の位置を求めます
        //   現在のフロア番号  / 横のフロア数
        var stageH = nowFloor / stageX;

        //横の位置を求めます
        //   現在のフロア番号 % 横のフロア数
        var stageW = nowFloor % stageX;

        //移動時どのステージフロアにいるかを求める
        switch (dire)
        {
            case "R":
                floor.Floors[stageH, stageW + 1].ActiveFloor();//右のフロアを表示

                floor.Floors[stageH, stageW].InActiveFloor();//左のフロアを非表示(今いたフロア)
                break;

            case "U":
                floor.Floors[stageH - 1, stageW].ActiveFloor();//上のフロアを表示

                floor.Floors[stageH, stageW].InActiveFloor();//下のフロアを非表示(今いたフロア)
                break;

            case "L"://左か下
                floor.Floors[stageH, stageW - 1].ActiveFloor();//左のフロアを表示
                
                floor.Floors[stageH, stageW].InActiveFloor();//右のフロアを非表示(今いたフロア)
                break;

            case "D":
                floor.Floors[stageH + 1, stageW].ActiveFloor();//下のフロアを表示
                
                floor.Floors[stageH, stageW].InActiveFloor();//上のフロアを非表示(今いたフロア)
                break;
        }

        //移動先のフロアを計算します
        var fn = GetFloorNo(dire, stageH, stageW, stageX);
        //プレイヤーの現在地点フロア番号をいれます
        floor.SetPlayerFloor(fn.ToString());
    }

    /// <summary>
    /// 移動先のフロア番号を取得します
    /// </summary>
    int GetFloorNo(string dire,int stageH,int nowFloor, int stageX)
    {
        int no = 0;

        #region 計算式
        //例 = ((現在の縦の番号 +(縦の移動先の符号) 1 ) * 横のフロア数 ) + (現在のフロア番号 +(横の移動先の符号) 1) 
        //例 = (stageH + 1) * c.StageX + nowFloor + 1;
        #endregion

        switch (dire)
        {
            //右に移動
            case "R":
                no = stageH * stageX + nowFloor + 1;
                break;

            //上に移動
            case "U":
                no = (stageH - 1) * stageX + nowFloor;
                break;

            //左に移動
            case "L":
                no = stageH * stageX + nowFloor - 1;
                break;

            //下に移動
            case "D":
                no = (stageH + 1) * stageX + nowFloor;
                break;
        }
        return no;
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

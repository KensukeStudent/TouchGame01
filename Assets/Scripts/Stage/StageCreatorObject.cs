using System;
using System.IO;
using UnityEngine;

/// <summary>
/// ステージ内のオブジェクトを作成するコードを記述
/// </summary>
public partial class StageCreator : MonoBehaviour
{
    #region マップのオブジェクト生成

    /// <summary>
    /// 壁
    /// </summary>
    void SwicthWALL(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        //ステージごとの壁を生成します
        string[][] wallNo = new string[2][]
        {
            new string[]{"22", "23", "24", "28", "29", "278", "279", "280", "284", "285", "534", "535", "536"},

            new string[]{"1031","1032","1033","1034","1286","1287","1288","1289","1542","1543","1544","1545"}
        };

        //読み込んだ文字数値がwallNo配列にあるかを解析し、配列番号のオブジェクトを
        //現在のフロアに生成します。
        if (!OnMozi(wallNo[fileNum], tileNum, 0))
        {
            //wc[ステージ番号][wallNoの番号]のオブジェクトを生成
            InstantObj(wc[fileNum].walls[SetNumber(wallNo[fileNum], tileNum)], pos, floor);
        }
    }

    /// <summary>
    /// 　背景
    /// </summary>
    void SwitchBack(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        //ステージごとの背景を生成します
        string[][] backNo = new string[2][]
        {
            new string[]{"0","768", "769", "770","1024", "1025", "1026",
                          "1280", "1281", "1282","1536", "1537","1792","1793"},

            new string[]{"0"},
        };

        //読み込んだ文字数値がbackNo配列にあるかを解析し、配列番号のオブジェクトを
        //現在のフロアに生成します。
        if (!OnMozi(backNo[fileNum], tileNum, 0))
        {
            //bc[ステージ番号][backNoの番号]のオブジェクトを生成
            InstantObj(bc[fileNum].backs[SetNumber(backNo[fileNum], tileNum)], pos, floor);
        }
    }

    /// <summary>
    /// デコレーション
    /// </summary>
    void SwitchDecoration(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        string[][] decoNo = new string[2][]
        {
            new string[]{"293", "294", "799", "800", "1055", "1056",
                          "5146", "5147", "5148", "5149", "5402" },

            new string[]{"5642","5898","6154","6410","6926","6927",
                       "6928","6929","7182","7183","7184","7185" }
        };

        //読み込んだ文字数値がdecoNo配列にあるかを解析し、配列番号のオブジェクトを
        //現在のフロアに生成します。
        if (!OnMozi(decoNo[fileNum], tileNum, 0))
        {
            //dc[ステージ番号][decoNoの番号]のオブジェクトを生成
            InstantObj(dc[fileNum].decos[SetNumber(decoNo[fileNum], tileNum)], pos, floor);
        }
    }

    /// <summary>
    /// プレイヤーと敵とゴール
    /// </summary>
    void SwitchPlayerGoal(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        switch (tileNum)
        {
            //プレイヤー
            case "0":

                //オブジェクトの生成
                InstantObj(initPos, pos, floor);
                InstantObj(player, pos, floor);

                var fm = GameObject.Find("FloorManager").GetComponent<FloorManager>();
                fm.SetPlayerFloor(floor.name);
                break;

            //ゴール
            case "1027":
                //オブジェクトの生成
                InstantObj(goal, pos, floor);
                break;
        }
    }

    /// <summary>
    /// ジャンプ
    /// </summary>
    /// <param name="tileNum"></param>
    /// <param name="pos"></param>
    void SwitchJump(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        string[] jObjNo = { "769", "2", "1282" };

        #region ジャンプ詳細
        //case : 769 回転
        //case :  2  一部
        //case : 1282 一度のみアイテム付きジャンプ台
        #endregion

        //読み込んだ文字数値がjObjNo配列にあるかを解析し、配列番号のオブジェクトを
        //現在のフロアに生成します。
        if (!OnMozi(jObjNo, tileNum, 0))
        {
            InstantObj(jumpObj[SetNumber(jObjNo, tileNum)], pos, floor);
        }
    }

    /// <summary>
    /// イベント関連
    /// </summary>
    void SwitchEventItem(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        #region 詳細
        
        //青鍵
        //case "0":
        //緑鍵
        //case "1":;
        //赤鍵
        //case "2":
        //黄鍵
        //case "3":
        //壺(空)
        //case "4":
        //壺(鍵)
        //case "5":
        //バクダンイベント
        //case "6"
        //バクダン
        //case "7"

        #endregion

        string[] itemNo = { "0", "1", "2", "3", "4", "5", "6", "7" };

        //読み込んだ文字数値がitemNo配列にあるかを解析し、配列番号のオブジェクトを
        //現在のフロアに生成します。
        if (!OnMozi(itemNo, tileNum, 0))
        {
            InstantObj(item[SetNumber(itemNo, tileNum)], pos, floor);
        }
    }

    #endregion

    #region Json読み込み生成

    /// <summary>
    /// 敵
    /// </summary>
    void SwitchEnemy(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        switch (tileNum)
        {
            //どくろShot
            case "0":
                var e0 = InstantObj(enemy[0], pos, floor);
                var d = e0.GetComponent<DokuroShot>();
                //情報を入れます
                ji.SetDokuro(stageID, d, counter, floorNum);
                counter++;
                break;
            //どくろMove
            case "1":

                break;

            //大砲
            case "2":
                //大砲を生成します
                var e2 = InstantObj(enemy[2], pos, floor);
                var c = e2.GetComponent<Cannon>();
                //情報を入れます
                ji.SetCannon(stageID, c, counter, floorNum);
                counter++;
                break;
        }
    }

    /// <summary>
    /// イベント関連
    /// </summary>
    void SwitchEventBlock(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        //ブロック名
        string[] nameArray = { "ev", "goal" };
        var blockName = "";
        GameObject b = null;

        #region 詳細

        //-----鍵に関するブロック-----//

        //    //青ブロック
        //    case "0":
        //    //緑ブロック
        //    case "1":
        //    //赤ブロック
        //    case "2":
        //    //黄ブロック
        //    case "3":

        //----------破壊ブロック----------//
               //破壊ブロック
        //    case "4":

        //    //-----ゴールに関するブロック-----//

        //    //敵検知ブロック(指定の敵を倒さないと壊れない)
        //    case "5":
        //    //鍵解除ブロック
        //    case "6":

        #endregion

        string[] bNo = { "0", "1", "2", "3", "4", "5", "6"};

        //配列に文字がなければreturnします
        if (OnMozi(bNo, tileNum, 0)) return;
        //---0～3が鍵ブロック---
        //ブロック名をイベントにします
        else if (OnMozi(bNo, tileNum, 5))
        {
            blockName = nameArray[0];
        }
        //---4～5がゴールブロック---
        //ブロック名をゴールにします
        else
        {
            blockName = nameArray[1];
        }


        //ブロックを作成します
        b = InstantObj(block[SetNumber(bNo, tileNum)], pos, floor);
        var bs = b.GetComponent<BlocksScript>();

        //ブロックに説明を入れます
        ji.SetBlock(stageID, blockName, bs, counter, floorNum);

        //生成カウントを増やします
        counter++;

        //ブロックがゴールブロックの場合
        if (blockName == nameArray[0]) SwitchGoalKind(b, tileNum);
    }

    /// <summary>
    /// ブロックがゴールブロックだった場合
    /// </summary>
    /// <param name="b"></param>
    void SwitchGoalKind(GameObject b, string num)
    {
        var g = b.GetComponent<GoalBlock>();

        switch (num)
        {
            case "4":
                //敵(全ての敵を倒す条件を指定)
                g.GoalKind(goalKind.enemy);
                break;

            case "5":
                //鍵(指定の鍵を入手)
                g.GoalKind(goalKind.key);
                break;
        }
    }

    /// <summary>
    /// ステージ内の面移動
    /// </summary>
    void SwitchMove(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        #region 移動向き詳細
        //上
        //case "0" "8"
        //左
        //case "1" "9"
        //下
        //case "2" "10"
        //右
        //case "4" "11"
        #endregion

        //向きの番号
        string[] direNo = { "0", "1", "2", "3", "8", "9", "10", "11" };
        //0-3がArrow作成(フロアを移動する向きの矢印)
        //8-11がJumpを作成(フロアを移動するジャンプ台)
        string[] name = { "Arrow_", "Jump_" };

        switch (tileNum)
        {
            case "0":
            case "1":
            case "2":
            case "3":

                //猫矢印の配置
                var arrow = InstantObj(catArrow[SetNumber(direNo, tileNum)], pos, floor);

                //矢印に名前とどの向きかを入れます
                arrow.name = name[0] + Direct(direNo, tileNum);

                var hc = arrow.GetComponent<HintCat>();

                //情報を入れます
                ji.SetHint(stageID, hc, arrowCounter, floorNum);
                arrowCounter++;
                break;

            case "8":
            case "9":
            case "10":
            case "11":

                //移動オブジェクトの配置
                var jump = InstantObj(moveObj, pos, floor);

                //ジャンプ台に名前とどの向きかを入れます
                jump.name = name[1] + Direct(direNo, tileNum);
                var jumpObj = jump.GetComponent<FloorMoveObj>();

                //生成されるジャンプオブジェクトに値をセットします
                ji.SetFloorVec(stageID, jumpObj, counter, floorNum);

                counter++;
                break;
        }
    }

    #endregion
}

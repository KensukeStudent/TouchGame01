using System;
using System.IO;
using UnityEngine;
#pragma warning disable 649

/// <summary>
/// ステージ作成クラス
/// </summary>
public class StageCreator : MonoBehaviour
{
    /// <summary>
    /// イベントブロック説明をJsonから取得します
    /// </summary>
    JsonInfo ji;

    /// <summary>
    /// 生成する初期位置
    /// </summary>
    readonly Vector2 createInitPos = new Vector2(-9.0f, 5.0f);

    //----------WALL----------//
    [Header("Wall")]
    /// <summary>
    /// 壁
    /// </summary>
    [SerializeField] GameObject[] walls;

    //----------PLAYER ENEMY GOAL----------//
    [Header("Player/Goal")]
    /// <summary>
    /// プレイヤー
    /// </summary>
    [SerializeField] GameObject player;
    [SerializeField] GameObject goal;
    [Header("Enemy")]
    /// <summary>
    /// エネミー
    /// </summary>
    [SerializeField] GameObject[] enemy;

    //----------JUMP----------//
    [Header("Jump")]
    /// <summary>
    /// 初期位置
    /// </summary>
    [SerializeField] GameObject initPos;

    /// <summary>
    /// ジャンプ台
    /// </summary>
    [SerializeField] GameObject[] jumpObj;

    //----------EventItem----------//
    [Header("EventItem")]
    [SerializeField] GameObject[] key;

    //----------EventBlock----------//
    [Header("EventBlock")]
    [SerializeField] GameObject[] block;

    //----------EventBlock----------//
    [Header("Move")]
    [SerializeField] GameObject moveObj;
    [SerializeField] GameObject[] catArrow;

    //----------BackGround----------//
    [Header("BackGround")]
    [SerializeField] GameObject[] grasses;

    //----------Decoration----------//
    [Header("Decoration")]
    [SerializeField] GameObject[] decos;

    /// <summary>
    /// 各ステージフロア作成
    /// </summary>
    /// <param name="tileNum">タイルナンバー</param>
    /// <param name="pos">生成位置</param>
    /// <param name="floor">生成元の親オブジェクト</param>
    /// <param name="floorNum">現在のフロア数</param>
    delegate void TileMap(string tileNum, Vector2 pos, GameObject floor,int floorNum);
    TileMap[] tileList;
    TileMap tile;
    readonly string[] pathName = { "Wall", "PlayerGoal", "Enemy", "Jump", "Move", "EventItem", "EventBlock", "BackGround", "Decoration" };
    /// <summary>
    /// イベントボックス生成カウント数
    /// </summary>
    int counter = 0;

    /// <summary>
    /// ステージ番号
    /// ステージ選択時にセットします
    /// </summary>
    int fileNum = 0;
    /// <summary>
    /// ステージ内のフロア数
    /// </summary>
    readonly int[] floorCount = { 3 };
    /// <summary>
    /// 現在のステージのフロア数を返します
    /// </summary>
    public int FloorCount
    {
        get
        {
            return floorCount[fileNum];
        }
    }

    /// <summary>
    /// 各ステージのX,Y長さ
    /// </summary>
    readonly Vector2[] stageVec = { new Vector2(19 - 1, 11 + 1) };

    /// <summary>
    /// 横のステージのフロア数
    /// </summary>
    readonly int[] stageX = { 3 };
    public int StageX
    {
        get
        {
            return stageX[fileNum];
        }
    }

    private void Awake()
    {
        //ステージ番号のステージを作成します
        fileNum = GameManager.Instance.StageNo;

        ji = new JsonInfo();

        tileList = new TileMap[]
        {
            SwicthWALL,//壁を生成
            SwitchPlayerGoal,//プレイヤーとゴールを生成
            SwitchEnemy,//敵を生成
            SwitchJump,//ジャンプ台を生成
            SwitchMove,//フロア移動
            SwitchEventItem,//イベントアイテムを生成
            SwitchEventBlock,//イベントブロックを生成
            SwitchBack,//背景生成
            SwitchDecoration//デコレーション生成
        };

        //ステージ作成
        CreateStage();
    }

    /// <summary>
    /// ステージの作成
    /// </summary>
    void CreateStage()
    {
        //CSVからデータを読み込む
        for (int sNum = 0; sNum < FloorCount; sNum++)
        {
            //余りが横
            float stageVecX = sNum % StageX;
            //商が縦
            float stageVecY = sNum / StageX;

            var vec = Vector2.zero;
            //このステージの横縦長さ * ステージのx(横のフロア)とy(縦のフロア)の何番目か + 作成する初期位置
            vec.x = stageVec[fileNum].x * stageVecX + createInitPos.x;
            vec.y = (-stageVec[fileNum].y) * stageVecY + createInitPos.y;
            

            //ステージを格納する空のオブジェクトを作成(フロアごとに分けてここに入れます)
            var obj = Resources.Load<GameObject>("StageEmpty");
            var floor = Instantiate(obj, transform.position, Quaternion.identity);
            floor.name = sNum.ToString();

            for (int i = 0; i < tileList.Length; i++)
            {
                var stageNo = fileNum + 1;
                var filePath = string.Format("Stage{0}/{1}/{2}", stageNo, stageNo + sNum, pathName[i]);
                CSVRead(filePath, i, floor, vec, sNum);
            }
        }
    }

    /// <summary>
    /// CSVファイルの読み込み
    /// </summary>
    void CSVRead(string stageTagName, int num, GameObject floor,Vector2 createPos,int floorNum)
    {
        //csvファイルの検索
        var csvFile = Resources.Load(stageTagName) as TextAsset;
        //csvファイルの読み込み
        var reader = new StringReader(csvFile.text);

        //数字文字を解析しステージを作成します
        StringMozi(reader, num, floor, createPos, floorNum);
    }

    /// <summary>
    /// ファイルの内の","で文字を区切ります
    /// </summary>
    void StringMozi(StringReader reader, int num, GameObject floor,Vector2 createPos, int floorNum)
    {
        var pos = createPos;
        tile = tileList[num];
        //読み取り対象がなくなるまで繰り返します
        while (reader.Peek() != -1)
        {
            //一行づつ読み込みます
            string line = reader.ReadLine();

            string[] split = line.Split(',');//,で区切ります
            foreach (var tileNum in split)
            {
                //現在の作成物
                tile(tileNum, pos, floor, floorNum);
                //生成位置を変更します
                pos.x += 1;
            }
            //xを初期位置yを-1下げたところから生成を再開しします
            pos.x = createPos.x;
            pos.y -= 1;
        }
        counter = 0;
    }

    /// <summary>
    /// 壁
    /// </summary>
    void SwicthWALL(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        string[] wallNo = { "22", "23", "24", "280", "536", "535", "534", "278", "28", "29", "279", "284" ,"285"};
        //読み込んだ文字数値がwallNo配列にあるかを解析し、配列番号のオブジェクトを
        //現在のフロアに生成します。
        if (!OnMozi(wallNo, tileNum, 0)) InstantObj(walls[SetNumber(wallNo, tileNum)], pos, floor);
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
    /// 敵
    /// </summary>
    /// <param name="tileNum"></param>
    /// <param name="pos"></param>
    /// <param name="floor"></param>
    /// <param name="floorNum"></param>
    void SwitchEnemy(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        switch (tileNum)
        {
            //どくろShot
            case "0":
                var e = InstantObj(enemy[0], pos, floor);
                var d = e.GetComponent<DokuroShot>();
                ji.SetDokuro("stage1", d, floorNum, counter);
                counter++;
                break;
            //どくろMove
            case "1":

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
        string[] jObjNo = { "769", "2" };

        #region ジャンプ詳細
        //case : 769 回転
        //case :  2  一部
        #endregion

        if (!OnMozi(jObjNo, tileNum, 0)) InstantObj(jumpObj[SetNumber(jObjNo, tileNum)], pos, floor);
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

        //    //-----ゴールに関するブロック-----//

        //    //敵検知ブロック(指定の敵を倒さないと壊れない)
        //    case "4":
        //    //鍵解除ブロック
        //    case "5":
        #endregion

        string[] bNo = { "0", "1", "2", "3", "4", "5", };

        //配列に文字がなければreturnします
        if (OnMozi(bNo, tileNum, 0)) return;
        //---0～3が鍵ブロック---
        //ブロック名をイベントにします
        else if (OnMozi(bNo, tileNum, 4)) blockName = nameArray[0];      
        //---4～5がゴールブロック---
        //ブロック名をゴールにします
        else blockName = nameArray[1];

        //ブロックを作成します
        b = InstantObj(block[SetNumber(bNo, tileNum)], pos, floor);
        var bs = b.GetComponent<BlocksScript>();

        //ブロックに説明を入れます
        ji.SetBlock("stage1", blockName, counter, bs, floorNum);
        
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
            //敵(全ての敵を倒す条件を指定)
            case "4":
                
                g.GoalKind(goalKind.enemy);
                break;

            //鍵(指定の鍵を入手)
            case "5":
                
                g.GoalKind(goalKind.key);
                break;
        }
    }

    /// <summary>
    /// イベント関連
    /// </summary>
    void SwitchEventItem(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        #region 鍵詳細
        //青鍵
        //case "0":
        //緑鍵
        //case "1":;
        //赤鍵
        //case "2":
        //黄鍵
        //case "3":
        #endregion

        string[] kNo = { "0", "1", "2", "3", };
        if (!OnMozi(kNo, tileNum, 0)) InstantObj(key[SetNumber(kNo, tileNum)], pos, floor);
    }

    /// <summary>
    /// ステージ内の面移動
    /// </summary>
    void SwitchMove(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        #region 移動向き詳細
        //上
        //case "0" "10"
        //左
        //case "1" "11"
        //下
        //case "2" "12"
        //右
        //case "4" "13"
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
                ji.SetFloorVec("stage1", jumpObj, counter, floorNum);

                counter++;
                break;
        }
    }

    /// <summary>
    /// 　背景
    /// </summary>
    void SwitchBack(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        string[] grassNo = {"0","768", "769", "770","1024", "1025", "1026",
                          "1280", "1281", "1282","1536", "1537","1792","1793" };

        if (!OnMozi(grassNo, tileNum, 0)) InstantObj(grasses[SetNumber(grassNo, tileNum)], pos, floor);
    }

    /// <summary>
    /// デコレーション
    /// </summary>
    void SwitchDecoration(string tileNum, Vector2 pos, GameObject floor, int floorNum)
    {
        string[] decoNo = { "293", "294", "799", "800", "1055", "1056", 
                          "5146", "5147", "5148", "5149", "5402" };

        if (!OnMozi(decoNo, tileNum, 0)) InstantObj(decos[SetNumber(decoNo, tileNum)], pos, floor);
    }

    /// <summary>
    /// 方向を判別する
    /// </summary>
    /// <returns></returns>
    string Direct(string[] array,string num)
    {
        //上左下右の4つの方向を取り出すため % 4の余りを求めます
        //例え : 4番目("8") % 4 = 0番目　--> "U"(上向き)
        
        var no = SetNumber(array,num) % 4;
        string[] direction = { "U", "L", "D", "R" };
        
        return direction[no];
    }

    /// <summary>
    /// Tileナンバーを取得
    /// </summary>
    /// <param name="array">検索する配列</param>
    /// <param name="tileNum">比較する文字数字</param>
    /// <returns></returns>
    int SetNumber(string[] array, string tileNum)
    {
        //検索した文字が配列の何番目にあるかを求めます
        return _ = Array.IndexOf(array, tileNum);
    }

    /// <summary>
    /// 指定された配列の中に文字があるかを解析します
    /// 比較する数値未満ならTrueを返します
    /// </summary>
    /// <param name="array">解析する配列</param>
    /// <param name="tileNum">何の文字</param>
    /// <param name="comp">比較する値</param>
    /// <returns></returns>
    bool OnMozi(string[] array, string tileNum, int comp)
    {
        //取得した配列番号と比較する数値未満ならTrueを返します
        var ret = SetNumber(array, tileNum);
        return ret < comp;
    }

    /// <summary>
    /// オブジェクトの作成
    /// </summary>
    /// <param name="array">文字配列</param>
    /// <param name="num">文字番</param>
    /// <param name="obj">作成するオブジェクト</param>
    /// <param name="pos">生成ポジション</param>
    GameObject InstantObj(GameObject obj, Vector2 pos, GameObject floor)
    {
        //オブジェクトを現在のフロアの中に生成します
        var go = Instantiate(obj, pos, Quaternion.identity);
        //parent = フロア番号
        go.transform.SetParent(floor.transform);
        return go;
    }
}

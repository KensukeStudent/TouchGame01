using System;
using System.IO;
using UnityEngine;
#pragma warning disable 649

#region 生成タイルクラス

/// <summary>
/// 壁のタイルを格納するクラス
/// </summary>
[Serializable]
public class WallsClass
{
    public string id;
    public GameObject[] walls;
}

/// <summary>
/// 背景のタイルを格納するタイル
/// </summary>
[Serializable]
public class BackGroundClass
{
    public string id;
    public GameObject[] backs;
}

/// <summary>
/// 装飾タイルを格納するクラス
/// </summary>
[Serializable]
public class DecorationClass
{
    public string id;
    public GameObject[] decos;
}

#endregion

/// <summary>
/// ステージ作成クラス
/// </summary>
public partial class StageCreator : MonoBehaviour
{
    /// <summary>
    /// イベントブロック説明をJsonから取得します
    /// </summary>
    JsonInfo ji;

    /// <summary>
    /// 生成する初期位置
    /// </summary>
    readonly Vector2 createInitPos = new Vector2(-8.5f, 5.5f);

    #region 生成するオブジェクト

    #region ステージごとに変わるタイル

    //----------WALL----------//
    [Header("Wall")]
    /// <summary>
    /// 壁
    /// </summary>
    [SerializeField] WallsClass[] wc;

    //----------BackGround----------//
    [Header("BackGround")]
    /// <summary>
    /// 背景
    /// </summary>
    [SerializeField] BackGroundClass[] bc;

    //----------Decoration----------//
    [Header("Decoration")]
    /// <summary>
    /// 装飾
    /// </summary>
    [SerializeField] DecorationClass[] dc;

    #endregion

    #region 全ステージ共通のタイル

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
    [SerializeField] GameObject[] item;

    //----------EventBlock----------//
    [Header("EventBlock")]
    [SerializeField] GameObject[] block;

    //----------EventBlock----------//
    [Header("Move")]
    [SerializeField] GameObject moveObj;
    [SerializeField] GameObject[] catArrow;

    #endregion

    #endregion

    /// <summary>
    /// 各ステージフロア作成
    /// </summary>
    /// <param name="tileNum">タイルナンバー</param>
    /// <param name="pos">生成位置</param>
    /// <param name="floor">生成元の親オブジェクト</param>
    /// <param name="floorNum">現在のフロア数</param>
    delegate void TileMap(string tileNum, Vector2 pos, GameObject floor, int floorNum);
    TileMap[] tileList;
    TileMap tile;
    readonly string[] pathName = { "Wall", "PlayerGoal", "Enemy", "Jump", "Move", "EventItem", "EventBlock", "BackGround", "Decoration" };
    /// <summary>
    /// 生成カウント
    /// </summary>
    int counter = 0;
    /// <summary>
    /// 矢印用カウンタ―
    /// </summary>
    int arrowCounter = 0;
    /// <summary>
    /// ステージ番号
    /// ステージ選択時にセットします
    /// </summary>
    static int fileNum = 0;
    /// <summary>
    /// ステージ内のフロア数
    /// </summary>
    readonly static int[] floorCount = { 3, 8 };
    /// <summary>
    /// 現在のステージのフロア数を返します
    /// </summary>
    public static int FloorCount
    {
        get
        {
            return floorCount[fileNum];
        }
    }

    /// <summary>
    /// 各ステージのX,Y長さ
    /// </summary>
    readonly Vector2[] stageVec = { new Vector2(19 - 1, 11 + 1), new Vector2(20 - 1, 11) };

    /// <summary>
    /// 横のステージのフロア数
    /// </summary>
    readonly static int[] stageX = { 3, 4 };
    public static int StageX
    {
        get
        {
            return stageX[fileNum];
        }
    }

    /// <summary>
    /// ステージ番号
    /// </summary>
    public static string stageID = "";

    private void Awake()
    {
        //ステージ番号のステージを作成します
        fileNum = GameManager.Instance.StageNo;
        //fileNum = 0;

        //現在のステージ番号を指定します
        stageID = string.Format("stage{0}", fileNum + 1);

        ji = new JsonInfo();
        ji.MapJson();

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

        //オブジェクトを破棄します
        Destroy(gameObject);
    }

    #region マップ生成読み込み

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
                var filePath = string.Format("Stage/Stage{0}/{1}/{2}", stageNo, sNum + 1, pathName[i]);
                CSVRead(filePath, i, floor, vec, sNum);
            }
        }
    }

    /// <summary>
    /// CSVファイルの読み込み
    /// </summary>
    void CSVRead(string stageTagName, int num, GameObject floor, Vector2 createPos, int floorNum)
    {
        //csvファイルの検索
        var csvFile = Resources.Load(stageTagName) as TextAsset;

        if (csvFile == null) return;

        //csvファイルの読み込み
        var reader = new StringReader(csvFile.text);

        //数字文字を解析しステージを作成します
        StringMozi(reader, num, floor, createPos, floorNum);
    }

    /// <summary>
    /// ファイルの内の","で文字を区切ります
    /// </summary>
    void StringMozi(StringReader reader, int num, GameObject floor, Vector2 createPos, int floorNum)
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
        //生成カウントの初期化
        counter = 0;
        arrowCounter = 0;
    }

    #endregion

    #region 簡略化するための関数

    /// <summary>
    /// 方向を判別する
    /// </summary>
    /// <returns></returns>
    string Direct(string[] array, string num)
    {
        //上左下右の4つの方向を取り出すため % 4の余りを求めます
        //例え : 4番目("8") % 4 = 0番目　--> "U"(上向き)

        var no = SetNumber(array, num) % 4;
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
    /// <param name="obj">作成するオブジェクト</param>
    /// <param name="pos">生成ポジション</param>
    GameObject InstantObj(GameObject obj, Vector2 pos, GameObject floor)
    {
        //オブジェクトを現在のフロアの中に生成します
        var go = Instantiate(obj, pos, Quaternion.identity);
        //parent = フロア番号
        go.transform.SetParent(floor.transform);

        go.transform.localPosition = pos;

        return go;
    }

    #endregion
}

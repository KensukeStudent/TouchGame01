using System;

/// <summary>
/// ステージ1ADVパート
/// </summary>
public class Stage1Event : ADV
{
    delegate string[] advPart();
    advPart[] advParts;

    public bool[] advFlag { private set; get; } = new bool[3];

    public Stage1Event()
    {
        //関数を設定
        advParts = new advPart[]
        {
            ADVPart,
            ADVPartTwoFloor,
            ADVPartThreeFloor
        };
    }

    /// <summary>
    /// ステージ１開始に流れるADVパート
    /// </summary>
    public override string[] ADVPart()
    {
        //使っているコマンド
        //@F* ----> フォント(Font)
        //@D  ----> ADVパートを終わり(Delete)

        string[] tutorial =
        {
            "@F1「ゲーム解説に移ります",
            "このゲームは基本クリックゲームです。\n黒い丸のあるジャンプ先に飛んだり、敵の攻撃を回避することができます。",
            "左上にある鍵は、同じ色のブロック近くでブロックをクリックすることで開くことができます。",
            "また、どんなブロックか知りたいときはブロックの近くに行き、ブロックをクリックしてみましょう。",
            "試しにやってみましょう！",
            "@D"
        };

        return tutorial;
    }

    /// <summary>
    /// 2フロア専用の会話パート
    /// </summary>
    /// <returns></returns>
    public string[] ADVPartTwoFloor()
    {
        string[] tutorial =
        {
            "敵を倒す時はリンゴを食べて力がついた状態でないと倒せません。",
            "敵の攻撃に当たると左上の体力が減ります。体力がなくなるとゲームオーバーになります。",
            "敵を倒してみましょう！",
            "@D"
        };

        return tutorial;
    }

    /// <summary>
    /// 3フロア専用の会話パート
    /// </summary>
    /// <returns></returns>
    public string[] ADVPartThreeFloor()
    {
        string[] tutorial =
        {
           "ブロックが開かれたら、次のフロアへのヒント猫があらわれるのでマウスカーソルを合わせてみましょう。",
           "取ったアイテムを確認したいときやステージを途中で抜けたいときは右上の「M」のボタンを押しましょう。",
           "画面中央にあるのが、お魚です。クリックして取りましょう！",
           "お目当てのお魚を求めて、これから猫の冒険を一緒に楽しみましょう!」",
           "@D"
        };

        return tutorial;
    }

    /// <summary>
    /// イベントアクション
    /// </summary>
    /// <returns></returns>
    public override Action[] Actions()
    {
        Action[] actions = {  };
        return actions;
    }


    /// <summary>
    /// Stage1の会話パートを取得
    /// </summary>
    /// <param name="no"></param>
    /// <returns></returns>
    public string[] GetADV(int no)
    {
        advFlag[no] = true;
        return advParts[no]();
    }
}
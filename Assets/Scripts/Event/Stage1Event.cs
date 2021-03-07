using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ1ADVパート
/// </summary>
public class Stage1Event : MonoBehaviour
{
    /// <summary>
    /// ステージ１開始に流れるADVパート
    /// </summary>
    public string[] ADVTutorial()
    {
        //使っているコマンド
        //@F* ----> フォント(Font)
        //@D  ----> ADVパートを終わり(Delete)

        string[] tutorial =
        {
            "@F1ゲーム解説に移ります",
            "このゲームは基本クリックゲームです。\nジャンプ先に飛んだり、敵の攻撃を回避することができます。",
            "右下にあるブロックは同じ色の鍵を取得し、ブロック近くでブロックをクリックすることで開くことができます。",
            "また、どんなブロックか知りたいときはブロックの近くに行き、ブロックをクリックしてみましょう。",
            "ブロックが開かれたら、次のフロアへのヒント猫があらわれるのでマウスカーソルを合わせてみましょう。",
            "敵を倒す時はリンゴを食べて力ついた状態でないと倒せません。",
            "お目当てのお魚を求めて、これから猫の冒険を一緒に楽しみましょう!",
            "@D"
        };

        return tutorial;
    }

    /// <summary>
    /// イベントアクション
    /// </summary>
    /// <returns></returns>
    public System.Action[] Actions()
    {
        System.Action[] actions = {  };
        return actions;
    }
}

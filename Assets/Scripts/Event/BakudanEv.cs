using System.Collections;
using System;
using UnityEngine;

/// <summary>
/// バクダンイベント
/// </summary>
public class BakudanEv : MonoBehaviour
{
    /// <summary>
    /// 取ったらバクダン生成するポジションを置きます
    /// </summary>
    [SerializeField] GameObject instantBakudanPos;

    /// <summary>
    /// 爆弾イベントのノベルパート
    /// </summary>
    public string[] ADVBakudan()
    {
        //使っているコマンド
        //@F* ----> フォント(Font)
        //@P* ----> 演出(Action)(Paferm)
        //@N  ----> 自動改行(文末につける)(Next)
        //@D  ----> ADVパートを終わり(Delete)

        string[] bakudanNovel =
        {
            "@F0むむっ！？",
            "なんにゃー？？このくだもの？？@P0@N",//爆弾の色を徐々に変えます,//自動で次の文へとぶ
            "にゃ！にゃ！？　ばくだんにゃーーーー！！",
            "@F1バクダンの解説に移ります。\n「右クリックすることで投げることができます」",//フォント変更
            "「敵に向かって投げることで倒すことができます。ただし、壁の近くで投げると自分もまき込まれるのでお気をつけて。」",
            "バクダンを持っている時に攻撃されると起爆してしまうため、敵と遭遇したらすぐに投げることをおすすめします。",
            "@P1@D"//徐々にNovelFrameを消します
        };

        return bakudanNovel;
    }

    /// <summary>
    /// テキスト内で動作させる関数をストックします
    /// </summary>
    /// <returns></returns>
    public Action[] Actions()
    {
        Action[] actions = { EventAnim, EventDelete };
        return actions;
    }

    /// <summary>
    /// テキスト内でアニメーションさせます
    /// </summary>
    void EventAnim()
    {
        //テキスト表示を停止します
        TextManager.StartStop(1);

        StartCoroutine(EventAnimCO());
    }

    /// <summary>
    /// イベント時に流すアニメーション
    /// </summary>
    IEnumerator EventAnimCO()
    {
        var sprite = GetComponent<SpriteRenderer>();

        var color = sprite.color;

        for (int i = 0; i < 100; i++)
        {
            color.r += 0.01f;
            color.g += 0.01f;
            color.b += 0.01f;

            yield return new WaitForSecondsRealtime(0.01f);
            sprite.color = color;
        }

        //テキスト表示を再開始します
        TextManager.StartStop(0);
    }

    /// <summary>
    /// テキスト内で削除します
    /// </summary>
    void EventDelete()
    {
        //全てが終わったら爆弾を削除して、プレイヤーの頭の上に爆弾を表示します
        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.ActiveBakudan();

        //バクダン生成を置きます
        //親の位置
        var go = Instantiate(instantBakudanPos, transform.position, Quaternion.identity);

        var root = transform.root;

        go.transform.SetParent(root);

        Destroy(gameObject);
    }
}

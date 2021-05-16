using UnityEngine;

public class ADVSystem : MonoBehaviour
{
    /// <summary>
    /// ADV呼び出し時に一回限り呼ばれます
    /// </summary>
    public static void StartADV(string[] adv, System.Action[] actions)
    {
        #region 処理流れ

        //黒いeventnの爆弾を取る

        //イベントパート
        //fontの変更
        //----> 「なんにゃー、このくだもの？？」(ネコのセリフ)

        //テキストの方でWaitを書けます(animationの時間分)
        // 爆弾の色を徐々に元の色に戻していきます

        //----> 「にゃ！にゃ！？　ばくだんにゃーーー！！！」

        //解説
        //---> fontを変更
        //----> 「ジャンプ位置でない所で右クリックすることで投げることができます」
        //----> 「敵を倒すこともできるので快感を味わってみてください。
        //        ただし、自分もまき壊れることもあるのでお気をつけて。」
        //----> 「爆弾は何度も生成されるので、何回でも使うことができます」

        #endregion

        //タイムスケールで時間を止めます
        Time.timeScale = 0;

        //表示用テキストCanvas
        var cavas = GameObject.Find("DescriptionCanvas");
        //親から表示用テキストを取得
        var tm = cavas.transform.Find("NovelFrame").GetComponent<TextManager>();

        //読み込むテキストを表示用UIの方に格納します
        tm.SetEvText(adv);

        //NovelFrameを徐々に表示します
        tm.GetImage();

        //advパート内で発火するイベントを入れます
        tm.SetAction(actions);
    }
}

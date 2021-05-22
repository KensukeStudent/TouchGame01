using UnityEngine;

public class ADVSystem : MonoBehaviour
{
    /// <summary>
    /// ADV呼び出し時に一回限り呼ばれます
    /// </summary>
    public static void StartADV(string[] adv, System.Action[] actions)
    {
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

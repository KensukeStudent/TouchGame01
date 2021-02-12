using UnityEngine;

/// <summary>
/// 次のフロアへのHintを伝えるクラス
/// </summary>
public class HintCat : MonoBehaviour
{
    /// <summary>
    /// ヒントテキスト
    /// </summary>
    string hint = "あああああああああああ";

    /// <summary>
    /// テキストの値に自分のhintを入れ、hintアニメーションのフラグを立てます
    /// </summary>
    public void SetHint()
    {
        var a = GameObject.Find("Canvas").GetComponent<test>();
        a.SetFlag(hint);
    }

    /// <summary>
    /// 初期値に値をセットします
    /// </summary>
    /// <param name="h">Jsonからのテキスト</param>
    public void SetInit(string h)
    {
        hint = h;
    }
}

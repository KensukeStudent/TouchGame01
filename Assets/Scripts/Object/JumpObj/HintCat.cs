using UnityEngine;

/// <summary>
/// 次のフロアへのHintを伝えるクラス
/// </summary>
public class HintCat : MonoBehaviour
{
    /// <summary>
    /// ヒントテキスト
    /// </summary>
    string hint = "この先、てきがたくさんいるきがするにゃ！";

    /// <summary>
    /// テキストの値に自分のhintを入れ、hintアニメーションのフラグを立てます
    /// </summary>
    public void SetHint()
    {
        var hU = GameObject.Find("HintCat").GetComponent<HintUI>();
        hU.SetFlag(hint);
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

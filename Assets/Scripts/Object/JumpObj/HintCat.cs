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
        //改行が入った文字をhintに入れます
        hint = Split(h);
    }

    /// <summary>
    /// 文字を区切ります
    /// </summary>
    string Split(string s)
    {
        //7字で区切ります
        const int splitCount = 7;

        //何回くぎるかを求めます
        var counter = Mathf.CeilToInt(s.Length / splitCount);

        //改行分の文字コードをプラス１します
        var kigyo = 0;

        for (int i = 0; i < counter; i++)
        {
            //必ずsplitCount番目から計算します
            var init = (i + 1);

            //7文字目づつ改行コードを入れます
            s = s.Insert(splitCount * init + kigyo, "\n");
            
            //改行コード分 + 1します
            kigyo = 1;
        }
        return s;
    }
}

using UnityEngine;

/// <summary>
/// 名前ログやシナリオログのサイズと文字サイズを設定
/// </summary>
public class InitLogSize : MonoBehaviour
{
    [SerializeField] GameObject sentence;
    [SerializeField] GameObject name;

    private void Start()
    {
        InitSize();
    }

    /// <summary>
    /// サイズと文字サイズを指定
    /// </summary>
    void InitSize()
    {
        //親のrectを取得
        var rtP = GetComponent<RectTransform>();

        //---------------Name---------------//
        RectTransform rtN = name.GetComponent<RectTransform>();
        rtN.anchorMax = new Vector2(0, 1);
        rtN.anchorMin = new Vector2(0, 1);

        //サイズの設定
        var sizeN = rtN.sizeDelta;
        sizeN.x = rtP.sizeDelta.x / 10;
        sizeN.y = rtP.sizeDelta.y / 3;
        rtN.sizeDelta = sizeN;

        //ポジション設定
        rtN.anchoredPosition = new Vector2(sizeN.x / 2, -sizeN.y / 2);

        //---------------Sentence---------------//
        RectTransform rtS = sentence.GetComponent<RectTransform>();
        rtS.anchorMax = new Vector2(1, 0);
        rtS.anchorMin = new Vector2(1, 0);

        //サイズの設定
        var sizeS = rtS.sizeDelta;
        sizeS.x = rtP.sizeDelta.x - sizeN.x;
        sizeS.y = rtP.sizeDelta.y;
        rtS.sizeDelta = sizeS;

        //ポジション設定
        rtS.anchoredPosition = new Vector2(-sizeS.x / 2, sizeS.y / 2);
    }
}

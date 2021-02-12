using TMPro;
using UnityEngine;

public class test : MonoBehaviour
{

    [SerializeField] RectTransform rt;

    bool a = false;

    /// <summary>
    /// アニメーションフラグ
    /// </summary>
    bool flag = false;

    Vector2 max;
    Vector2 min;

    private void Start()
    {
        max = rt.anchoredPosition;
        min = -rt.anchoredPosition;
    }

    private void Update()
    {
        HintAnim();
    }

    /// <summary>
    /// ヒントアニメーション
    /// </summary>
    void HintAnim()//マウスカーソルがあてられた時と離れた時に処理します
    {
        if (!a && flag)
        {
            var pos = rt.anchoredPosition;
            if (pos.x > min.x)
            {
                pos.x -= Time.deltaTime * 500;
                pos.y -= Time.deltaTime * 500;

                if (pos.x <= min.x)
                {
                    pos = min;
                    a = true;//----->フラグはマウスカーソルが離れたら呼びます
                }
            }
            rt.anchoredPosition = pos;
        }
        else if (a && !flag)
        {
            var pos = rt.anchoredPosition;
            if (pos.x < max.x)
            {
                pos.x += Time.deltaTime * 500;
                pos.y += Time.deltaTime * 500;

                if (pos.x >= max.x)
                {
                    pos = max;
                    a = false;//----->フラグはマウスカーソルが離れたら呼びます
                }
            }
            rt.anchoredPosition = pos;
        }
    }

    /// <summary>
    /// マウスカーソルが当たられたら呼びます
    /// </summary>
    public void SetFlag(string hint)
    {
        flag = true;
        a = false;
        var text = GameObject.Find("FloorEvText").GetComponent<TMP_Text>();
        text.text = hint;
    }

    /// <summary>
    /// マウスカーソルが離れたら呼ばれます
    /// </summary>
    public void ReturnFlag()
    {
        a = true;
        flag = false;
    }
}

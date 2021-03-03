using TMPro;
using UnityEngine;

/// <summary>
/// ヒントUIクラス
/// </summary>
public class HintUI : MonoBehaviour
{
    RectTransform rt;

    bool anim = true;

    const float speed = 650;

    /// <summary>
    /// アニメーションフラグ
    /// </summary>
    bool flag = false;

    Vector2 max;
    Vector2 min;

    private void Start()
    {
        rt = GetComponent<RectTransform>();

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
        if (anim && flag)
        {
            var pos = rt.anchoredPosition;
            if (pos.x > min.x)
            {
                pos.x -= Time.deltaTime * speed;
                pos.y -= Time.deltaTime * speed;

                if (pos.x <= min.x)
                {
                    pos = min;
                    anim = false;//----->フラグはマウスカーソルが離れたら呼びます
                }
            }
            rt.anchoredPosition = pos;
        }
        else if (!anim && !flag)
        {
            var pos = rt.anchoredPosition;
            if (pos.x < max.x)
            {
                pos.x += Time.deltaTime * speed;
                pos.y += Time.deltaTime * speed;

                if (pos.x >= max.x)
                {
                    pos = max;
                    anim = true;//----->フラグはマウスカーソルが離れたら呼びます
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
        anim = true;
        var text = transform.GetChild(0).GetComponent<TMP_Text>();
        text.text = hint;
    }

    /// <summary>
    /// マウスカーソルが離れたら呼ばれます
    /// </summary>
    public void ReturnFlag()
    {
        anim = false;
        flag = false;
    }
}

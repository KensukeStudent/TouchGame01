using TMPro;
using UnityEngine;

/// <summary>
/// ヒントUIクラス
/// </summary>
public class HintUI : MonoBehaviour
{
    RectTransform rt;
    /// <summary>
    /// 移動フラグ
    /// </summary>
    bool anim = true;
    /// <summary>
    /// 移動速度
    /// </summary>
    const float speed = 650;

    /// <summary>
    /// アニメーションフラグ
    /// </summary>
    bool flag = false;

    /// <summary>
    /// 位置移動の最大値
    /// </summary>
    Vector2 max;
    /// <summary>
    /// 位置移動の最小値
    /// </summary>
    Vector2 min;

    private void Start()
    {
        rt = GetComponent<RectTransform>();

        //現在の位置を最大値とします
        max = rt.anchoredPosition;
        //現在の逆の位置を最小値とします
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

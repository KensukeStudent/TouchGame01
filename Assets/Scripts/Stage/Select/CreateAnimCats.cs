using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アニメーションをする猫を作り出すクラス
/// </summary>
public class CreateAnimCats : MonoBehaviour
{
    /// <summary>
    /// 生成する画像
    /// </summary>
    [SerializeField] Image image;
    /// <summary>
    /// 代入する猫イラスト
    /// </summary>
    [SerializeField] Sprite[] cats;

    /// <summary>
    /// スコア達成エフェクト
    /// </summary>
    [SerializeField] GameObject scoreEffect;

    private void Start()
    {
        InstantCats(transform);
    }

    /// <summary>
    /// 猫を生成します
    /// </summary>
    public void InstantCats(Transform parent)
    {
        //一つ当たりの角度
        var th = 360 / cats.Length;

        //親のサイズと子(猫)のサイズ
        GetParentSize(parent, out float sizeP, out float sizeC);

        //半径
        var dist = (sizeC / 2f) / Mathf.Tan((th / 2f) * Mathf.Deg2Rad) + sizeP / 2f;

        for (int i = 0; i < cats.Length; i++)
        {
            //生成,画像の割り当て
            var go = Instantiate(image);

            //各猫に画像を割り当てます
            go.sprite = cats[i];

            //rectを取得
            var rt = go.transform as RectTransform;

            //Achor指定
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchorMin = new Vector2(0.5f, 0.5f);

            //自分の子に割り当てます
            rt.transform.SetParent(parent);

            //スケールを初期化します
            rt.localScale = new Vector3(1, 1, 1);

            //1つ1つの角度を求めます
            var angle = 360 * i / cats.Length;

            //x,y軸の位置を求めます
            //角度を弧度法に変換します
            var x = dist * Mathf.Sin(angle * Mathf.Deg2Rad);
            var y = dist * Mathf.Cos(angle * Mathf.Deg2Rad);

            //位置を初期化
            rt.anchoredPosition = new Vector2(0, 0);

            //子に情報を伝えます(角度と周回する半径を入れます)
            var animCat = go.GetComponent<AnimCat>();
            animCat.SetInit(angle, dist, new Vector2(x, y));
        }

        //達成エフェクトを生成します
        Instantiate(scoreEffect, parent);
    }

    /// <summary>
    /// 親のサイズを取得
    /// </summary>
    void GetParentSize(Transform parent, out float sizeP, out float sizeC)
    {
        //親のrectを取得
        var size = parent as RectTransform;
        sizeP = size.sizeDelta.x;
        //生成するrectを取得
        var rt = image.GetComponent<RectTransform>();
        sizeC = rt.sizeDelta.x;
    }
}

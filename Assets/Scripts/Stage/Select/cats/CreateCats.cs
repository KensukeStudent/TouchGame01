using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 猫のアニメーションの基底クラス
/// </summary>
public abstract class CreateCats : MonoBehaviour
{
    /// <summary>
    /// 生成する画像
    /// </summary>
    [SerializeField] protected Image image;

    /// <summary>
    /// 代入する猫イラスト
    /// </summary>
    [SerializeField]protected Sprite[] cats;

    /// <summary>
    /// エフェクト
    /// </summary>
    [SerializeField] protected GameObject effect;

    /// <summary>
    /// 親のサイズを取得
    /// </summary>
    protected void GetParentSize(Transform parent,GameObject child ,out float sizeP, out float sizeC)
    {
        //親のrectを取得
        var size = parent as RectTransform;
        sizeP = size.sizeDelta.x;
        //生成するrectを取得
        var rt = child.GetComponent<RectTransform>();
        sizeC = rt.sizeDelta.x;
    }

    /// <summary>
    /// 猫を作成します
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="aS"></param>
    abstract public void InstantCats(Transform parent, AnimState aS);

    /// <summary>
    /// 角度と半径を取得します
    /// </summary>
    protected void GetAngleRadius(out float radius,int length,Transform parent)
    {
        //一つ当たりの角度
        var th = 360 / length;

        var child = image.gameObject;

        //親のサイズと子(猫)のサイズ
        GetParentSize(parent, child, out float sizeP, out float sizeC);

        //半径
        radius = (sizeC / 2f) / Mathf.Tan((th / 2f) * Mathf.Deg2Rad) + sizeP / 2f;
    }

    /// <summary>
    /// 親の元に生成します
    /// </summary>
    /// <param name="parent">親の下に生成</param>
    protected Image InstantChild(Transform parent)
    {
        //生成,画像の割り当て
        var go = Instantiate(image);

        //rectを取得
        var rt = go.transform as RectTransform;

        //Achor指定
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchorMin = new Vector2(0.5f, 0.5f);

        //自分の子に割り当てます
        rt.transform.SetParent(parent);

        //スケールを初期化します
        rt.localScale = new Vector3(1, 1, 1);

        //位置を初期化
        rt.anchoredPosition = new Vector2(0, 0);

        return go;
    }

    /// <summary>
    /// 生成する位置を取得します
    /// </summary>
    protected void GetVec(out float x, out float y, float radius, float angle)
    {
        //x,y軸の位置を求めます
        //角度を弧度法に変換します
        x = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        y = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
    }
}

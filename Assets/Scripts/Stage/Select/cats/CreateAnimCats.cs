using UnityEngine;

/// <summary>
/// アニメーションをする猫を作り出すクラス
/// </summary>
public class CreateAnimCats : CreateCats
{
    /// <summary>
    /// 猫を生成します
    /// </summary>
    public override void InstantCats(Transform parent,AnimState aS)
    {
        //生成するオブジェクトの長さ
        var length = cats.Length;

        //生成する一つ当たり角度とどのくらいの距離に生成するか半径を求めます
        GetAngleRadius(out float radius, length, parent);

        for (int i = 0; i < length ; i++)
        {
            //親オブジェクトの下に画像を生成します
            var go = InstantChild(parent);

            //各猫に画像を割り当てます
            go.sprite = cats[i];

            //1つ1つの角度を求めます
            var angle = 360 * i / length;

            //角度に応じて指定のベクトル座標を求めます
            GetVec(out float x, out float y, radius, angle);

            //子に情報を伝えます(角度と周回する半径を入れます)
            var animCat = go.GetComponent<AnimCat>();
            animCat.SetHankoInit(aS, new Vector2(x, y));
        }

        //スコア達成エフェクトを生成します
        Instantiate(effect, parent);
    }
}

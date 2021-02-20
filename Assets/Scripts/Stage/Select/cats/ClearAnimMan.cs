using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// クリア猫を制御するクラス
/// </summary>
public class ClearAnimMan : CreateCats
{
    /// <summary>
    /// 回す猫を格納
    /// </summary>
    List<Image> catsObj = new List<Image>();
    /// <summary>
    /// 空の画像
    /// </summary>
    [SerializeField] Sprite enpty;

    private void Awake()
    {
        InstantCats(transform, AnimState.clear);
    }

    /// <summary>
    /// 猫を生成します
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="aS"></param>
    public override void InstantCats(Transform parent, AnimState aS)
    {
        //生成するオブジェクトの長さ
        var length = cats.Length;

        //生成する一つ当たり角度とどのくらいの距離に生成するか半径を求めます
        GetAngleRadius(out float radius, length, parent);

        for (int i = 0; i < length; i++)
        {
            //親オブジェクトの下に画像を生成します
            var go = InstantChild(parent);

            //1つ1つの角度を求めます
            var angle = 360 * i / length;

            //角度に応じて指定のベクトル座標を求めます
            GetVec(out float x, out float y, radius, angle);

            var animCat = go.GetComponent<AnimCat>();
            //角度方向に生成や周回させるための初期値を入れます
            animCat.SetClearInit(aS, new Vector2(x, y), angle, radius, cats[i]);

            //管理子に生成したオブジェクトを格納します
            catsObj.Add(go);

            go.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ステージ更新したときに呼びます
    /// 各ねこを表示か非表示にするかを判定します   
    /// </summary>
    public void SetObject(bool flag)
    {
        //アニメーションが終わっている状態
        for (int i = 0; i < catsObj.Count; i++)
        {
            //表示します
            catsObj[i].gameObject.SetActive(flag);

            if (flag)
            {
                //すべてに画像を入れます
                catsObj[i].sprite = cats[i];
            }
            else
            {
                //空の画像を入れます
                catsObj[i].sprite = enpty;
            }
        }
    }

    /// <summary>
    /// アニメーションの再生する時に呼びます
    /// </summary>
    public void AnimSetObj()
    {
        for (int i = 0; i < catsObj.Count; i++)
        {
            catsObj[i].gameObject.SetActive(true);
            catsObj[i].sprite = enpty;
        }

        //クリアエフェクトを再生します
        Instantiate(effect, transform);
    }
}

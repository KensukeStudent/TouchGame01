using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 現在のプレイヤーからマウス座標までの位置を描くクラス
/// </summary>
public class MouseController : MonoBehaviour
{
    /// <summary>
    /// マウス座標までの位置を示す画像
    /// </summary>
    GameObject mouseToPoint;
    GameObject mousePad;
    /// <summary>
    /// 直径1cmにするのにこの画像比では何倍する必要があるか求めます
    /// </summary>
    float length;
    
    private void Update()
    {
        mouseToPoint = GameObject.Find("CatArm");
        mousePad = GameObject.Find("CatPad");
        var s = mouseToPoint.GetComponent<SpriteRenderer>().size;
        length = 1 / s.y;

        FromPlayerToMouse();
    }

    /// <summary>
    /// プレイヤーからマウスまでの距離を計測し描画する
    /// </summary>
    void FromPlayerToMouse()
    {
        //マウス座標を取得
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0.0f;

        var objPos = mouseToPoint.transform.position;
        var vec = (pos - objPos).normalized;

        //角度方向の向きにします
        mouseToPoint.transform.rotation = Quaternion.Euler(0,0,SetAngle(vec));

        //マウスとオブジェクト分の長さにします
        var size = mouseToPoint.transform.localScale;
        size.y = Mathf.Sqrt(Mathf.Pow(pos.x - objPos.x, 2) + Mathf.Pow(pos.y - objPos.y, 2)) * length;

        mouseToPoint.transform.localScale = size;

        //mouseToPointの子の位置を指定します
        var childSize = mousePad.transform.localScale;
        childSize.y = 1 / size.y;
        mousePad.transform.localScale = childSize;
    }

    /// <summary>
    ///　現在のプレイヤーとマウス座標の角度に応じて画像の方向を変える
    /// </summary>
    /// <returns></returns>
    float SetAngle(Vector3 vec)
    {
        float angle = Mathf.Atan2(vec.y, vec.x);
        return (angle * Mathf.Rad2Deg) - 90;
    }
}

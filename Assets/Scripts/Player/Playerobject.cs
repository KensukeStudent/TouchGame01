using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// プレイヤーとオブジェクト間との処理をするクラス
/// </summary>
public partial class PlayerController : MonoBehaviour
{
    #region ジャンプ
    /// <summary>
    /// 今のマウスカーソルが載っているジャンプ台ではない
    /// </summary>
    /// <returns></returns>
    bool SameJumpObj(GameObject hitObj)
    {
        //比較する初期値
        var result = 0;
        if (moveObj) result = moveObj.GetInstanceID();

        return result == hitObj.GetInstanceID();
    }

    /// <summary>
    /// ジャンプオブジェクトにレイが当たっている時にジャンプオブジェクトの色を変更します
    /// </summary>
    /// <param name="hit"></param>
    void ChangeJumpObjColor(RaycastHit2D hit)
    {
        //ジャンプ先のオブジェクトの色を変更します
        //ヒット先の足場を現在の足場に代入
        hitJumpObj = hit.transform.gameObject;
        var jumpObj = hit.transform.GetComponent<JumpObj>();
        jumpObj?.NowRay();
    }

    /// <summary>
    /// クリックしたときにジャンプオブジェクトの飛び移ります
    /// </summary>
    void ClickMoveToJumpObj(RaycastHit2D hit)
    {
        if (Input.GetMouseButtonDown(0))
        {
            //ジャンプ手数を1増やします
            jumpCount++;
            //テキストを同期させます


            //SEを鳴らします
            PlaySE(0);

            //ヒットした物を入れます
            moveObj = hit.transform.gameObject;

            //parentを解除します
            transform.SetParent(null);

            //角度設定
            var q = transform.localEulerAngles;
            //-----角度を親の角度にします-----//
            q.z = GetJumpAngle(moveObj.transform);
            transform.localEulerAngles = q;

            var pos = transform.position;
            //スプライトのy軸の大きさ / 2
            pos.y -= 0.5f;

            //ジャンプエフェクトを入れます
            Instantiate(dustEffect, pos, transform.localRotation);

            //現在移動中
            move = true;

            //クリック先のJumpObjを捨てます
            RemoveJumpObj();
        }
    }

    /// <summary>
    /// ジャンプ先の角度を求めます
    /// </summary>
    float GetJumpAngle(Transform child)
    {
        //子要素
        var c = child;
        //角度
        var angle = c.transform.localEulerAngles.z;

        //親要素のすべての角度を取得   
        do
        {
            var parent = c.transform.parent;
            //一番上のfloorを管理している親(0や1)の名前であればbreakします
            //それまで角度を取得しつづけます
            if (!OnlyNum(parent.name))
            {
                var q = parent.localEulerAngles.z;
                angle += q;

                c = parent;
            }
            else break;

        } while (true);

        return angle;
    }

    /// <summary>
    /// 数値のみの取得
    /// </summary>
    bool OnlyNum(string s)
    {
        //数値でない文字があるかを判別それを反転させて数値のみかを取得
        return _ = !Regex.IsMatch(s, @"[^0-9]");
    }

    /// <summary>
    /// Rayにヒットした、していた物の状態を更新します
    /// </summary>
    void Object(RaycastHit2D hit)
    {
        //Jumpオブジェクト
        //ジャンプ先のオブジェクトの色を元に戻します
        if (hitJumpObj && !hit) RemoveJumpObj();

        //Hintオブジェクト
        HintObj(hit);
    }

    /// <summary>
    /// マウス座標がジャンプオブジェクトから外れたら処理します
    /// </summary>
    void RemoveJumpObj()
    {
        var jumpObj = hitJumpObj.GetComponent<JumpObj>();
        jumpObj?.RemoveNowRay();
        hitJumpObj = null;
    }

    #endregion


    /// <summary>
    /// ヒントオブジェクトに当たったら処理します
    /// 猫の手アニメーションを開始フラグを立てます
    /// </summary>
    /// <param name="hit"></param>
    void HintObj(RaycastHit2D hit)
    {
        //hintObjがない、ヒットしている
        if (hintObj == null && hit && SameLayer(hit, "Hint"))
        {
            hintObj = hit.transform.gameObject;
            var hint = hintObj.GetComponent<HintCat>();
            hint?.SetHint();//レイがhintLayer上を通ってしまった時は通過してしまいます
        }
        else if (hintObj && !hit)
        {
            var hU = GameObject.Find("HintCat").GetComponent<HintUI>();
            hU.ReturnFlag();
            hintObj = null;
        }
    }

    /// <summary>
    /// ヒットしたオブジェクトと比較するレイヤーを比較します
    /// </summary>
    bool SameLayer(RaycastHit2D hit, string layerName)
    {
        var hitObj = hit.transform.gameObject;
        return hitObj.layer == LayerMask.NameToLayer(layerName);
    }

    /// <summary>
    /// バクダンを持っているときに投げることができます
    /// </summary>
    void ThrowBakudan(Vector2 mousePos)
    {
        if (Input.GetMouseButtonDown(1) && bakudan.activeInHierarchy)
        {
            var pos = Camera.main.ScreenToWorldPoint(mousePos);

            //プレイヤーからマウスまでの角度方向を取得
            //Z成分を加味しないベクトル座標を正規化して角度を求めます
            var direction = Vector3.Scale((pos - transform.position), new Vector3(1, 1, 0)).normalized;

            var go = Instantiate(throwB, bakudan.transform.position, Quaternion.identity);
            var b = go.GetComponent<Bakudan>();

            //ベクトル方向を代入
            b.SetVec(direction);

            //頭上のバクダンを非表示にします
            bakudan.SetActive(false);
        }
    }
}

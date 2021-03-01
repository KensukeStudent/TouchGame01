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
    /// 1マスが1cmなのでこの画像比では何倍する必要があるか求めます
    /// </summary>
    float length;

    /// <summary>
    /// 障害物のレイヤー
    /// </summary>
    [SerializeField] LayerMask obstacle;
     GameObject player;

    private void Start()
    {
        mouseToPoint = GameObject.Find("CatArm");
        mousePad = GameObject.Find("CatPad");
        player = GameObject.FindGameObjectWithTag("Player");

        var s = mouseToPoint.GetComponent<SpriteRenderer>().size;
        //1マスをこの画像では何倍するかを計算します
        length = 1 / s.y;
    }

    private void Update()
    {
        //画像のマウスまでの距離を描画します
        FromPlayerToMouse();

        mouseToPoint.transform.position = player.transform.position;
    }

    /// <summary>
    /// 画像からマウスまでの距離を計測し描画する
    /// </summary>
    public void FromPlayerToMouse()
    {
        //マウス座標を取得
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 1.0f;
        //猫の手画像座標取得
        var objPos = mouseToPoint.transform.position;

        //マウスから猫の手原点のベクトル方向
        var direction = (mousePos - objPos).normalized;
        //角度方向の向きにします
        mouseToPoint.transform.rotation = Quaternion.Euler(0, 0, SetAngle(direction));

        //mouseToPoint画像のサイズ
        var size = mouseToPoint.transform.localScale;
        //mouseToPointの子の画像サイズ
        var childSize = mousePad.transform.localScale;

        //マウス座標から画像座標までの長さを求めます
        size.y = SizeY(mousePos, objPos, direction);
        //親スケールに値を入れます
        mouseToPoint.transform.localScale = size;

        //子のスケールに値を入れます
        childSize.y = 1 / size.y;
        childSize.z = 1;
        mousePad.transform.localScale = childSize;
    }

    /// <summary>
    /// 画像の長さを求めます
    /// </summary>
    float SizeY(Vector3 mousePos, Vector3 objPos, Vector3 direction)
    {
        //初期値を設定
        var size = 0.0f;

        //画像の原点から現在のマウス座標までの間に障害物があるかをチェックします
        if (CheckObstacle(mousePos, objPos, direction, out float distance))
        {
            //hit先の距離 * 画像を1マスに合わせた倍数
            size = distance * length;
        }
        else
        {
            #region 正規化別
            //size.y = Mathf.Sqrt(Mathf.Pow(mousePos.x - objPos.x, 2) + Mathf.Pow(mousePos.y - objPos.y, 2)) * length;
            #endregion

            //マウス座標と画像座標 * 画像を1マスに合わせた倍数
            //距離を求め、正規化します
            size = Vector2.Distance(mousePos, objPos) * length;
        }
        return size;
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

    /// <summary>
    /// 猫の手の原点からマウス座標までの間に障害物があるかを求めます
    /// </summary>
    /// <param name="distance">hit先の距離</param>
    /// <returns></returns>
    bool CheckObstacle(Vector3 mousePos,Vector3 objPos,Vector3 direction, out float distance)
    {   
        //距離を求めます
        var dis = Vector2.Distance(mousePos, objPos);
        //レイ方向を入れ飛ばします
        var ray = new Ray(objPos, direction);
        //レイ方向へ放射したものを検知
        var hit = Physics2D.Raycast(ray.origin, ray.direction, dis, obstacle);

        //初期値を設定
        distance = 0.0f;

        //Debug.DrawRay(ray.origin, ray.direction, Color.green, 0.5f);

        if (hit)
        {
            //ヒットするまでの距離を入れます
            //ヒット先のdistanceを使って計算します
            distance = hit.distance;

            return true;
        }

        return false;
    }
}

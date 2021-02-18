using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アニメーションをする猫クラス
/// </summary>
public class AnimCat : MonoBehaviour
{
    /// <summary>
    /// 自分のRect
    /// </summary>
    RectTransform rt;
    /// <summary>
    /// 生成時の角度位置
    /// </summary>
    float angle = 0;
    /// <summary>
    /// 周回する半径
    /// </summary>
    float radius = 0;
    /// <summary>
    /// 目的座標
    /// </summary>
    Vector2 pos;
    
    /// <summary>
    /// 移動速度X
    /// </summary>
    float moveSpeedX = 350;
    /// <summary>
    /// 移動速度Y
    /// </summary>
    float moveSpeedY = 350;

    bool stopX = false;
    bool stopY = false;

    /// <summary>
    /// 画像を透過させる時間
    /// </summary>
    const float timeT = 1.2f;

    /// <summary>
    /// 自分のImage
    /// </summary>
    Image image;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        //どの方向へ移動するかを決めます
        MoveDirection();
    }

    private void Update()
    {
        //ハンコのアニメーション
        HankoAnim();
    }

    /// <summary>
    /// HankoAnimation
    /// </summary>
    void HankoAnim()
    {
        //指定の座標まで移動します
        if (!stopX || !stopY)
        {
            Move();
        }
        else  //X,Yが両方止まったなら
        {
            //colorのaを下げます
            var color = image.color;
            color.a -= Time.deltaTime * timeT;
            image.color = color;

            if (color.a <= 0)
            {
                //透過されたら削除します
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 角度と半径を入れます
    /// </summary>
    /// <param name="a">angle</param>
    /// <param name="r">radius</param>
    /// <param name="p">position</param> 
    public void SetInit(float a, float r,Vector2 p)
    {
        angle = a;
        radius = r;
        pos = p;
    }

    /// <summary>
    /// 周回の仕方
    /// </summary>
    /// <param name="rad"></param>
    /// <param name="radius"></param>
    void AroundMove()
    {
        var rad = angle * Time.timeSinceLevelLoad;

        //x軸y軸に移動先を与えます
        var relativePos = new Vector2(
            Mathf.Cos(rad) * radius, //周回する半径を拡大していかせます
            Mathf.Sin(rad) * radius);

        //位置を代入
        rt.anchoredPosition = relativePos;
    }

    /// <summary>
    /// 目的座標に応じて移動速度の符号を変更します
    /// </summary>
    void MoveDirection()
    {
        if (pos.x < 0) moveSpeedX = -moveSpeedX;
        if (pos.y < 0) moveSpeedY = -moveSpeedY;
    }

    /// <summary>
    /// 指定の位置まで移動します
    /// </summary>
    void Move()
    {
        var rp = rt.anchoredPosition;

        //X軸の移動
        MoveX(ref rp.x);

        //Y軸の移動
        MoveY(ref rp.y);

        rt.anchoredPosition = rp;
    }

    /// <summary>
    /// X軸への移動
    /// </summary>
    /// <param name="posX">rt.posX</param>
    void MoveX(ref float posX)
    {
        if (stopX) return;

        //右へ進みます              //左へ進みます
        if (pos.x > 0 && posX < pos.x || pos.x < 0 && posX > pos.x)
        {
            posX += moveSpeedX * Time.deltaTime;
        }
        else
        {
            stopX = true;
            posX = pos.x;
        }
    }

    /// <summary>
    /// Y軸への移動
    /// </summary>
    void MoveY(ref float posY)
    {
        if (stopY) return;

        //上へ進みます              //下へ進みます
        if (pos.y > 0 && posY < pos.y || pos.y < 0 && posY > pos.y)
        {
            posY += moveSpeedY * Time.deltaTime;
        }
        else
        {
            stopY = true;
            posY = pos.y;
        }
    }
}

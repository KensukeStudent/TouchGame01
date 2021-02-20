using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アニメーション一覧
/// </summary>
public enum AnimState
{
    hanko,//ハンコアニメーション
    clear //クリアアニメーション
}

/// <summary>
/// アニメーションをする猫クラス
/// </summary>
public class AnimCat : MonoBehaviour
{
    /// <summary>
    /// アニメーションのステート
    /// </summary>
    public AnimState State { private set; get; }

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

    /// <summary>
    /// x軸が指定の座標まで移動しました
    /// </summary>
    bool stopX = false;
    /// <summary>
    /// y軸が指定の座標まで移動しました
    /// </summary>
    bool stopY = false;

    /// <summary>
    /// 画像を透過させる時間
    /// </summary>
    const float timeT = 1.2f;

    /// <summary>
    /// 自分のImage
    /// </summary>
    Image image;
    /// <summary>
    /// イラスト
    /// </summary>
    Sprite sprite;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        //どの方向へ移動するかを決めます
        switch (State)
        {
            case AnimState.hanko:
                MoveDirection();
                break;
           
            case AnimState.clear:
        
                break;
        }
    }

    private void Update()
    {
        switch (State)
        {
            //ハンコのアニメーション
            case AnimState.hanko:

                HankoAnim();

                break;

            //周回アニメーション
            case AnimState.clear:

                AroundMove();

                break;
        }
    }

    /// <summary>
    /// ステートclear時の初期化
    /// </summary>
    public void SetClearInit(AnimState aS, Vector2 p, float a, float rd, Sprite s)
    {
        SetHankoInit(aS, p);

        //回転
        angle = a;
        radius = rd;
        sprite = s;
    }

    /// <summary>
    /// 角度と半径を入れます
    /// </summary>
    /// <param name="aS">アニメーション名</param>
    /// <param name="p">position</param>
    public void SetHankoInit(AnimState aS, Vector2 p)
    {
        //どのアニメーションを動かすか
        State = aS;

        //位置
        pos = p;
    }

    #region 半径を徐々に大きくしていき、周回します

    /// <summary>
    /// 周回の仕方
    /// </summary>
    /// <param name="rad"></param>
    /// <param name="radius"></param>
    void AroundMove()
    {
        //角度方向に生成された初期位置からプラスして回転させていきます
        //度数法から弧度法に変換します
        var rad = (angle * Mathf.Deg2Rad) + Time.timeSinceLevelLoad * 2;

        //x軸y軸に移動先を与えます
        var relativePos = new Vector2(
            Mathf.Cos(rad) * radius, 
            Mathf.Sin(rad) * radius);

        //位置を代入
        rt.anchoredPosition = relativePos;

        //画像が入っていないなら下の処理をします
        if (image.sprite == sprite) return;

        //角度計算
        //ラジアンを角度に直して360で割り、余りを求めることで
        //360を超えた数値を0～360に変換します
        var a = (rad * Mathf.Rad2Deg) % 360;

        if (a > 315 && a < 360)
        {
            image.sprite = sprite;
        }
    }

    //float r = radius / 4;

    /// <summary>
    /// 周回の仕方
    /// </summary>
    /// <param name="rad"></param>
    /// <param name="radius"></param>
    //void AroundMove()
    //{
    //    //角度方向に生成された初期位置からプラスして回転させていきます
    //    var rad = (angle * Mathf.Deg2Rad) + Time.timeSinceLevelLoad * 2;

    //    //x軸y軸に移動先を与えます
    //    //度数法から弧度法に変換します
    //    var relativePos = new Vector2(
    //        Mathf.Cos(rad) * r, //周回する半径を拡大していかせます
    //        Mathf.Sin(rad) * r);

    //    //位置を代入
    //    rt.anchoredPosition = relativePos;

    //    if (r < radius) r += Time.deltaTime * r;
    //}

    #endregion

    #region 一方向に飛びます

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

    #endregion
}

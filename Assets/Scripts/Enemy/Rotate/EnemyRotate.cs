using System.Collections;
using UnityEngine;
#pragma warning disable 649

public enum MoveKind
{
    nomal,
    strecth,
    timeStrecth,
}

/// <summary>
/// 回転する敵につける
/// </summary>
public class EnemyRotate : MonoBehaviour
{
    [SerializeField] MoveKind currentMove;

    /// <summary>
    /// 初期角度
    /// </summary>
    float angle;
    /// <summary>
    /// 中心軸
    /// </summary>
    [SerializeField] GameObject pivot;
    /// <summary>
    /// 半径(周る円の半径と自身の半径)
    /// </summary>
    const float radius = 1.55f + 0.185f;
    float upDownRadius =  0;
    /// <summary>
    /// 一周する速度
    /// </summary>
    const float aroundSpeed = 0.5f;

    /// <summary>
    /// 上下するための判定値
    /// </summary>
    bool updown = false;

    /// <summary>
    /// 平行移動の角度
    /// </summary>
    const int parallel  = 90;

    BoxCollider2D box2D;
    /// <summary>
    /// コライダーの初期値を設定 (0.1174size)
    /// </summary>
    float defaultSize;
    /// <summary>
    /// コライダーサイズが大きくなる時のスピード
    /// </summary>
    float sizeSpeed;
    /// <summary>
    /// コライダーサイズの拡大縮小する時の判定
    /// </summary>
    bool expand = true;

    private void Start()
    {
        InitAngle();
        switch (currentMove)
        {
            case MoveKind.strecth:
            case MoveKind.timeStrecth:
                InitColSizePos();
                break;
        }
    }

    private void Update()
    {
        Around();
        switch (currentMove)
        {
            case MoveKind.strecth:
            case MoveKind.timeStrecth:
                var size = box2D.size;
                var pos = box2D.offset;
                if (currentMove == MoveKind.strecth) ActiveColliderSizeAndPos(ref size, ref pos);
                else if (currentMove == MoveKind.timeStrecth) TimeStrecth(ref size, ref pos);
                //サイズと位置の代入
                box2D.size = size;
                box2D.offset = pos;
                break;
        }
    }

    /// <summary>
    /// 初期角度を求める
    /// </summary>
    void InitAngle()
    {
        //現在位置から中心軸までの距離を求める
        Vector2 relativePos = transform.position - pivot.transform.position;
        var rad = Mathf.Atan2(relativePos.y, relativePos.x);
        //アングルに求めた弧を代入する
        angle = rad;
        relativePos.y = Mathf.Sin(angle) * radius;
        relativePos.x = Mathf.Cos(angle) * radius;
        var pivotPos = new Vector2(pivot.transform.position.x, pivot.transform.position.y);
        transform.position = pivotPos + relativePos;
    }

    /// <summary>
    /// コライダーの初期値(サイズとポジションを)指定します
    /// </summary>
    void InitColSizePos()
    {
        box2D = GetComponent<BoxCollider2D>();
        var size = GetComponent<SpriteRenderer>().size;

        box2D.size = size;
        box2D.offset = Vector2.zero;
        defaultSize = size.x / 10;

        //サイズ時間 = (目標サイズ-現在のサイズ) / 秒数
        sizeSpeed = (size.x / 2 - defaultSize) / 0.5f;

        //初期サイズを指定
        size.x = defaultSize;
        box2D.size = size;
        //初期位置を指定
        var pos = box2D.offset;
        pos.x = Positon(box2D.offset, box2D.size);
        box2D.offset = pos;
    }

    //---------------Around---------------//

    #region Around

    /// <summary>
    /// 円状に回る
    /// </summary>
    void Around()
    {
        //移動する角度指定
        var rad = angle + Time.timeSinceLevelLoad * aroundSpeed;

        //回転に応じて角度を合わせます
        Vector3 e = transform.localEulerAngles;

        switch (currentMove)
        {
            case MoveKind.nomal:
                AroundMove(rad, radius);
                //常に角度90度を足すことで平行移動しているように見せる
                e.z = AroundAngle(transform.position, pivot.transform.position) + parallel;
                break;
            
            case MoveKind.strecth:
            case MoveKind.timeStrecth:
                AroundMove(rad, upDownRadius);
                e.z = AroundAngle(transform.position, pivot.transform.position);
                Stretch();
                break;
        }     
        transform.localEulerAngles = e;
    }

    /// <summary>
    /// 周回の仕方
    /// </summary>
    /// <param name="rad"></param>
    /// <param name="radius"></param>
    void AroundMove(float rad,float radius)
    {
        //x軸y軸に移動先を与えます
        var relativePos = new Vector2(
            Mathf.Cos(rad) * radius,
            Mathf.Sin(rad) * radius);
        //Debug.Log(relativePos);
        var pivotPos = new Vector2(pivot.transform.position.x, pivot.transform.position.y);
        //位置を代入
        transform.position = pivotPos + relativePos;
    }

    /// <summary>
    /// 回転に応じて角度を指定します
    /// </summary>
    float AroundAngle(Vector2 my,Vector2 target) 
    {
        Vector2 dis = target - my;
        float rad = Mathf.Atan2(dis.y, dis.x);
        return rad * Mathf.Rad2Deg;
    }

    #endregion

    //---------------Stretch---------------//

    #region Stretch

    /// <summary>
    /// 上下します
    /// </summary>
    void Stretch()
    {
        //radiusの半径を変更します
        if (updown)
        {
            switch (currentMove)
            {
                //即縮小する
                case MoveKind.strecth:
                    upDownRadius -= Time.deltaTime;
                    break;

                //時間経過で縮小する
                case MoveKind.timeStrecth:
                    strecthTime += Time.deltaTime;

                    if (strecthTime > goTime)
                        upDownRadius -= Time.deltaTime;
                    break;
            }
            if (upDownRadius <= 0)
            {
                updown = false;
                strecthTime = 0;
            }
        }
        else
        {
            upDownRadius += Time.deltaTime;
            if (upDownRadius >= radius) updown = true;
        }
    }

    /// <summary>
    /// 頭身が出ている間コライダーを表示
    /// サイズとポジションを頭身に応じて変更します
    /// </summary>
    void ActiveColliderSizeAndPos(ref Vector2 size,ref Vector2 pos)
    {
        //当たり判定を調整しまう
        if (upDownRadius > radius / 1.6f)
        {
            box2D.enabled = true;
            //Ｍａｘサイズ:radiusの1.6%分のサイズ

            //全体のサイズから1.6%分のサイズになるまで拡大
            if (size.x < (defaultSize * 10) / 1.6f && expand)
            {
                //サイズの拡大
                size.x = Size(size, Time.deltaTime * sizeSpeed);
                if (size.x > (defaultSize * 10) / 1.6f) expand = false;
            }
            //サイズの縮小
            else if(!expand) size.x = Size(size, Time.deltaTime * -sizeSpeed);
            //サイズに応じて位置の調整
            pos.x = Positon(pos, size);
        }
        else
        {
            //初期化する
            box2D.enabled = false;
            size.x = defaultSize;
            pos.x = Positon(pos, size);
            expand = true;
        }
    }

    /// <summary>
    /// コライダーのサイズの変更
    /// </summary>
    float Size(Vector2 size,float calc)
    {
        //サイズが拡大か縮小かを計算します
        size.x += (calc);
        return size.x;
    }

    /// <summary>
    /// サイズに応じたコライダーのポジション変更
    /// </summary>
    /// <param name="pos"></param>
    float Positon(Vector2 pos,Vector2 size)
    {
        pos.x = (-defaultSize * 10 / 2) + size.x / 2;
        return pos.x;
    }

    #endregion

    //---------------TimeStretch---------------//

    #region TimeStretch

    /// <summary>
    /// 伸縮する時間
    /// </summary>
    float strecthTime;
    const float goTime = 2f;

    /// <summary>
    /// 時間差で伸びたり縮んだり 
    /// </summary>
    void TimeStrecth(ref Vector2 size, ref Vector2 pos)
    {
        //当たり判定を調整
        if (upDownRadius > radius / 1.6f)
        {
            box2D.enabled = true;

            if (size.x < (defaultSize * 10) / 1.6f && expand)
            {
                //サイズの拡大
                size.x = Size(size, Time.deltaTime * sizeSpeed);
                if (size.x > (defaultSize * 10) / 1.6f) expand = false;
            }
            else if (!expand && strecthTime > goTime)
                size.x = Size(size, Time.deltaTime * -sizeSpeed);

            //サイズに応じて位置の調整
            pos.x = Positon(pos, size);
        }
        else
        {
            //初期化
            box2D.enabled = false;
            size.x = defaultSize;
            pos.x = Positon(pos, size);
            expand = true;
        }
    }
    #endregion
}

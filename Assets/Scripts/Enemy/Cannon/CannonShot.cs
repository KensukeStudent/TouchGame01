using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 大砲から発射される弾クラス
/// </summary>
public class CannonShot : MonoBehaviour
{
    /// <summary>
    /// 弾の速度
    /// </summary>
    float speed = 0;

    private void Update()
    {
        //移動します
        MoveShot();
    }

    /// <summary>
    /// 弾の移動
    /// </summary>
    void MoveShot()
    {
        var pos = transform.position;

        pos.y += speed * Time.deltaTime;
        
        transform.position = pos;
    }

    /// <summary>
    /// 弾の速度を入れます
    /// </summary>
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Obstacles"))
        {
            //障害物に当たったら削除します
            Destroy(gameObject);
        }   
    }
}

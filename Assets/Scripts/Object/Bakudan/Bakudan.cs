using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 爆弾スクリプト
/// </summary>
public class Bakudan : MonoBehaviour
{
    /// <summary>
    /// 速度
    /// </summary>
    float speed = 5;
    Rigidbody2D rb2d;

    /// <summary>
    /// 投げる方向
    /// </summary>
    Vector3 throwDirect;

    /// <summary>
    /// 爆破エフェクト
    /// </summary>
    [SerializeField] GameObject explosion;

    bool go = false;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        ThrowBakudan();
    }

    /// <summary>
    /// プレイヤーとマウスの角度方向に爆弾を投げます
    /// </summary>
    void ThrowBakudan()
    {
        rb2d.velocity = throwDirect * speed;
    }

    /// <summary>
    /// 投げる方向
    /// </summary>
    public void SetVec(Vector3 direct)
    {
        throwDirect = direct;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Obstacles") || col.CompareTag("Enemy"))
        {
            if (go) return;

            go = true;
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の弾クラス
/// </summary>
public abstract class EnemyShot : MonoBehaviour,IDamage
{
    /// <summary>
    /// 弾の速度
    /// </summary>
    protected float shotSpeed;
    protected Rigidbody2D rb2D;
    /// <summary>
    /// 弾が消滅するまでの時間
    /// </summary>
    const float destoryTime = 5.0f;
    /// <summary>
    /// 消滅後エフェクト
    /// </summary>
    [SerializeField] GameObject effect;

    /// <summary>
    /// プレイヤーへのダメージ基本１
    /// </summary>
    protected int damageAmount = 1;

    protected virtual void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        Destroy(gameObject, destoryTime);
    }

    protected virtual void FixedUpdate()
    {
        ShotMove();
    }

    /// <summary>
    /// 弾の移動
    /// </summary>
    public abstract void ShotMove();

    /// <summary>
    /// 弾の速度を入れます
    /// </summary>
    public void SetSpeed(float speed)
    {
        shotSpeed = speed;
    }

    /// <summary>
    /// 自分もつ相手へのダメージ量
    /// </summary>
    public int Damage()
    {
        return _ = damageAmount;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Obstacles") || col.CompareTag("Player"))
        {
            //エフェクトを作成
            var e = Instantiate(effect, transform.position, Quaternion.identity);
            var efE = e.GetComponent<ExplosionEffect>();
            //エフェクトを指定時間で削除します
            var desTime = efE.DestoryEffectTime();
            Destroy(e, desTime);

            Destroy(gameObject);
        }
    }
}

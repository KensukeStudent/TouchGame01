using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パーティクルの当たり判定
/// </summary>
public class EffectCollider : MonoBehaviour
{
    /// <summary>
    /// 削除する時間
    /// </summary>
    float deleteTime = 0;

    /// <summary>
    /// 当たり判定の拡大速度
    /// </summary>
    float colSpeed;

    /// <summary>
    /// 当たり範囲
    /// </summary>
    float colRange;
    /// <summary>
    /// 最小値
    /// </summary>
    [SerializeField] float colMin;
    /// <summary>
    /// 最大値
    /// </summary>
    [SerializeField] float colMax;

    /// <summary>
    /// 衝突するレイヤー
    /// </summary>
    [SerializeField] LayerMask hitLayer;

    /// <summary>
    /// 当たりコライダー
    /// </summary>
    CircleCollider2D cc;

    [System.Obsolete]
    private void Start()
    {
        //パーティクルのライフタイムを取得
        var particle = GetComponent<ParticleSystem>();
        deleteTime = particle.startLifetime;

        var aud = GetComponent<AudioSource>();

        //アニメーション終了後、削除します
        Destroy(gameObject, aud.clip.length);

        //deleteTimeまでに指定の大きさにするために時間を求めます
        colSpeed = (colMax - colMin) / deleteTime;

        colRange = colMin;

        cc = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        //当たり判定を拡大していきます
        ExpandRange();
    }

    /// <summary>
    /// 当たり判定の拡大
    /// </summary>
    void ExpandRange()
    {
        if (colRange > colMax) return;

        colRange += Time.deltaTime * colSpeed;
        cc.radius = colRange;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player") || col.CompareTag("Enemy") || col.CompareTag("Block"))
        {
            //ヒットしたコライダーに応じて処理を変えます
            HitCollider(col);
        }
    }

    /// <summary>
    /// Hitした物に応じて処理を変えます
    /// </summary>
    void HitCollider(Collider2D col)
    {
        switch (col.tag)
        {
            //プレイヤーの場合
            case "Player":
                //敗北アニメーション開始
                var p = col.GetComponent<PlayerController>();
                p.DiePlayer();
                break;

            //敵の場合
            case "Enemy":
                //敵の破壊の一定の処理を行います
                var e = col.GetComponent<Enemy>();
                e.Explosion();
                break;

            //ブロックの場合(ExplosionBlock)
            case "Block":
                //破壊してアニメーションさせます
                var b = col.GetComponent<BlocksScript>();
                b.Destroy();
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, colRange);
    }
}

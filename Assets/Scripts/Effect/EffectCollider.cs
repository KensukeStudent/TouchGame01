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

    //当たり判定の拡大速度
    float colSpeed;

    //当たり範囲
    float colRange;
    //最小値
    [SerializeField] float colMin;
    //最大値
    [SerializeField] float colMax;

    //衝突するレイヤー
    [SerializeField] LayerMask hitLayer;

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

    /// <summary>
    /// Hitした物に応じて処理を変えます
    /// </summary>
    /// <param name="col"></param>
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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player") || col.CompareTag("Enemy") || col.CompareTag("Block"))
        {
            //ヒットしたコライダーに応じて処理を変えます
            HitCollider(col);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, colRange);
    }
}

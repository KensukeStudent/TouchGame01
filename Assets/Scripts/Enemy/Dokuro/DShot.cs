using UnityEngine;

public class DShot : MonoBehaviour
{
    public float ShotSpeed { set; get; }
    Rigidbody2D rb2D;
    const float destoryTime = 5.0f;
    [SerializeField] GameObject destroyEffect;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        Destroy(gameObject, destoryTime);
    }

    private void Update()
    {
        ShotMove();
    }

    /// <summary>
    /// ベクトル方向に移動
    /// </summary>
    void ShotMove()
    {
        rb2D.velocity = transform.right * ShotSpeed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Obstacles"))
        {
            //エフェクトを作成
            var effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
            var efE = effect.GetComponent<ExplosionEffect>();
            //エフェクトを指定時間で削除します
            var desTime = efE.DestoryEffectTime();
            Destroy(effect, desTime);

            Destroy(gameObject);
        }
    }
}

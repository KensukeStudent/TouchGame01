using UnityEngine;

public class DShot : EnemyShot
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    /// <summary>
    /// ベクトル方向に移動
    /// </summary>
    public override void ShotMove()
    {
        rb2D.velocity = transform.right * shotSpeed;
    }
}

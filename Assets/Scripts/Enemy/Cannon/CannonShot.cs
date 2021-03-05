using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 大砲から発射される弾クラス
/// </summary>
public class CannonShot : EnemyShot
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
    /// 弾の移動
    /// </summary>
    public override void ShotMove()
    {
        var pos = rb2D.velocity;
        //y軸方向に飛ばします
        pos.y = shotSpeed;
        
        //pos.y += ..... * Time.deltatime   これで、徐々に加速させる
        //pos.y += .....　　　　　　　　　これで、高速

        rb2D.velocity = pos;
    }
}

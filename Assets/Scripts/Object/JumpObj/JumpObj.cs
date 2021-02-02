using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObj : MonoBehaviour
{
    /// <summary>
    /// 現在レイにあてられている
    /// </summary>
    public void NowRay()
    {
        var sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.white;
    }

    /// <summary>
    /// あてられていない
    /// </summary>
    public void RemoveNowRay()
    {
        var sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(0, 0, 0, 0.3f);
    }
}

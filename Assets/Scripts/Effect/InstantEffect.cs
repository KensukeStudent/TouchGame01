using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エフェクトを生成するクラス
/// </summary>
public class InstantEffect : MonoBehaviour
{
    [SerializeField] protected GameObject effect;

    /// <summary>
    /// エフェクトを生成します
    /// </summary>
    public GameObject EffectInstant(Vector3 pos)
    {
        return _ = Instantiate(effect, pos, Quaternion.identity);
    }
}

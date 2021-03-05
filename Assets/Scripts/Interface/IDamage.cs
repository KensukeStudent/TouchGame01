using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相手へのダメージ与えるインターフェイス
/// </summary>
interface IDamage
{
    /// <summary>
    /// ダメージを与えます
    /// </summary>
    int Damage();
}

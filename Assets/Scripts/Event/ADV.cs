using System.Collections;
using System;
using UnityEngine;

public abstract class ADV : MonoBehaviour
{
    /// <summary>
    ///ADVパート 
    /// </summary>
    public abstract string[] ADVPart() ;

    /// <summary>
    /// シナリオ内で発火するイベント
    /// </summary>
    public abstract Action[] Actions();
}

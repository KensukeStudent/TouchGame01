using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各フロア内のオブジェクトを管理するクラス
/// </summary>
public class Floor : MonoBehaviour
{
    public List<GameObject> FloorObj { private set; get; } = new List<GameObject>();

    /// <summary>
    /// 自分のフロア内の指定のオブジェクトを取得する
    /// </summary>
    public void SetFloorChildObj(GameObject obj)
    {
        FloorObj.Add(obj);
    }

    /// <summary>
    /// 格納しているオブジェクトを表示します
    /// </summary>
    public void ActiveFloor()
    {
        for (int i = 0; i < FloorObj.Count; i++)
        {
            FloorObj[i]?.SetActive(true);
        }
    }

    /// <summary>
    /// 格納しているオブジェクトを非表示します
    /// </summary>
    public void InActiveFloor()
    {
        for (int i = FloorObj.Count - 1; i > -1; i--)
        {
            if (FloorObj[i] == null) FloorObj.Remove(FloorObj[i]);
            else FloorObj[i].SetActive(false);
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    /// <summary>
    /// 各ステージの達成度
    /// </summary>
    public int[][] scoreData = new int[3][];
    /// <summary>
    /// 各ステージ達成時のアニメーション
    /// </summary>
    public bool[][] animData = new bool[3][];
    /// <summary>
    /// 各ステージのクリア
    /// </summary>
    public bool[] clearData = new bool[3];


    private void Start()
    {
        Instant(scoreData, 0);

        Instant(animData, true);


        int[][] a = scoreData;
        bool[][] b = animData;

        List(a);
        List(b);

        //for (int i = 0; i < scoreData.Length; i++)
        //{
        //    for (int j = 0; j < scoreData[i].Length; j++)
        //    {
        //        Debug.Log(a[i][j]);
        //    }
        //}

        //for (int i = 0; i < animData.Length; i++)
        //{
        //    for (int j = 0; j < animData[i].Length; j++)
        //    {
        //        Debug.Log(b[i][j]);
        //    }
        //}
    }

    void Instant<T>(T[][] s,T d) where T : struct
    {
        for (int i = 0; i < s.Length; i++)
        {
            s[i] = new T[3];
            for (int j = 0; j < s[i].Length; j++)
            {
                s[i][j] = d;
            }
        }
    }

    void List<T>(T[][] a) where T : struct
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Debug.Log(a[i][j]);
            }
        }
    }
}

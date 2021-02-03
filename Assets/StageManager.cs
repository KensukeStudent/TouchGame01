﻿using UnityEngine;

/// <summary>
/// 各ステージを管理するクラス
/// </summary>
public class StageManager : MonoBehaviour
{
    //記憶するもの

    //背景画像(各ステージ選択の背景)
    //クリアフラグ

    /// <summary>
    /// 現在の全ステージ数
    /// </summary>
    const int stageCount = 4;
    /// <summary>
    /// 各ステージ背景
    /// </summary>
    public Sprite[] BackGroundMan { private set; get; } = new Sprite[stageCount];
    /// <summary>
    /// 各ステージの達成度
    /// </summary>
    public int[][] ScoreMan { private set; get; } = new int[stageCount][];
    /// <summary>
    /// 各ステージのクリア
    /// </summary>
    public bool[] StageClearMan { private set; get; } = new bool[stageCount];

    private void Awake()
    {
        //各ステージ初期値を設定します
        InitStageSet();
    }

    /// <summary>
    /// ステージの初期値をセットします
    /// </summary>
    void InitStageSet()
    {
        //ステージ背景のパス
        string backPath = "StageBack/StageBack_";

        for (int b = 0; b < BackGroundMan.Length; b++)
        {
            //パスから背景画像を取得し入れます
            var sprite = Resources.Load<Sprite>(backPath + b);
            BackGroundMan[b] = sprite;
        }

        //初期値未クリア値を宣言
        int[] initClear = { 0, 0, 0 };
        //クリア管理に設定します
        for (int c = 0; c < ScoreMan.Length; c++) ScoreMan[c] = initClear;
        //ステージクリアを初期値で設定します
        for (int s = 0; s < StageClearMan.Length; s++) StageClearMan[s] = false;
    }
}
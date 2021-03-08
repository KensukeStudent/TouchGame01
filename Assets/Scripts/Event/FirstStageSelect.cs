using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 初めてステージセレクトに入った時に呼ばれる
/// </summary>
public class FirstStageSelect : MonoBehaviour
{
    [SerializeField] GameObject stopPanel;

    private void Start()
    {
        //はじめてステージ選択画面に入った時に操作説明をします
        if(!StageManager.IsSelectScene)
        {
            StageManager.IsSelectScene = true;

            ADV.StartADV(SelectADV(), Actions());
        }
        else
        {
            stopPanel.SetActive(false);
        }
    }

    /// <summary>
    /// StageSelect画面ADVパート
    /// </summary>
    /// <returns></returns>
    string[] SelectADV()
    {
        string[] adv =
        {
            "@P0@F1「遊んでいただきありがとうございます。",
            "画面説明に移ります",
            "このゲームはステージ式のゲームとなっています。",
            "左右の猫をクリック、又はマウスでスクロールすることで画面を移動することができます。",
            "ゲームを始めるには、各ステージの「ぼうけんへいく」ボタンを押してください",
            "また、このゲームにはやりこみ要素としてクリア後にゲーム内のジャンプの手数に応じて特殊な演出が見れます。",
            "是非ご堪能ください。」",
            "@P1@D"
        };
        return adv;
    }

    /// <summary>
    /// ADVパート内で動かす演出
    /// </summary>
    /// <returns></returns>
    System.Action[] Actions()
    {
        System.Action[] actions = { StopOn, StopOff };
        return actions;
    }

    void StopOn()
    {
        stopPanel.SetActive(true);
    }

    void StopOff()
    {
        stopPanel.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージごとに発生するイベントを管理するクラス
/// </summary>
public class StageADV : MonoBehaviour
{
    /// <summary>
    /// 発火する会話パート
    /// </summary>
    int advCount = 0;

    Stage1Event st1;

    FloorManager fm;

    private void Start()
    {
        if(GameManager.Instance.StageNo == 0)
        {
            st1 = new Stage1Event();
            fm = GameObject.Find("FloorManager").GetComponent<FloorManager>();
        }
    }


    public void StageADVGO()
    {
        if (GameManager.Instance.StageNo != 0) return;

        if (fm.PlayerFloor == 1)
        {
            ADVGO(1);
        }
        else if (fm.PlayerFloor == 2)
        {
            ADVGO(2);
        }
    }

    /// <summary>
    /// イベント発火
    /// </summary>
    public void ADVGO(int no)
    {
        if (st1.advFlag[no]) return;

        //ADVパートを発火します
        ADVSystem.StartADV(st1.GetADV(advCount), st1.Actions());
        
        advCount++;
    }
}

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各ステージの中身を管理するクラス
/// </summary>
public class StageContentManager : MonoBehaviour
{
    List<StageContent> scList = new List<StageContent>();

    /// <summary>
    /// ステージを入れます
    /// </summary>
    public void AddSC(StageContent sc)
    {
        scList.Add(sc);
    }

    /// <summary>
    /// アニメーションを再生するかを判定します
    /// </summary>
    public void AnimGO()
    {
        var stageNo = GameManager.Instance.StageNo;

        //ステージ番号のリストを取り出します
        var sc = scList[stageNo];

        var sm = GameObject.Find("StageManager").GetComponent<StageManager>();

        //※セーブ前に必ずここを処理します

        //ハンコとクリアのアニメーションを再生します
        sc.SetAnim(stageNo, sm.ScoreAnimMan[stageNo], sm.ClearAnimMan[stageNo], sm);
    }
}

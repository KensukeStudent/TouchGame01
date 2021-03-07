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
        //ステージ番号
        var stageNo = GameManager.Instance.StageNo;

        //ステージ番号のリストを取り出します
        var sc = scList[stageNo];

        var sm = GameObject.Find("StageManager").GetComponent<StageManager>();

        //※セーブ前に必ずここを処理します

        //ハンコとクリアのアニメーションを再生します
        sc.SetAnim(stageNo, sm.ScoreAnimMan[stageNo], sm.ClearAnimMan[stageNo], sm);

        //次のステージを解放します
        StageOpen(stageNo);
    }

    /// <summary>
    /// 即ステージを解放します
    /// </summary>
    void StageOpen(int stageNo)
    {
        //全体のステージ数 - 1番目までステージ解放処理を行います
        if (stageNo - 1 < StageManager.stageCount && stageNo < scList.Count - 1)
        {
            var sc = scList[stageNo + 1];

            //次のscのstopを非表示にします
            sc.SetStop(false);
        }
    }
}

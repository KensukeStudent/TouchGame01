using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// 各ステージの情報を表示するクラス
/// </summary>
public class StageContent : MonoBehaviour
{
    /// <summary>
    /// ステージ名
    /// </summary>
    [SerializeField] TMP_Text stageName;
    /// <summary>
    /// ステージ背景
    /// </summary>
    Image backGround;
    /// <summary>
    /// クリア時に条件クリアのハンコ
    /// </summary>
    [SerializeField] Image[] scores;
    /// <summary>
    /// ステージの達成度
    /// 0か1かでクリアしたかを判別
    /// </summary>
    int[] scoresFlag = new int[3];
    /// <summary>
    /// ハンコのスプライト画像
    /// </summary>
    [SerializeField] Sprite[] hanko;
    /// <summary>
    /// このステージをクリアした
    /// </summary>
    bool stageClear = false;
    /// <summary>
    /// クリアアニメを再生する際の親オブジェクト
    /// </summary>
    [SerializeField] RectTransform clearRt;

    /// <summary>
    /// このステージの中身をセットします
    /// </summary>
    /// <param name="sName">ステージ名</param>
    public void SetContent(string sName, StageManager sm)
    {
        backGround = transform.Find("BackGround").GetComponent<Image>();

        //ステージ番号を数値化します
        var stageNo = int.Parse(Regex.Match(sName, "[0-9]+").Groups[0].Value) - 1;

        //ステージ名を入れます
        stageName.text = sName;
        //ステージ背景
        backGround.sprite = sm.BackGroundMan[stageNo];
        //フラグを入れます
        scoresFlag = sm.ScoreMan[stageNo];

        //猫のハンコを入れます
        for (int i = 0; i < scores.Length; i++)
        {
            //アニメーションが再生されているなら
            //画像を入れます
            if (sm.ScoreAnimMan[stageNo][i]) 
                scores[i].sprite = hanko[scoresFlag[i]];
            else
                scores[i].sprite = hanko[0];
        }

        //クリア時に表示する猫クラスを取得します
        var cat = transform.Find("ClearAnim").GetComponent<ClearAnimMan>();

        //アニメーションが再生されているなら
        if (sm.ClearAnimMan[stageNo])
        {
            //クリアフラグを入れます
            stageClear = sm.StageClearMan[stageNo];
        }

        //クリア時のオブジェクトの状態をセットします
        cat.SetObject(sm.ClearAnimMan[stageNo]);
    }

    /// <summary>
    /// ステージクリア後のアニメーションを再生します
    /// </summary>
    /// <param name="stageNo">現在のステージ番号</param>
    /// <param name="anim_S">ScoreAnim</param>
    /// <param name="anim_C">ClearAnim</param>
    public void SetAnim(int stageNo, bool[] anim_S, bool anim_C, StageManager sm)
    {
        //Hankoアニメーションから開始します
        StartCoroutine(AnimLag_Hanko(stageNo, anim_S, anim_C, sm));
    }

    /// <summary>
    /// Hankoアニメシーンをタイムラグありで再生します
    /// </summary>
    /// ※flag = 1でanimがfalseなら再生します
    IEnumerator AnimLag_Hanko(int stageNo, bool[] anim_S, bool anim_C, StageManager sm)
    {
        bool flaf0 = anim_S[0];
        bool flaf1 = anim_S[1];
        bool flaf2 = anim_S[2];

        bool[] flag = { flaf0, flaf1, flaf2 };

        //ステージハンコに画像を入れます
        for (int i = 0; i < scores.Length; i++)
        {
            //アニメーションが再生中のインターバル
            if (scoresFlag[i] == 1 && !flag[i])
            {
                flag[i] = true;
                //アニメーション再生
                var hankoAnim = GameObject.Find("AnimCatMan").GetComponent<CreateAnimCats>();
                //円型に広がるように猫をアニメーションさせます
                hankoAnim.InstantCats(scores[i].transform,AnimState.hanko);
                var aud = GetComponent<AudioSource>();
                aud.Play();

                //インターバルをつけます
                yield return new WaitForSeconds(1);

                //画像を入れます
                scores[i].sprite = hanko[scoresFlag[i]];
            }
        }

        //smのscoreにフラグを入れます
        sm.SetAnimFlag_S(stageNo, flag);


        //Hankoアニメーション終了後Clearアニメシーンを再生します
        StartCoroutine(AnimLag_Clear(stageNo, anim_C, sm));
    }

    /// <summary>
    /// Clearアニメシーンを再生します
    /// </summary>
    //flagがfalseならアニメーションを再生します
    IEnumerator AnimLag_Clear(int stageNo, bool anim_C, StageManager sm)
    {
        bool flag = anim_C;

        //クリアアニメーションが流れていないなら
        if (!flag)
        {
            flag = true;

            //クリアアニメーション用の猫を処理します
            var catClear = transform.Find("ClearAnim").GetComponent<ClearAnimMan>();
            catClear.AnimSetObj();

            //インターバルをつけます
            yield return new WaitForSeconds(1);

            //smのclearにflagを入れます
            sm.SetAnimFlag_C(stageNo, flag);
        }

        //全てのアニメーション終了後
        //セーブ,画面操作可能にします
        sm.StageAnimFinish();
    }
}

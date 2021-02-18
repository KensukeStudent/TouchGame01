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
    /// ハンコとクリアのアニメーション
    /// </summary>
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// このステージの中身をセットします
    /// </summary>
    /// <param name="sName">ステージ名</param>
    /// <param name="flag">クリアフラグ</param>
    /// <param name="back">ステージ背景</param>
    public void SetContent(string sName, StageManager sm)
    {
        backGround = GetComponent<Image>();

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
            if (sm.ScoreAnimMan[stageNo][i]) scores[i].sprite = hanko[scoresFlag[i]];
            else scores[i].sprite = hanko[0];
        }

        //アニメーションが再生されているなら
        //クリアフラグを入れます
       　if (sm.ClearAnimMan[stageNo]) stageClear = sm.StageClearMan[stageNo];   
    }

    /// <summary>
    /// ステージクリア後のアニメーションを再生します
    /// </summary>
    /// <param name="stageNo">現在のステージ番号</param>
    /// <param name="anim_S">ScoreAnim</param>
    /// <param name="anim_C">ClearAnim</param>
    public void SetAnim(int stageNo, bool[] anim_S, bool anim_C, StageManager sm)
    {
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
                hankoAnim.InstantCats(scores[i].transform);
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

        //アニメーションが再生中のインターバル
        if (!anim)
        {
            flag = true;

            //アニメーション再生


            //インターバルをつけます
            yield return new WaitForSeconds(1);
        }

        //画像を入れます


        //smのclearにflagを入れます
        sm.SetAnimFlag_C(stageNo,flag);

        //全てのアニメーション終了後
        //セーブ,画面操作可能にします
        sm.StageAnimFinish();
    }
}

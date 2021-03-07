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
    /// ステージ番号
    /// </summary>
    [SerializeField] TMP_Text stageNo;
    /// <summary>
    /// ステージ名
    /// </summary>
    [SerializeField] TMP_Text stageName;
    /// <summary>
    /// クリアしていなかったら被せるオブジェクト
    /// </summary>
    [SerializeField] GameObject stop;
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

    #region 初期値

    /// <summary>
    /// このステージの中身をセットします
    /// </summary>
    /// <param name="sName">ステージ名</param>
    public void SetContent(string stageNo, StageManager sm)
    {
        //ステージ番号を数値化します
        var no = int.Parse(RE.GetNo(stageNo)) - 1;

        //ステージ1もしくは前のステージをクリアしていたらstopオブジェクトを非アクティブにします
        if (stageNo == "Stage1" || sm.StageClearMan[no - 1] && no > 0)
        {
            SetStop(false);
        }
        //クリアしていない場合はstopオブジェクトをアクティブ状態にします
        else if (!sm.StageClearMan[no - 1])
        {
            SetStop(true);
        }

        backGround = transform.Find("BackGround").GetComponent<Image>();

        //ステージ名を入れます
        this.stageNo.text = stageNo;

        //ステージ名
        stageName.text = sm.StageName[no];

        //ステージ背景
        backGround.sprite = sm.BackGroundMan[no];
        
        //フラグを入れます
        scoresFlag = sm.ScoreMan[no];

        //猫のハンコを入れます
        for (int i = 0; i < scores.Length; i++)
        {
            //アニメーションが再生されているなら
            //画像を入れます
            if (sm.ScoreAnimMan[no][i]) 
                scores[i].sprite = hanko[scoresFlag[i]];
            else
                scores[i].sprite = hanko[0];
        }

        //クリア時に表示する猫クラスを取得します
        var cat = transform.Find("ClearAnim").GetComponent<ClearAnimMan>();

        //アニメーションが再生されているなら
        if (sm.ClearAnimMan[no])
        {
            //クリアフラグを入れます
            stageClear = sm.StageClearMan[no];
        }

        //クリア時のオブジェクトの状態をセットします
        cat.SetObject(sm.ClearAnimMan[no]);
    }
    
    /// <summary>
    /// Stopオブジェクトを表示か非表示かを決めます
    /// </summary>
    public void SetStop(bool flag)
    {
        stop.SetActive(flag);
    }

    #endregion

    #region アニメーションの再生

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

        //stageの長さ - 1番目までステージ解放を処理します


        //全てのアニメーション終了後
        //セーブ,画面操作可能にします
        sm.StageAnimFinish();
    }
    
    #endregion
}

using UnityEngine.UI;
using TMPro;
using UnityEngine;

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
    int[] clearFlag = new int[3];
    /// <summary>
    /// ハンコのスプライト画像
    /// </summary>
    [SerializeField] Sprite[] hanko;
    /// <summary>
    /// このステージをクリアした
    /// </summary>
    bool stageClear = false;

    /// <summary>
    /// このステージの中身をセットします
    /// </summary>
    /// <param name="sName">ステージ名</param>
    /// <param name="flag">クリアフラグ</param>
    /// <param name="back">ステージ背景</param>
    public void SetContent(string sName,int[] flag,Sprite back,bool clear)
    {
        backGround = GetComponent<Image>();

        //ステージ名を入れます
        stageName.text = sName;
        //ステージ背景
        backGround.sprite = back;
        //フラグを入れます
        clearFlag = flag;
        //ステージハンコに画像を入れます
        for (int i = 0; i < scores.Length; i++) 
            scores[i].sprite = hanko[clearFlag[i]];
        stageClear = clear;
    }
}

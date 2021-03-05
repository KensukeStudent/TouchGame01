using TMPro;
using UnityEngine;
#pragma warning disable 649

/// <summary>
/// ステージ選択から遷移されたら処理されるクラス
/// </summary>
public class UIManager : MonoBehaviour
{
    /// <summary>
    /// ステージ開始前キャンバス
    /// </summary>
    [SerializeField] GameObject stageBacCanvas;
    /// <summary>
    /// ネコの手
    /// </summary>
    [SerializeField] GameObject catHand;
    AudioSource aud;
    /// <summary>
    /// クリックされたら鳴らします
    /// </summary>
    [SerializeField] AudioClip catVoice;
    /// <summary>
    /// クリック後テキスト表示を切り替えます
    /// </summary>
    [SerializeField] GameObject textObj;
    /// <summary>
    /// この文字に変換
    /// </summary>
    string changeText = "We Love Cats";

    private void Start()
    {
        aud = catHand.GetComponent<AudioSource>();
    }

    private void Update()
    {
        //クリックしたら音が鳴り遷移開始
        if (!GameManager.Instance.GameStart && Input.GetMouseButtonDown(0) && ScreenTransition.Instance.TouchClick)
        {
            //遷移準備
            SetCatHand();
        }
    }

    /// <summary>
    /// 猫の手を出します
    /// </summary>
    void SetCatHand()
    {
        //猫の手を表示
        catHand.SetActive(true);
        GameManager.Instance.SetGame(true);
        //テキスト変更とアニメーションの停止
        var text = textObj.GetComponent<TMP_Text>();
        text.text = changeText;
        var tAnim = textObj.GetComponent<Animator>();
        tAnim.enabled = false;
        var clip = aud.clip;
        //現在のclipの長さ時間を待った後に処理開始
        Invoke(nameof(SetCatVoice), clip.length);
    }

    /// <summary>
    /// ボタンの音が終わったタイミングで猫の鳴き声を鳴かせます
    /// </summary>
    void SetCatVoice()
    {
        aud.clip = catVoice;
        aud.Play();

        //ステージのクリックイベントがすべて終わったら遷移を開始します
        ScreenTransition.Instance.TimeST(0);

        //もう使わないので破棄します
        Destroy(gameObject, aud.clip.length);
    }
}

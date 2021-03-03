using TMPro;
using UnityEngine;
#pragma warning disable 649

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// PlayerInventryパネル
    /// MaxY: 135  MinY:-105
    /// </summary>
    RectTransform rectP;
    /// <summary>
    /// 現在動作中
    /// </summary>
    bool move = false;
    /// <summary>
    /// true -> down,false -> up
    /// </summary>
    bool downOrUp = false;
    /// <summary>
    /// 鍵の取得数UIの最上位置
    /// </summary>
    const float maxPos = 135f;
    /// <summary>
    /// 鍵の取得数UIの最低位置
    /// </summary>
    const float minPos = -105f;

    /// <summary>
    /// ステージ開始前キャンバス
    /// </summary>
    [SerializeField] GameObject stageBacCanvas;
    [SerializeField] GameObject catHand;
    AudioSource aud;
    [SerializeField] AudioClip catVoice;
    [SerializeField] GameObject textObj;
    string changeText = "We Love Cats";

    private void Start()
    {
        aud = catHand.GetComponent<AudioSource>();
        rectP = GameObject.Find("KeyPanel").GetComponent<RectTransform>();
    }

    private void Update()
    {
        //クリックしたら音が鳴り遷移開始
        if (!GameManager.Instance.GameStart && Input.GetMouseButtonDown(0) && ScreenTransition.Instance.TouchClick)
        {
            //遷移準備
            SetCatHand();
        }

        //もしゲームスタートならUI鍵取得数を上下出来ます
        if (GameManager.Instance.GameStart) 
            PanelMove();
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
    }

    /// <summary>
    /// 鍵パネルを動作させます
    /// </summary>
    void PanelMove()
    {
        if (Input.GetMouseButtonDown(1) && !move)
        {
            move = true;
            if (rectP.anchoredPosition.y <= minPos) downOrUp = false;
            else if (rectP.anchoredPosition.y >= maxPos) downOrUp = true;
        }

        if (move)
        {
            var pos = rectP.anchoredPosition;

            //down
            if (downOrUp)
            {
                pos.y -= Time.deltaTime * 1000;
                if (pos.y <= minPos)
                {
                    move = false;
                    pos.y = minPos;
                }
            }
            else
            {
                pos.y += Time.deltaTime * 1000;
                if (pos.y >= maxPos)
                {
                    move = false;
                    pos.y = maxPos;
                }
            }
            rectP.anchoredPosition = pos;
        }
    }
}

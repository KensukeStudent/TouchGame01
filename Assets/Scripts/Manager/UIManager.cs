using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#pragma warning disable 649

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// 説明キャンバス
    /// </summary>
    [SerializeField] GameObject desCanvas;

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
    const float maxPos = 135f;
    const float minPos = -105f;

    /// <summary>
    /// ステージ開始前キャンバス
    /// </summary>
    [SerializeField] GameObject stageBacCanvas;
    [SerializeField] GameObject catHand;
    AudioSource aud;
    [SerializeField] AudioClip catVoice;
    bool catb = true;
    [SerializeField] GameObject textObj;
    string changeText = "We Love Cats";

    private void Start()
    {
        desCanvas.SetActive(false);
        aud = catHand.GetComponent<AudioSource>();
        rectP = GameObject.Find("KeyPanel").GetComponent<RectTransform>();
    }

    private void Update()
    {
        //if (!GameStart && Input.GetMouseButtonDown(0)) SetCatHand();
        //else if (GameStart && !aud.isPlaying && catb)
        //{
        //    catb = false;
        //    aud.clip = catVoice;
        //    aud.Play();
        //}

         //if(GameManager.Instance.GameStart)
        PanelMove();
    }

    /// <summary>
    /// 猫の手を出します
    /// </summary>
    void SetCatHand()
    {
        catHand.SetActive(true);
        GameManager.Instance.SetGame(true);
        var text = textObj.GetComponent<TMP_Text>();
        text.text = changeText;
        var tAnim = textObj.GetComponent<Animator>();
        tAnim.enabled = false;
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

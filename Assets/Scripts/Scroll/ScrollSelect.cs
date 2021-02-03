using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージセレクトスクロールクラス
/// </summary>
public class ScrollSelect : MonoBehaviour
{
    /// <summary>
    /// 親のキャンバス
    /// </summary>
    [SerializeField] RectTransform rCanvas;
    /// <summary>
    /// 自分のrect
    /// </summary>
    RectTransform rt;
    /// <summary>
    /// ステージ分の数
    /// </summary>
    const int stageCount = 4;
    /// <summary>
    /// ステージUIを3つ生成します(残りは使い回します)
    /// </summary>
    const int maxInstant = 3;
 
    /// <summary>
    /// ステージプレファブ
    /// </summary>
    [SerializeField] GameObject stagePre;
    /// <summary>
    /// 位置をリストに保存 
    /// </summary>
    public LinkedList<RectTransform> linkList = new LinkedList<RectTransform>();
    /// <summary>
    /// 現在のStageContent番号
    /// </summary>
    int currentNo = 0;
    /// <summary>
    /// 次の番号になったら前の情報を+2番目の情報に書き換えます
    /// </summary>
    float updatePos = 0;

    /// <summary>
    /// スクロールカウント
    /// </summary>
    int scrollNo = 0;
    /// <summary>
    /// スクロール位置
    /// </summary>
    List<float> rPos = new List<float>();

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        var sm = GameObject.Find("StageManager").GetComponent<StageManager>();
        //contentサイズの指定
        InitSizeContent();
        //stageSelectサイズの指定
        InstantStage(sm);

        for (int i = 0; i < stageCount; i++)
        {
            rPos.Add(rCanvas.sizeDelta.x * i);
        }
    }

    private void Update()
    {
        while (scrollNo < stageCount - 1 && -rt.anchoredPosition.x > rPos[scrollNo + 1])
        {
            scrollNo++;
            AutoScrollFalse();
        }

        while (scrollNo > 0 && -rt.anchoredPosition.x < rPos[scrollNo - 1])
        {
            scrollNo--;
            AutoScrollFalse();
        }

        //ボタンを押したときにオートでスクロールします
        AutoScroll();

        //スクロールされたときに呼ばれます
        StageUpdate();
    }

    /// <summary>
    /// Contentのサイズをキャンバスサイズに合わせます
    /// </summary>
    void InitSizeContent()
    {
        //キャンバスサイズに合わせた大きさに設定します
        var size = rt.sizeDelta;
        size.x = rCanvas.sizeDelta.x * stageCount;
        rt.sizeDelta = size;
        //画面サイズ分の位置をupdatePosに設定します
        updatePos = -rCanvas.sizeDelta.x;
    }

    /// <summary>
    /// ステージを生成分、生成します
    /// </summary>
    void InstantStage(StageManager sm)
    {
        for (int i = 0; i < maxInstant; i++)
        {
            var s = Instantiate(stagePre);
            var sRt = s.transform as RectTransform;

            //Achor指定
            sRt.anchorMax = new Vector2(0, 0.5f);
            sRt.anchorMin = new Vector2(0, 0.5f);

            //親指定
            sRt.transform.SetParent(transform);

            //画面の中央に生成します
            var sPos = sRt.anchoredPosition;
            //画面の半分の位置(中心位置) + 次の画面サイズ * 何番目
            sPos.x = rCanvas.sizeDelta.x / 2 + rCanvas.sizeDelta.x * i;
            sPos.y = 0;
            sRt.anchoredPosition = sPos;
            //ステージ表示するための情報の箱を取得
            var sc = sRt.GetComponent<StageContent>();
            //このステージに情報を入れます
            InitStageInfo(sm, sc, i);

            //後入れでリストの中にStageContentをしまっていきます
            linkList.AddLast(sRt);
        }
    }

    /// <summary>
    /// 初期値として生成されるステージ分、情報を設定します
    /// </summary>
    void InitStageInfo(StageManager sm,StageContent sc,int num)
    {
        //Stage名を数字の値にします
        var stageNo = "Stage" + (num + 1);
        //ステージ名、そのステージのスコア、背景、クリア済みかの情報を入れます
        sc.SetContent(stageNo, sm.ScoreMan[num], sm.BackGroundMan[num], sm.StageClearMan[num]);
    }

    /// <summary>
    /// スクロールされ位置が一定なったら情報を更新します
    /// </summary>
    void StageUpdate()
    {
        //右に引っ張る
        //contentのポジションとnextPosを引いて0になったら処理します  (stageCount - maxInstant)分使いまわします
        while ( -rt.anchoredPosition.x + updatePos > 0 && currentNo < stageCount - maxInstant)
        {
            //nextPosに値を足す(-100 + (-100))次のcontentのポジションを等しい値にするため
            updatePos += -rCanvas.sizeDelta.x;

            //一番上のlist.Value削除して一番下に入れ直します
            var stage = linkList.First.Value;
            linkList.RemoveFirst();
            linkList.AddLast(stage);

            //位置をmaxInstant分右に移動させます
            var pos = stage.anchoredPosition.x + rCanvas.sizeDelta.x * maxInstant;
            stage.anchoredPosition = new Vector2(pos, 0);

            currentNo++;
            //(maxInstant + currentNo)番目のリストを参照します

            //余分に処理してしまったものを非表示にします
            SetActiveR(stage.gameObject);
        }

        //左に引っ張る
        //contentのポジションとupdatePosを引いて右辺より大きくなったら処理します
        while (-rt.anchoredPosition.x + updatePos < -rCanvas.sizeDelta.x && currentNo > 0)
        {
            //nextPosに値を引く(-100 + 100) -contentのポジションを引き値を0にするため
            updatePos += rCanvas.sizeDelta.x;

            //一番下のlist.Valueを削除して一番上に入れ直します
            var stage = linkList.Last.Value;
            linkList.RemoveLast();
            linkList.AddFirst(stage);

            //位置をmaxInstant分、左に移動させます
            var pos = stage.anchoredPosition.x - rCanvas.sizeDelta.x * maxInstant;
            stage.anchoredPosition = new Vector2(pos, 0);

            currentNo--;

            //currentNo番目のリストを参照します

            //余分に処理してしまったものを非表示にします
            SetActiveL(stage.gameObject);
        }
    }

    /// <summary>
    /// currentNoが指定の値になれば表示させます
    /// </summary>
    void SetActiveR(GameObject obj)
    {
        //stageCount - maxInstant = 使いまわします数以上ならfalse
        if(currentNo > stageCount - maxInstant) obj.SetActive(false);
    }

    /// <summary>
    /// currentNoが指定の値になれば非表示させます
    /// </summary>
    void SetActiveL(GameObject obj)
    {
        //stageCount - maxInstant = currentNoが使いまわします数以上の値ならtrueにして戻します
        if (currentNo >= stageCount - maxInstant) obj.SetActive(true);
    }


    bool moveR;
    /// <summary>
    /// 右ボタンを押したときの処理
    /// </summary>
    public void ClickRight()//マイナス方向
    {
        moveR = true;
        moveL = false;
    }

    bool moveL;
    /// <summary>
    /// 左ボタンを押したときの処理
    /// </summary>
    public void ClickLeft()//プラス方向
    {
        moveR = false;
        moveL = true;
    }

    /// <summary>
    /// 右か左のボタンが押されたときに移動処理がされます
    /// </summary>
    void AutoScroll()
    {
        //現在のcurrentNo + 1番目のリスト番号の位置に移動
        if (moveR && scrollNo < stageCount - 1 && -rt.anchoredPosition.x < rPos[scrollNo + 1])
        {
            var rPos = rt.anchoredPosition;
            rPos.x -= Time.deltaTime * 1000;
            rt.anchoredPosition = rPos;
        }
        //現在のcurrentNo - 1番目のリスト番号の位置に移動
        else if (moveL && scrollNo > 0 && -rt.anchoredPosition.x > rPos[scrollNo - 1])
        {
            var rPos = rt.anchoredPosition;
            rPos.x += Time.deltaTime * 1000;
            rt.anchoredPosition = rPos;
        }
    }

    void AutoScrollFalse()
    {
        moveR = false;
        moveL = false;
    }
}

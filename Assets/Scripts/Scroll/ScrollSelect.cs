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
    LinkedList<RectTransform> linkList = new LinkedList<RectTransform>();
    /// <summary>
    /// 現在のStageContent番号
    /// </summary>
    int updateCounter = 0;
    /// <summary>
    /// 次の番号になったら前の情報を+2番目の情報に書き換えます
    /// </summary>
    float updatePos = 0;

    /// <summary>
    /// 右ボタンクリック右方向にスクロール
    /// </summary>
    bool moveR;
    /// <summary>
    /// 左ボタンクリック左方向にスクロール
    /// </summary>
    bool moveL;
    /// <summary>
    /// スクロールカウント
    /// </summary>
    int scrollNo = 0;
    /// <summary>
    /// 次にスクロールするインデックス番号
    /// </summary>
    int nextScroll = 0;
    /// <summary>
    /// スクロール位置
    /// </summary>
    List<float> sPos = new List<float>();

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        var sm = GameObject.Find("StageManager").GetComponent<StageManager>();
        
        //contentサイズの指定
        InitSizeContent();
        
        //stageSelectサイズの指定
        InstantStage(sm);
       
        //ボタンのスクロール位置の記憶
        InitScrollPos();
    }

    private void Update()
    {
        //scrollNoを更新します
        ScrollNoUpdate();

        //ボタンを押したときにオートでスクロールします
        AutoScroll();

        //スクロールされたときに呼ばれます
        StageUpdate();

        //自動スクロールの際もしクリックしてしまったらオートを解除します
        if (Input.GetMouseButtonDown(0))
        {
            moveR = false;
            moveL = false;
        }
    }

    #region 初期化

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
    /// ボタンスクロールの位置を記憶します
    /// </summary>
    void InitScrollPos()
    {
        for (int i = 0; i < stageCount; i++)
            sPos.Add(rCanvas.sizeDelta.x * i);
    }
    #endregion

    #region ステージの更新

    /// <summary>
    /// スクロールされ位置が一定なったら情報を更新します
    /// </summary>
    void StageUpdate()
    {
        //右に引っ張る
        //contentのポジションとnextPosを引いて0になったら処理します  (stageCount - maxInstant)分使いまわします
        while ( -rt.anchoredPosition.x + updatePos > 0 && updateCounter < stageCount - maxInstant)
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

            updateCounter++;
            //(maxInstant + currentNo)番目のリストを参照します

            //余分に処理してしまったものを非表示にします
            SetActiveR(stage.gameObject);
        }

        //左に引っ張る
        //contentのポジションとupdatePosを引いて右辺より大きくなったら処理します
        while (-rt.anchoredPosition.x + updatePos < -rCanvas.sizeDelta.x && updateCounter > 0)
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

            updateCounter--;

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
        if(updateCounter > stageCount - maxInstant) obj.SetActive(false);
    }

    /// <summary>
    /// currentNoが指定の値になれば非表示させます
    /// </summary>
    void SetActiveL(GameObject obj)
    {
        //stageCount - maxInstant = currentNoが使いまわします数以上の値ならtrueにして戻します
        if (updateCounter >= stageCount - maxInstant) obj.SetActive(true);
    }
    #endregion

    #region ボタンスクロール

    /// <summary>
    /// 右ボタンを押したときの処理
    /// </summary>
    public void ClickRight()//マイナス方向
    {
        //値を同期させます(移動のずれを防ぐため)
        nextScroll = scrollNo;

        //現在の位置がsPosのstageCount - 1未満なら処理します
        if (Mathf.Abs(rt.anchoredPosition.x) < sPos[stageCount - 1])
        {
            moveR = true;
            moveL = false;

            //クリックしたとき仮に現在のnextScrollが２であるとき
            //現在のrtポジションを-1500とするこの時２の要素は-2406とする。
            //クリックしたときにnextScrollは+1されるため、※2の要素よりも前にいるのに3の要素に行き、2回右に進んでいるように見えてしまう
            //現在の値が２要素のポジションを超えていたら-1する処理をする
            if (Mathf.Abs(rt.anchoredPosition.x) < sPos[nextScroll])
            {
                scrollNo--;
                return;
            }

            //右に一つずつ移動させたいのでscrollNo + 1の値がnextScroll + 1の値を超えていた場合
            //ずれる前の値に書き換えます
            if ((scrollNo + 1) > nextScroll + 1)
            {
                //次の位置を入れ
                nextScroll = scrollNo;
                //値を前の位置に戻します----->1番の位置に移動したいのに2番に移動するを防ぎます
                scrollNo = nextScroll - 1;
            }
            else
            {
                //次に移動する番号 +1を入れます
                nextScroll = scrollNo + 1;
            }
        }
    }

    /// <summary>
    /// 左ボタンを押したときの処理
    /// </summary>
    public void ClickLeft()//プラス方向
    {
        //値を同期させます(移動のずれを防ぐため)
        nextScroll = scrollNo;

        //現在のポジションがsPosの0番目の位置より大きいなら処理します
        if (Mathf.Abs(rt.anchoredPosition.x) > sPos[0])
        {
            moveR = false;
            moveL = true;

            //クリックしたとき仮に現在のnextScrollが２であるとき
            //現在のrtポジションを-1500とするこの時１の要素は-1209とする。
            //クリックしたときにnextScrollは-1されるため、※1の要素よりも後ろにいるのに0の要素に行き２回左に進んでいるように見えてしまう
            //現在の値が２要素のポジションを超えていたら+1する処理をする
            if (Mathf.Abs(-rt.anchoredPosition.x) > sPos[nextScroll])
            {
                scrollNo++;
                return;
            }

            //左に一つずつ移動させたいのでscrollNo - 1の値がnextScroll - 1の値を超えていた場合
            //ずれる前の値に書き換えます
            if (scrollNo - 1 < nextScroll - 1)
            {
                //次の位置を入れ
                nextScroll = scrollNo;
                //値を前の位置に戻します --->1番の位置に移動したいのに0番の位置に移動するを防ぎます
                scrollNo = nextScroll + 1;
            }
            else
            {
                //次に移動する番号 -1を入れます
                nextScroll = scrollNo - 1;
            }
        }
    }

    /// <summary>
    /// 右か左のボタンが押されたときに移動処理がされます
    /// </summary>
    void AutoScroll()
    {
        //現在のcurrentNo + 1番目のリスト番号の位置に移動
        if (moveR && rt.anchoredPosition.x <= sPos[nextScroll])
        {
            var sPos = rt.anchoredPosition;
            sPos.x -= Time.deltaTime * 1000;
            rt.anchoredPosition = sPos;
        }
        //現在のcurrentNo - 1番目のリスト番号の位置に移動
        else if (moveL && rt.anchoredPosition.x <= sPos[nextScroll])
        {
            var sPos = rt.anchoredPosition;
            sPos.x += Time.deltaTime * 1000;
            rt.anchoredPosition = sPos;
        }
    }

    /// <summary>
    /// 一定までスクロールしたらscrollNoを更新します
    /// </summary>
    void ScrollNoUpdate()
    {
        //目標の位置になったら処理します
        while (scrollNo < stageCount - 1 && -rt.anchoredPosition.x > sPos[scrollNo + 1])
        {
            scrollNo++;
            AutoScrollFalse();
        }

        while (scrollNo > 0 && -rt.anchoredPosition.x < sPos[scrollNo - 1])
        {
            scrollNo--;
            AutoScrollFalse();
        }
    }

    /// <summary>
    /// オートスクロールを停止します
    /// </summary>
    void AutoScrollFalse()
    {
        moveR = false;
        moveL = false;
    　　//ぴったり合うように座標を指定
        var rPos = rt.anchoredPosition;
        rPos.x = -sPos[scrollNo];
        rt.anchoredPosition = rPos;
    }

    #endregion
}

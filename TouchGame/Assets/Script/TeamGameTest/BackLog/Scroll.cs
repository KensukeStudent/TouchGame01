using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    /// <summary>
    /// 自分のRect
    /// </summary>
    RectTransform rt;
    [SerializeField] RectTransform canvasRt;
    /// <summary>
    /// 生成するバックログ
    /// </summary>
    [SerializeField] GameObject backLog;

    const int maxCount = 5;
    Vector2 instantAnchorPos = new Vector2(0.5f, 1);
    
    /// <summary>
    /// バックログに生成する数を設定
    /// </summary>
    [Range(7, 15)]
    [SerializeField] public int instantCount;

    /// <summary>
    /// ログ内容を保存
    /// </summary>
    List<(string name, string sentence)> logList = new List<(string name, string sentence)>();
    /// <summary>
    /// 位置をリストに保存(使いまわす)
    /// </summary>
    public LinkedList<RectTransform> linkList = new LinkedList<RectTransform>();

    /// <summary>
    /// 現在のバックログのナンバー
    /// </summary>
    int currentLogNo = 0;
    
    /// <summary>
    /// ログの値を変化させるための位置
    /// </summary>
    float nextPos = 0;

    /// <summary>
    /// ログYサイズを設定
    /// </summary>
    private float sizeY = -1;
    public float LogSizeY {
        get
        {
            if (sizeY == -1) sizeY = canvasRt.sizeDelta.y / maxCount;
            return sizeY;
        }
    }

    /// <summary>
    /// 前回から何個のログが追加されたかを記録
    /// </summary>
    int beforeBackLogCount = 0;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        SetSizeBackLog();
        MySize();
        InitMyPos();
    }

    private void OnEnable()
    {
        //バックログが非表示の間、何個追加されたか
        var activeCount = logList.Count - beforeBackLogCount;
        //Debug.Log("追加された数:" + activeCount);

        if (activeCount > 0 && currentLogNo >= 0)
        {
            //Debug.Log("追加されました。サイズを大きくします");
            MySize();
            ActiveLog(activeCount);
        }
    }

    private void OnDisable()
    {
        beforeBackLogCount = logList.Count;

        //currentCountがマイナスになっていた場合linkListの調整をする
        //---->表示する際に値がずれる
        if (currentLogNo < 0) ReMakeLinkList();
    }

    private void Update()
    {
        ScrollLog();
    }

    /// <summary>
    /// バックログのサイズを調整し生成する
    /// </summary>
    void SetSizeBackLog()
    {
        for (int i = 0; i < instantCount; i++)
        {
            GameObject b = Instantiate(backLog);
            RectTransform bRt = b.transform as RectTransform;

            //Anchorを指定
            bRt.anchorMax = instantAnchorPos;
            bRt.anchorMin = instantAnchorPos;

            //親指定
            bRt.transform.SetParent(transform);//自分の上に生成する

            //サイズの変更
            var size = bRt.sizeDelta;
            size.x = canvasRt.sizeDelta.x;//きっちり合わせる
            size.y = LogSizeY;//最大5個表示させる
            bRt.sizeDelta = size;

            bRt.anchoredPosition = new Vector2(0, -size.y / 2 - size.y * i);

            linkList.AddLast(bRt);

            //生成する数がログリストより多い場合それは非表示として生成する
            //SetActive(i, b);

            //logDesに指定の文字を入れる
            if(i < logList.Count)
            {
                //var logDes = b.transform.GetChild(0).GetComponent<LogDescription>();
                //logDes.SetLogInfo(logList[i].name, logList[i].sentence);
            }
        }
    }

    /// <summary>
    /// 自身のサイズを指定
    /// </summary>
    /// <param name="sizeY"></param>
    /// <param name="counter"></param>
    void MySize()
    {
        var size = rt.sizeDelta;
        size.y = LogSizeY * logList.Count;
        size.x = canvasRt.sizeDelta.x;
        rt.sizeDelta = size;
    }

    /// <summary>
    /// 自身のアンカーとポジションを設定
    /// </summary>
    void InitMyPos()
    {
        //Contentの初期値を設定
        rt.anchorMax = instantAnchorPos;
        rt.anchorMin = instantAnchorPos;

        rt.anchoredPosition = new Vector2(-rt.sizeDelta.x / 2, 0);
    }

    /// <summary>
    /// このオブジェクトを表示するか非表示するか
    /// </summary>
    void SetActive(int counter ,GameObject obj)
    {
        //下に引っ張った時、0より小さかったらそれは非表示
        //上に下げた時、list.Countより大きかったらそれは非表示
        if (counter < 0 || counter >= logList.Count) obj.SetActive(false);
        else obj.SetActive(true);
    }

    /// <summary>
    /// currentLogNoが-1以下の時にLinkLogを調整する
    /// </summary>
    void ReMakeLinkList()
    {
        //Debug.Log("位置を調整します");

        //生成するrectのyサイズを引く
        nextPos += LogSizeY;
        //一番上のlist.Valueを一番下に入れる
        var log = linkList.First.Value;
        linkList.RemoveFirst();
        linkList.AddLast(log);

        //その後rectのy軸ポジションを調整する
        var logPos = log.anchoredPosition.y - LogSizeY * instantCount;
        log.anchoredPosition = new Vector2(0, logPos);

        //自分の位置を調整する(currentLogNoが-1から始まらないようにする---> -1だと表示処理ができないため)
        var pos = rt.anchoredPosition;
        pos.y = 0;
        rt.anchoredPosition = pos;

        currentLogNo = 0;

        //logList.Count未満なら非表示から表示に変える(currentLogNoが-1になった時に一番上に来るため非表示になるため)
        SetActive(currentLogNo + instantCount, log.gameObject);
    }

    /// <summary>
    /// スクロールする時に呼ぶ処理
    /// </summary>
    void ScrollLog()
    {
        if (logList == null) return;

        //上に引っ張る
        while (-rt.anchoredPosition.y + nextPos < -LogSizeY)
        {
            //生成するrectのyサイズを引く
            nextPos += LogSizeY;
            //一番上のlist.Valueを一番下に入れる
            var log = linkList.First.Value;
            linkList.RemoveFirst();
            linkList.AddLast(log);

            //その後rectのy軸ポジションを調整する
            var pos = log.anchoredPosition.y - LogSizeY * instantCount;
            log.anchoredPosition = new Vector2(0, pos);

            //上に引っ張った時のログのナンバー
            var logIndex = instantCount + currentLogNo;

            // instanrCount + currentLogNo番目のlistの内容を読み込んで入れる
            if (instantCount + currentLogNo < logList.Count)
            {
                //子要素に入っているコンポーネントを参照
                //var logDes = log.transform.GetChild(0).GetComponent<LogDescription>();
                //logListから参照したログ内容を入れる
                //logDes.SetLogInfo(logList[logIndex].name, logList[logIndex].sentence);
            }

            //表示非表示
            SetActive(logIndex, log.gameObject);

            //Debug.Log("instant:" + instantCount + "No:" + currentLogNo + "Sum:" + (instantCount + currentLogNo));
            
            //0から計算するため後に足して処理を行う
            currentLogNo++;
        }

        //下に引っ張る
        while (-rt.anchoredPosition.y + nextPos > 0)
        {
            //生成するrectのyサイズを足す
            nextPos -= LogSizeY;

            //一番下のlist.Valueを一番上に入れる
            var log = linkList.Last.Value;
            linkList.RemoveLast();
            linkList.AddFirst(log);

            //その後rectのy軸を調整する
            var pos = log.anchoredPosition.y + LogSizeY * instantCount;
            log.anchoredPosition = new Vector2(0, pos);

            //currentLogNo - 1から計算するため先に引く
            currentLogNo--;

            //下に引っ張った時のログナンバー
            var logIndex = currentLogNo;

            // currentLogNo番目のlistの内容を読み込んで入れる(マイナスの値は処理しない)
            // それが入っているlogList分あれば
            if (currentLogNo >= 0 && currentLogNo < logList.Count)
            {
                //子要素に入っているコンポーネントを参照
                //var logDes = log.transform.GetChild(0).GetComponent<LogDescription>();
                //logListから参照したログ内容を入れる
                //logDes.SetLogInfo(logList[logIndex].name, logList[logIndex].sentence);
            }

            //Debug.Log("instant:" + instantCount + "No:" + currentLogNo + "Sum:" + currentLogNo);

            //表示非表示
            SetActive(logIndex, log.gameObject);
        }
    }

    /// <summary>
    /// ログが新たに追加された後、非表示になっているものをリストの大きさより小さいなら表示し直す
    /// スクロールした時の値(currentLogNo + instantCount)がlogListを超えていたら呼ぶ
    /// </summary>
    void ActiveLog(int activeCount)
    {
        var counter = 0;
        //子要素に何個の非表示オブジェクトがあるか
        for (int i = 0; i < transform.childCount; i++)
            if (!transform.GetChild(i).gameObject.activeInHierarchy) counter++;

        //表示させるものがあれば処理する
        if (counter > 0)
        {
            //Debug.Log("非表示の数:" + counter + "表示させる数:" + activeCount);

            //この位置から表示処理をする
            var activeNum = linkList.Count - counter;

            //foreach内の回した個数を記録
            var linkCount = 0;
            foreach (var log in linkList)
            {
                if (activeCount <= 0) break;
                else if (linkCount >= activeNum && activeCount > 0)
                {
                    log.gameObject.SetActive(true);

                    //logListから参照するナンバー
                    var logIndex = logList.Count - activeCount;

                    //子要素に入っているコンポーネントを参照
                   // var logDes = log.transform.GetChild(0).GetComponent<LogDescription>();
                    //logListから参照したログ内容を入れる
                    //logDes.SetLogInfo(logList[logIndex].name, logList[logIndex].sentence);

                    activeCount--;
                }

                linkCount++;
            }
        }
    }

    /// <summary>
    /// 読み終わったシナリオをログに入れる
    /// </summary>
    /// <param name="name"></param>
    /// <param name="sentence"></param>
    public void SetLogList(string name , string sentence)
    {
        logList.Add((name, sentence));
    }
}

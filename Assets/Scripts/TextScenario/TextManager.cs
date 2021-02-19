using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Collections;

#region 文字数について

//一列の文字数は20文字とします。
//行はいくらでも大丈夫です(限度はもちろんありです！。)

#endregion

/// <summary>
/// テキストを出力するクラス
/// </summary>
public class TextManager : MonoBehaviour
{
    /// <summary>
    /// シナリオ読み込みクラス
    /// </summary>
    ScenarioReader sr;

    /// <summary>
    /// 記述するテキスト欄
    /// </summary>
    [SerializeField] TMP_Text text;
    [Header("Audio")]
    [SerializeField] AudioSource clickAud;
    [SerializeField] AudioSource bgmAud;
    [SerializeField] AudioSource seAud;

    #region BGM一覧
    //0 ----> 朝
    #endregion
    [SerializeField] AudioClip[] bgmClip;
    #region SE一覧
    //0  ------>   クリック音
    #endregion
    [SerializeField] AudioClip[] seClip;
    /// <summary>
    /// クリック画像
    /// </summary>
    [SerializeField] Image nextClick;
    #region 画像一覧
    //0 ----> 空の画像
    //1 ----> クリック画像
    #endregion
    /// <summary>
    /// クリックのイラスト
    /// </summary>
    [SerializeField] Sprite[] clickSprite;
    /// <summary>
    /// クリックアニメ
    /// </summary>
    Animator animC;

    /// <summary>
    /// 出力するテキスト
    /// </summary>
    string scenario = "";
    /// <summary>
    /// 現在何番目の文字を出力するカウンター
    /// </summary>
    int counter = 0;
    /// <summary>
    /// 次の文字を表示する時間
    /// </summary>
    const float nextTime = 0.05f;
    /// <summary>
    /// 出力計測時間
    /// </summary>
    float time = 0;

    #region stopの能力
    /// 0 -> 文字出力
    /// 1 -> 停止
    #endregion
    /// <summary>
    /// 文字の出力に指示します
    /// </summary>
    int stop = 0;

    /// <summary>
    /// 20字ごとに改行します
    /// </summary>
    const int lineCount = 20;
    /// <summary>
    /// 行数
    /// </summary>
    int line = 0;
    /// <summary>
    /// その行の文字数
    /// </summary>
    int lineLength = 0;

    private void Start()
    {
        sr = new ScenarioReader();
        animC = nextClick.GetComponent<Animator>();
        Init();
    }

    private void Update()
    {
        MainUpdate();
    }

    /// <summary>
    /// 全ての処理をします
    /// </summary>
    void MainUpdate()
    {
        //文を出し終わったらクリック可能
        //クリック後は次の文章に移ります
        ClickNext();

        //stopが1なら処理を止めます
        if (stop == 1) return;

        time += Time.deltaTime;

        if (time < nextTime) return;

        //現在の文字に特殊コードがあるかを判定、処理します
        CheckMozi();

        if (counter >= scenario.Length) return;

        //次の文字を表示します
        SetNextMozi();

        //最後まで出力しました
        FinishCount();

        //20文字目に改行を入れます
        SetLine();
    }

    /// <summary>
    /// 次の一文字を表示、予約します
    /// </summary>
    void SetNextMozi()
    {
        text.text += scenario.Substring(counter, 1);
        clickAud.PlayOneShot(clickAud.clip);
        time = 0;

        counter++;
        lineLength++;
    }

    /// <summary>
    /// lineLengthが20文字になったら改行文字を入れます
    /// </summary>
    void SetLine()
    {
        if (lineLength == lineCount)
        {
            //改行コードを入れます
            text.text += "\n";
            //行数++して、
            line++;
            //その行の長さを初期化します
            lineLength = 0;
        }
    }

    #region 特殊コード処理

    /// <summary>
    /// 特殊コードがある場合処理します
    /// </summary>
    void CheckMozi()
    {
        //次の文字に特殊文字がないかをチェックします
        var s = scenario.Substring(counter, 1);

        switch (s)
        {
            //改行コードがある場合の処理
            case "\n":

                line++;
                //次の改行に備えて-1します
                lineLength = -1;

                break;

            #region 特殊コード一覧
            //@Sシーン名       次のシーンに飛びます
            //@A**             音を鳴らします
            //@E**             SEを鳴らします
            //@B**             背景を変えます
            #endregion

            //特殊コード
            case "@":

                //次の文字を取得します
                var sc0 = scenario.Substring(counter + 1, 1);
                
                //複数の特殊コードで処理を分けます
                SpecailCode(sc0);
               
                break;

            //タグコード
            case "<":

                break;

            default:
                break;
        }
    }

    /// <summary>
    /// 特殊コード
    /// </summary>
    void SpecailCode(string sc)
    {
        switch (sc)
        {
            //Scene遷移
            case "S":
                //コードを処理します(Scene)
                SpecialS();
                break;

            //オーディオ、SE、背景の処理
            //〇**(文字+数字)
            case "A":
            case "E":
            case "B":
                //コード処理をします(Audio,SE,BackGround)
                SpecialAEB(sc);
                break;
        }
    }

    /// <summary>
    /// Sceneの特殊コード
    /// </summary>
    void SpecialS()
    {
        var scene = Regex.Match(scenario, @"@S(.+)").Groups[1].Value;

        //クリック音が終わったら遷移開始します
        StartCoroutine(ChangeScene(scene));

        //@とSとsceneの長さ分プラスします
        counter += 2 + scene.Length;

        //テキスト表示を止めます
        stop = 1;
    }

    /// <summary>
    /// Sceneの遷移
    /// </summary>
    /// <param name="scene"></param>
    IEnumerator ChangeScene(string scene)
    {
        yield return new WaitForSeconds(seClip[0].length);
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// オーディオ、SE、背景の特殊コード処理
    /// </summary>
    void SpecialAEB(string sc)
    {
        //次の文字を取得します
        //2文字を取得
        var sc1 = scenario.Substring(counter + 2, 1);
        var sc2 = scenario.Substring(counter + 3, 1);
        var no = int.Parse(sc1) + int.Parse(sc2);


        //BGM
        if (sc == "A")
        {
            //BGMを鳴らします
            bgmAud.clip = bgmClip[no];
            bgmAud.Play();
        }
        //SE
        else if (sc == "E")
        {
            //SEを鳴らします
            seAud.PlayOneShot(seClip[no]);
        }
        //BackGround
        else if (sc == "B")
        {
            //背景を変えます

        }

        //特殊コード分、プラスします
        counter += 4;

        FinishCount();
    }

    #endregion

    #region 文章終わりの処理

    /// <summary>
    /// 文字数が文字列の長さと同じ時に処理します
    /// </summary>
    void FinishCount()
    {
        if (counter >= scenario.Length)
        {
            //テキスト表示を止めます
            stop = 1;

            //マークを指定の位置に出します
            SetMarkPos();
            //画像の切り替え
            ClickSprite();
        }
    }

    /// <summary>
    /// クリックしたら次の文の読み込みを開始します
    /// </summary>
    void ClickNext()
    {
        if(stop == 1 && Input.GetMouseButtonDown(0) && !sr.FinishScenario())
        {
            Init();
            seAud.PlayOneShot(seClip[0]);
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    void Init()
    {
        //初期化します
        stop = 0;
        //counterを0にします
        counter = 0;
        //行を0にします
        line = 0;
        //行の文字を0にします
        lineLength = 0;
        //timeを0にします
        time = 0;
        //表示テキストを空にします
        text.text = "";
        //画像の切り替え
        ClickSprite();
        //シナリオに次の一文を割り当てます
        scenario = sr.IncreaseIndex();
    }

    /// <summary>
    /// クリックマークの座標を求めます
    /// </summary>
    void SetMarkPos()
    {
        //マージン
        const float margin = 3.8f;
        //fontサイズ
        const float fontSize = 36.0f;

        //rectを取得
        var rt = nextClick.transform as RectTransform;
        //初期位置
        var initVec = new Vector2(fontSize / 2, -fontSize / 2);
        rt.anchoredPosition = initVec;

        //位置を指定します
        var pos = rt.anchoredPosition;
        var size = rt.sizeDelta;

        //初期位置にプラスします
        //(フォントサイズ + マージン) * (行目番目の)文字の長さ
        pos.x += (fontSize + margin) * lineLength + size.x;
        //フォントサイズ * (列番目の)長さ
        pos.y -= fontSize * line + size.y / 2.3f;

        rt.anchoredPosition = pos;
    }

    /// <summary>
    /// クリック画像を切り替えます
    /// </summary>
    void ClickSprite()
    {
        nextClick.sprite = clickSprite[stop];

        switch (stop)
        {
            case 0:
                animC.SetBool("Arrow", false);
                break;
            
            case 1:
                animC.SetBool("Arrow", true);
                break;
        }
    }

    #endregion
}

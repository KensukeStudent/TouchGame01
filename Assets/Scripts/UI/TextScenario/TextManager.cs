using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Collections;
using System;

/// <summary>
/// 場所に応じて出力の仕方を変更します
/// </summary>
public enum MoziState
{
    scenario,//シナリオ
    game     //ゲーム内のイベント
}

/// <summary>
/// テキストを出力するクラス
/// </summary>
public class TextManager : MonoBehaviour
{
    #region 定義

    /// <summary>
    /// 出力種類
    /// </summary>
    [SerializeField] MoziState moziState;

    /// <summary>
    /// シナリオ読み込みクラス
    /// </summary>
    ScenarioReader sr;

    /// <summary>
    /// 記述するテキスト欄
    /// </summary>
    [SerializeField]public TMP_Text text;
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
    #region Font一覧
    //ふで
    //明朝
    #endregion
    /// <summary>
    /// フォント
    /// </summary>
    [SerializeField] TMP_FontAsset[] fonts;
    #endregion

    #region 出力

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
    const float nextTime = 0.025f;
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
    static int stop = 1;

    /// <summary>
    /// 20字か25字ごとに改行します
    /// </summary>
    int lineCount = 20;
    /// <summary>
    /// 行数
    /// </summary>
    int line = 0;
    /// <summary>
    /// その行の文字数
    /// </summary>
    int lineLength = 0;

    #endregion

    #region イベント

    Action[] actions;

    #endregion

    private void Start()
    {
        switch (moziState)
        {
            //シナリオ
            case MoziState.scenario:

                //テキスト出力する文字を保存
                sr = new ScenarioReader();
                //改行を20文字にセットします
                lineCount = 20;
                //scenarioIdのシナリオデータを読み込みます
                sr.SinarioModeInit();
                //初期化
                Init();
                break;
            
            //ゲーム内イベント
            case MoziState.game:

                //改行を25文字にセットします
                lineCount = 25;

                break;
        }

        animC = nextClick.GetComponent<Animator>();
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }

    private void Update()
    {
        //文字出力します
        MainUpdate();
    }

    #region ゲーム内イベント用

    /// <summary>
    /// ゲーム内のイベントを配列番号で発火します
    /// </summary>
    /// <param name="a">参照されるAction</param>
    public void SetAction(Action[] a)
    {
        actions = a;
    }

    /// <summary>
    /// イベントのテキストをストックします
    /// イベントのオブジェクトによって呼ばれます
    /// </summary>
    public void SetEvText(string[] evText)
    {
        //srがなければ作成します
        if (sr == null)
        {
            sr = new ScenarioReader();
        }

        gameObject.SetActive(true);
        //呼び出す時にこれから出力するテキストをストックします
        sr.SetArray(evText);

        //徐々に表示するフレーム(枠の透明度が1になったらstop = 0にします)

    }
    
    /// <summary>
    /// 透明化する画像を取得します
    /// </summary> 
    public void GetImage()
    {
        var image = GetComponent<Image>();
        //同時に透明度を変更させる画像の取得
        Image[] cats = new Image[2];

        //検索するパス
        string[] catsPath = { "RightCat", "LeftCat" };

        for (int i = 0; i < 2; i++)
        {
            cats[i] = transform.Find(catsPath[i]).GetComponent<Image>();
        }

        //Colorのalpha値を取得します
        var alpha = image.color;

        //透過処理を行います
        StartCoroutine(TransparentCO(alpha, image, cats));
    }

    IEnumerator TransparentCO(Color alpha,Image image,Image[] cats)
    {
        //alpha値が0なら プラス
        //alpha値が1ならマイナス
        var flag = alpha.a;

        //毎フレーム0.025を足していき演出します
        for (int i = 0; i < 100; i++)
        {
            if (flag == 0)
            //透明度を下げます
                alpha.a += 0.025f;
            else
            //透明度を上げます
                alpha.a -= 0.025f;

            yield return new WaitForSecondsRealtime(0.01f);

            image.color = alpha;

            for (int c = 0; c < cats.Length; c++)
            {
                cats[c].color = alpha;
            }
        }

        if (flag == 0)
            //初期化
            Init();//これからテキストを出力していきます
        else
            //文章を読み終わります
            gameObject.SetActive(false);
    }

    #endregion

    #region 文字の処理

    /// <summary>
    /// 全ての処理をします
    /// </summary>
    void MainUpdate()
    {
        //読むためのフラグが立っているなら処理します
        if (!sr.IsReading()) return;

        //文を出し終わったらクリック可能
        //クリック後は次の文章に移ります
        ClickNext();

        //stopが1なら処理を止めます
        if (stop == 1) return;

        time += Time.unscaledDeltaTime;

        if (time < nextTime) return;

        //現在の文字に特殊コードがあるかを判定、処理します
        if (counter >= scenario.Length || CheckMozi()) return;

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

    #endregion

    #region 特殊コード処理

    /// <summary>
    /// 特殊コードがある場合処理します
    /// </summary>
    bool  CheckMozi()
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
        }

        return _ = Regex.IsMatch(s, @"(@|<)");
    }

    /// <summary>
    /// 特殊コード
    /// </summary>
    void SpecailCode(string sc)
    {
        #region 特殊コード一覧
        
        //@Sシーン名       次のシーンに飛びます
        //@A**             音を鳴らします
        //@E**             SEを鳴らします
        //@B**             背景を変えます
        //@P*              アクションを発火します
        //@F*              フォントを変更します
        //@Sシーン名       Sceneを変更します
        //@N               自動改行
        //@D               ADVパートを終了します

        #endregion

        switch (sc)
        {
            //文字を使わない
            case "S":
            case "N":
            case "D":
                //文字を扱わない
                NotMozi(sc);
                break;

            //二つの文字を扱う
            case "A":
            case "E":
            case "B":
            //一つの文字を扱う
            case "P":
            case "F":
                //次の文字数字を取得
                var sc1 = scenario.Substring(counter + 2, 1);
                //数字を扱い特殊イベントを発火します
                UseSuzi(sc,sc1);
                break;
        }
    }

    /// <summary>
    /// @の後の文字のみで処理を分けます
    /// </summary>
    void NotMozi(string sc)
    {
        switch (sc)
        {
            //Scene
            case "S":
                SpecialS();
                break;
            //自動改行
            case "N":
                SpecialN();
                break;
            //徐々に透明化させます
            case "D":
                GetImage();
                stop = 1;
                break;
        }
    }

    /// <summary>
    /// 数字を扱う特殊コード
    /// </summary>
    void UseSuzi(string sc,string sc1)
    {
        switch (sc)
        {
            //二つの文字を扱う
            //〇**(文字+数字(2文字))
            //A(Audio),E(SE),B(BackGround)
            case "A":
            case "E":
            case "B":
                SpecialAEB(sc, sc1);
                break;

            //一つの文字を扱う
            //〇*(文字 + 数字(1文字))
            //P(Peform),F(Font)
            case "P":
            case "F":
                OneSuzi(sc, sc1);
                break;
        }
    }

    /// <summary>
    /// 一つの数字だけ使います
    /// </summary>
    void OneSuzi(string sc,string sc1)
    {
        switch (sc)
        {
            //Peform
            case "P":
                //ゲーム内イベント発火
                SpecialP(sc1);
                break;

            //Font
            case "F":
                //テキスト出力fontを変更
                SpecialF(sc1);
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
        yield return new WaitForSecondsRealtime(seClip[0].length);
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// 自動改行します
    /// </summary>
    void SpecialN()
    {
        Init();
        //画像の切り替え
        ClickSprite();
    }

    /// <summary>
    /// オーディオ、SE、背景の特殊コード処理
    /// </summary>
    void SpecialAEB(string sc , string sc1)
    {
        //次の文字を取得します
        //2文字を取得
        var sc2 = scenario.Substring(counter + 3, 1);
        var no = int.Parse(sc1) + int.Parse(sc2);

        switch (sc)
        {  
            //BGM
            case "A":
                //BGMを鳴らします
                bgmAud.clip = bgmClip[no];
                bgmAud.Play();
                break;

            //SE
            case "E":
                //SEを鳴らします
                seAud.PlayOneShot(seClip[no]);
                break;

            //BackGround
            case "B":
            
                break;
        }

        //特殊コード分、プラスします
        counter += 4;

        FinishCount();
    }

    /// <summary>
    /// ゲーム内でのアニメーション
    /// </summary>
    void SpecialP(string sc1)
    {
        //番号を取得
        var no = int.Parse(sc1);

        //指定のアクションを起こします
        actions[no]();

        //3文字飛ばします
        counter += 3;
    }

    /// <summary>
    /// Stopフラグに応じて出力を変えます
    /// </summary>
    /// <param name="flag">0->出力,1->停止</param>
    public static void StartStop(int flag)
    {
        stop = flag;
    }

    /// <summary>
    /// フォントの変更
    /// </summary>
    /// <param name="sc1"></param>
    void SpecialF(string sc1)
    {
        var no = int.Parse(sc1);
        text.font = fonts[no];

        counter += 3;
    }

    #endregion

    #region 文章終わりの処理

    /// <summary>
    /// 文字数が文字列の長さと同じ時に処理します
    /// </summary>
    void FinishCount()
    {
        if(counter >= scenario.Length)
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
        if(stop == 1 && Input.GetMouseButtonDown(0) && FinishSentence() && !sr.FinishScenario())
        {
            Init();
            //画像の切り替え
            ClickSprite();
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
        //シナリオに次の一文を割り当てます
        scenario = sr.IncreaseIndex();
    }

    /// <summary>
    /// クリックマークの座標を求めます
    /// </summary>
    void SetMarkPos()
    {
        //マージン
        const float marginX = 3.8f;
        const float marginY = 50.0f;
        //fontサイズ
        const float fontSize = 50.0f;

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
        pos.x += (fontSize + marginX) * lineLength + size.x;
        //フォントサイズ * (列番目の)長さ
        pos.y -= (fontSize + marginY) * line + size.y / 2.3f;

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
                nextClick.gameObject.SetActive(false);
                break;
            
            case 1:
                animC.SetBool("Arrow", true);
                nextClick.gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 現在の文章を読み切りました
    /// </summary>
    /// <returns></returns>
    bool FinishSentence()
    {
        //文字カウントが現在の文章を全て表示しきった
        return _ = counter >=  scenario.Length;
    }

    #endregion
}

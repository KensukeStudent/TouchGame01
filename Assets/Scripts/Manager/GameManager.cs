using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームを管理するクラス
/// </summary>

public class GameManager : MonoBehaviour,IAudio
{
    /// <summary>
    /// ステージ番号
    /// </summary>
    public int StageNo { private set; get; } = 0;

    /// <summary>
    /// ゲーム開始フラグ
    /// </summary>  
    public bool GameStart { private set; get; } = false;

    public static GameManager Instance { private set; get; }

    AudioSource aud;
    [SerializeField] AudioClip[] clip;

    [SerializeField] GameObject waningImage;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    /// <summary>
    /// GameStartを指定します
    /// </summary>
    public void SetGame(bool start)
    {
        GameStart = start;
    }

    /// <summary>
    /// ステージ番号を入れます
    /// </summary>
    /// <param name="no"></param>
    public void StageSet(int no)
    {
        StageNo = no;
    }

    #region TitleButton

    /// <summary>
    /// スタートボタン
    /// </summary>
    public void StartButton()
    {
        //ボタンとそれを管理しているコンポーネントを切ります
        InActive();

        //セーブデータを初期化
        var delete = new SaveLoad();
        var sm = GameObject.Find("StageManager").GetComponent<StageManager>();
        //データを削除し、新たにデータを作成します
        delete.FileDelete(sm);

        //SEを鳴らします
        PlaySE(0);

        //sceneをノベルパート
        StartCoroutine(DelaySceneChange("NovelScene", aud.clip.length));
    }

    /// <summary>
    /// つづきからボタン
    /// </summary>
    public void ContinueButton()
    {
        //SEを鳴らします
        PlaySE(0);

        var load = new SaveLoad();

        var data = new StageData();

        //セーブデータがあるなら
        if (load.Load(load.SavePath, ref data))
        {
            //ボタンとそれを管理しているコンポーネントを切ります
            InActive();

            var sm = GameObject.Find("StageManager").GetComponent<StageManager>();
            //データを引き継ぎます
            sm.InitData();

            //セーブデータを参照しつづきから始めます
            StartCoroutine(DelaySceneChange("StageSelect", aud.clip.length));
        }
        else
        {
            //警告メッセージ表示
            waningImage.SetActive(true);
        }
    }

    /// <summary>
    /// サウンド分遅らせて、scene遷移させます
    /// </summary>
    IEnumerator DelaySceneChange(string sceneName,float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 終わるボタン
    /// </summary>
    public void EndButton()
    {
        //ゲームを終わりにします
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else 
        Application.Quit();
#endif
    }

    /// <summary>
    /// 警告ボタン
    /// </summary>
    public void WaningButton()
    {
        waningImage.SetActive(false);
    }

    /// <summary>
    /// SEを鳴らします
    /// </summary> 
    public void PlaySE(int clipNo, float vol = 1)
    {
        aud.clip = clip[clipNo];
        aud.Play();
    }

    /// <summary>
    /// クリックした後、コンポーネントを切ります
    /// </summary>
    void InActive()
    {
        //ボタンUIとマウスを管理しているコンポーネントを取得
        var tm = GameObject.Find("EventSystem").GetComponent<TitleUIManager>();

        //切ります
        tm.enabled = false;

        //ボタンコンポーネントを持っているクラスを全て取得
        var bs = FindObjectsOfType<Button>();
        for (int i = 0; i < bs.Length; i++)
        {
            //切ります
            bs[i].enabled = false;
        }
    }

    #endregion
}

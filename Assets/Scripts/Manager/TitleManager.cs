using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour, IAudio
{
    AudioSource aud;
    [SerializeField] AudioClip[] clip;
    
    /// <summary>
    /// 警告イメージ
    /// </summary>
    [SerializeField] GameObject waningImage;

    private void Start()
    {
        aud = GetComponent<AudioSource>();
    }

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
        delete.FileDelete(sm);                    /////ここが怪しい点

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
        if (load.SaveDataLoad(ref data))
        {
            //ボタンとそれを管理しているコンポーネントを切ります
            InActive();

            var sm = GameObject.Find("StageManager").GetComponent<StageManager>();
            //データを引き継ぎます
            sm.InitData();

            //セーブデータを参照しつづきから始めます
            StartCoroutine(DelaySceneChange("StageSelect", aud.clip.length));

            //select状態を初期化します
            ScrollSelect.InitSelect();
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
    IEnumerator DelaySceneChange(string sceneName, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 警告を閉じます
    /// </summary>
    public void WarningColsed()
    {
        waningImage.SetActive(false);
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
}

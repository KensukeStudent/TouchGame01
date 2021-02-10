﻿ using UnityEngine;
 using UnityEngine.SceneManagement;

/// <summary>
/// ゲームを管理するクラス
/// </summary>

public class GameManager : MonoBehaviour
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

    public void StartButton()
    {
        //セーブデータを初期化
        SceneManager.LoadScene("StageSelect");       
    }

    public void ContinueButton()
    {
        //セーブデータを参照しつづきから始めます

    }

    public void EndButton()
    {
        //ゲームを終わりにします
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else 
        Application.Quit();
#endif
    }
    #endregion
}

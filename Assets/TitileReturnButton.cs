using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトル戻るボタン
/// </summary>
public class TitileReturnButton : MonoBehaviour
{
    /// <summary>
    /// タイトルに戻るボタン
    /// </summary>
    public void TitleReturn()
    {
        if (ScreenTransition.Instance.State == SceneState.stageSelect)
        {
            SceneManager.LoadScene("Title");
        }
    }
}

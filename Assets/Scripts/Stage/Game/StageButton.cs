using UnityEngine;

/// <summary>
/// ステージのボタンを管理するクラス
/// </summary>
public class StageButton : MonoBehaviour
{
    /// <summary>
    /// メニューのボタン達
    /// </summary>
    [SerializeField] GameObject buttons;

    /// <summary>
    /// フロアを戻る時に表示するオブジェクト
    /// </summary>
    [SerializeField] GameObject warning;

    #region Mボタン
    
    /// <summary>
    /// Mボタンを押したときの処理 
    /// </summary>
    public void MButton()
    {
        //メニューのボタンを表示したり非表示したりします
        buttons.SetActive(!buttons.activeInHierarchy);
    }
    #endregion

    #region フロアを抜ける

    /// <summary>
    /// 警告メッセージを表示します
    /// </summary>
    public void ActiveWarning()
    {
        warning.SetActive(true);
        Time.timeScale = 0;
    }

    /// <summary>
    /// フロアを抜けないを選択の処理
    /// </summary>
    public void CloseWaring()
    {
        warning.SetActive(false);
        Time.timeScale = 1;
    }

    /// <summary>
    /// フロアを抜けるを選択の処理
    /// </summary>
    public void ReturnStageSelect()
    {
        //sceneステートを変更します
        ScreenTransition.Instance.ChangeState(SceneState.gameOverMode);

        //プレイヤーのクリア又は倒れるアニメーションが終わったら遷移を開始します
        ScreenTransition.Instance.TimeST(0);

        //ゲーム開始フラグを切ります
        GameManager.Instance.SetGame(false);
    }

    #endregion
}

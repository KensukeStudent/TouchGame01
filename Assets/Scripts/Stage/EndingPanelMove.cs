using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// エンディングパネルを動かすスクリプトコード
/// </summary>
public class EndingPanelMove : MonoBehaviour
{
    #region パネルの移動

    /// <summary>
    /// 移動させるパネル
    /// </summary>
    [SerializeField] RectTransform rt;

    /// <summary>
    ///　始まり位置
    /// </summary>
    const float start = -4707.577f;
    /// <summary>
    /// 終わり位置
    /// </summary>
    const float end = 3610.0f;

    /// <summary>
    /// 移動速度
    /// </summary>
    float speed;

    #endregion

    #region ボタン
    /// <summary>
    /// タイトルへ戻るボタン
    /// </summary>
    [SerializeField] GameObject button;
    /// <summary>
    /// ボタンを表示する時間計測
    /// </summary>
    float timer = 0;
    /// <summary>
    /// 開始から3秒後にボタンを表示します
    /// </summary>
    const float time = 3.0f;

    #endregion

    [SerializeField] Image outPanel;

    private void Start()
    {
        //パネルが60秒で移動する速度
        Init();
    }

    private void Update()
    {
        //パネルの移動
        Move();

        //ボタンの表示
        ActiveButton();
    }

    //MOVE

    /// <summary>
    /// 位置startから位置endまでの距離から60秒で移動する速度を求めます
    /// </summary>
    void Init()
    {
        speed = (end - start) / 60.0f;
    }

    /// <summary>
    /// 指定の位置まで移動します
    /// </summary>
    void Move()
    {
        var pos = rt.anchoredPosition;
        if (pos.y > end) return;

        //y軸に速度を足していきます
        pos.y += speed * Time.deltaTime;

        //目的位置未満ならそのまま位置を代入します
        if (pos.y < end)
        {
            rt.anchoredPosition = pos;
        }
        //目的位置以上なら目的位置座標を代入します
        else
        {
            pos.y = end;
            rt.anchoredPosition = pos;
        }
    }

    //BUTTON

    /// <summary>
    /// ボタンを表示します
    /// </summary>
    void ActiveButton()
    {
        if (timer > time) return;
        timer += Time.deltaTime;

        //指定時間になったらボタンを表示します
        if(timer > time)
        {
            button.SetActive(true);
        }
    }

    /// <summary>
    /// タイトルへ戻る
    /// </summary>
    public void ReturnTitle()
    {
        //ボタンを押せなくします
        var b = button.GetComponent<Button>();
        b.enabled = false;
        //画面を暗くします
        StartCoroutine(OutPanelCO());
    }

    /// <summary>
    /// 徐々に場面を暗くします
    /// </summary>
    /// <returns></returns>
    IEnumerator OutPanelCO()
    {
        outPanel.gameObject.SetActive(true);

        //outPanelのα値を徐々に上げます
        var c = outPanel.color;
        for (int i = 0; i < 100; i++)
        {
            c.a += 0.01f;
            yield return new WaitForSeconds(0.01f);
            outPanel.color = c;
        }

        //全て終わったらSceneを遷移します
        SceneManager.LoadScene("Title");
    }

}

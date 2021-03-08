using UnityEngine;

public class KeyPanel : MonoBehaviour
{
    /// <summary>
    /// PlayerInventryパネル
    /// MaxY: 135  MinY:-105
    /// </summary>
    RectTransform rect;
    /// <summary>
    /// 現在動作中
    /// </summary>
    bool move = false;
    /// <summary>
    /// true -> down,false -> up
    /// </summary>
    bool downOrUp = false;
    /// <summary>
    /// 鍵の取得数UIの最上位置
    /// </summary>
    const float maxPos = 135f;
    /// <summary>
    /// 鍵の取得数UIの最低位置
    /// </summary>
    const float minPos = -105f;


    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        //move = trueの時に自動で動きます
        PanelMove();
    }

    /// <summary>
    /// 鍵パネルを動作させます
    /// </summary>
    void PanelMove()
    {
        if (move)
        {
            var pos = rect.anchoredPosition;

            //down
            if (downOrUp)
            {
                pos.y -= Time.deltaTime * 1000;
                if (pos.y <= minPos)
                {
                    move = false;
                    pos.y = minPos;
                }
            }
            //up
            else
            {
                pos.y += Time.deltaTime * 1000;
                if (pos.y >= maxPos)
                {
                    move = false;
                    pos.y = maxPos;
                }
            }
            rect.anchoredPosition = pos;
        }
    }

    /// <summary>
    /// パネルの動きを切り替えます
    /// </summary>
    public void PanelSwitch()
    {
        move = true;

        //下に下がります
        if (rect.anchoredPosition.y <= minPos)
            downOrUp = false;
        //上に上がります
        else if (rect.anchoredPosition.y >= maxPos)
            downOrUp = true;
    }

}

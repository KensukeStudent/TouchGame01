using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーのHPを管理するクラス
/// </summary>
public class HeartManager : MonoBehaviour
{
    /// <summary>
    /// 1ならフルHP,0なら空HP画像に振り分けます
    /// </summary>
    int[] flag = { 1, 1, 1, 1, 1 };

    /// <summary>
    /// 変更するハート
    /// </summary>
    [SerializeField] Image[] hearts;

    /// <summary>
    /// ハートの画像
    /// </summary>
    [SerializeField] Sprite[] heartSprite;

    /// <summary>
    /// マウスが乗っている時の処理
    /// </summary>
    void OnMouse()
    {
        
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="amount">ダメージ回数</param>
    public void DamageHP(int amount)
    {
        //amount回ダメージを受けます
        for (int c = 0; c < amount; c++)
        {
            //flag = 1まで回します
            for (int i = flag.Length - 1; i > -1; i--)
            {
                if(flag[i] == 1)
                {
                    hearts[i].sprite = heartSprite[0];
                    flag[i] = 0;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 回復する
    /// </summary>
    /// <param name="amount">回復回数</param>
    public void IncreaseHP(int amount)
    {
        //amount回ダメージを回復します
        for (int c = 0; c < amount; c++)
        {
            //flag = 0まで回します
            for (int i = 0; i < flag.Length; i++)
            {
                if (flag[i] == 0)
                {
                    hearts[i].sprite = heartSprite[1];
                    flag[i] = 1;
                    break;
                }
            }
        }
    }
}

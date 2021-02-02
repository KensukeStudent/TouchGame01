using UnityEngine;

/// <summary>
/// プレイヤーのジャンプ先のオブジェクトクラス
/// </summary>
public class JumpObjItem : JumpObj
{
    /// <summary>
    /// 敵を倒せるアイテムを生成
    /// </summary>
    [SerializeField] GameObject attackItem;
    /// <summary>
    /// 生成するタイマー
    /// </summary>
    const float instantTime = 6.0f;
    float timer = 0;
    /// <summary>
    /// 現在アイテムが表示されているか
    /// </summary>
    GameObject currentItem;

    private void Update()
    {
        InstantItem();
    }

    /// <summary>
    /// アイテムを生成する
    /// </summary>
    void InstantItem()
    {
        if(!currentItem)
        {
            timer += Time.deltaTime;
            if(timer > instantTime)
            {
                //アイテム生成位置を指定
                var size = GetComponent<SpriteRenderer>().size;
                var item = Instantiate(attackItem, transform.localPosition, transform.rotation);

                currentItem = item;
                item.transform.SetParent(transform);

                var itemPos = item.transform.localPosition;
                itemPos.x = 0;
                itemPos.y = 0;
                item.transform.localPosition = itemPos;

                timer = 0;
            }
        }
    }
}

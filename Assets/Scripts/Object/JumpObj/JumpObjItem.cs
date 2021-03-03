using UnityEngine;

/// <summary>
/// プレイヤーのジャンプ先のオブジェクトクラス
/// </summary>
public class JumpObjItem : JumpObj
{
    /// <summary>
    /// 敵を倒せるアイテムを生成
    /// </summary>
    [SerializeField] GameObject item;
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
                //親位置に合わせたいので、localPositionでとります
                var item = Instantiate(this.item, transform.localPosition, transform.rotation);

                //アイテムが生成されていることをストックします
                currentItem = item;
                item.transform.SetParent(transform);

                //位置を初期化します
                var itemPos = item.transform.localPosition;
                itemPos.x = 0;
                itemPos.y = 0;
                item.transform.localPosition = itemPos;

                timer = 0;
            }
        }
    }
}

using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 爆弾スクリプト
/// </summary>
public class Bakudan : MonoBehaviour
{
    /// <summary>
    /// 速度
    /// </summary>
    float speed = 5;
    Rigidbody2D rb2d;

    /// <summary>
    /// 投げる方向
    /// </summary>
    Vector3 throwDirect;


    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        ThrowBakudan();
    }

    /// <summary>
    /// プレイヤーとマウスの角度方向に爆弾を投げます
    /// </summary>
    void ThrowBakudan()
    {
        rb2d.velocity = throwDirect * speed;
    }

    /// <summary>
    /// 投げる方向
    /// </summary>
    public void SetVec(Vector3 direct)
    {
        throwDirect = direct;
    }

    /// <summary>
    /// 爆弾イベントのノベルパート
    /// </summary>
    public string[] EvBakudan()
    {
        string[] bakudanNovel =
        {
            "むむっ！？",
            "なんにゃー？？このくだもの？？@P0@N",//爆弾の色を徐々に変えます,//自動で次の文へとぶ
            "にゃ！にゃ！？　ばくだんにゃーーーー！！",
            "@F1バクダンの解説に移ります。\n「ジャンプ位置でない所で左クリックすることで投げることができます」",//フォント変更
            "「敵を倒すこともできるので快感を味わってみてください。ただし、自分もまき込まれることもあるのでお気をつけて。」",
            "バクダンを持っている時は、攻撃できないので敵とあったらすぐに投げることをおすすめします。",
            "@P1@D"//徐々にNovelFrameを消します
        };

        return bakudanNovel;
    }

    /// <summary>
    /// テキスト内で動作させる関数をストックします
    /// </summary>
    /// <returns></returns>
    public Action[] Actions()
    {
        Action[] actions = { EventAnim , EventDelete };
        return actions;
    } 

    /// <summary>
    /// テキスト内でアニメーションさせます
    /// </summary>
    void EventAnim()
    {
        //テキスト表示を停止します
        TextManager.StartStop(1);

        StartCoroutine(EventAnimCO());
    }

    /// <summary>
    /// イベント時に流すアニメーション
    /// </summary>
    IEnumerator EventAnimCO()
    {
        var sprite = GetComponent<SpriteRenderer>();

        var color = sprite.color;

        for (int i = 0; i < 100; i++)
        {
            color.r += 0.01f;
            color.g += 0.01f;
            color.b += 0.01f;

            yield return new WaitForSecondsRealtime(0.01f);
            sprite.color = color;
        }

        //テキスト表示を再開始します
        TextManager.StartStop(0);
    }

    /// <summary>
    /// テキスト内で削除します
    /// </summary>
    void EventDelete()
    {
        //全てが終わったら爆弾を削除して、プレイヤーの頭の上に爆弾を表示します
        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerContoller>();
        player.ActiveBakudan();
        Destroy(gameObject);
    }
}

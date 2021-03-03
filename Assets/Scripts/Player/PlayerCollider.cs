using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// プレイヤーの当たりを処理するクラス
/// </summary>
public partial class PlayerController : MonoBehaviour
{
    #region 当たり判定

    private void OnTriggerEnter2D(Collider2D col)
    {
        //敵に当たったら処理
        if (col.gameObject.CompareTag("Enemy")) HitEnemy(col);

        //敵弾に当たったら処理
        if (col.gameObject.CompareTag("EnemyShot")) HitEnemyShot();

        //アイテムを取得し解析します
        if (col.CompareTag("Item")) GetItemSwitch(col);

        //ゴールのアイテムをとったらゲームクリア
        if (col.gameObject.CompareTag("Goal")) GoalClear(col);
    }

    /// <summary>
    /// 敵に衝突
    /// </summary>
    void HitEnemy(Collider2D col)
    {
        var enemy = col.GetComponent<Enemy>();

        //Enemyコンポーネントがないなら処理しません
        if (!enemy) return;

        //攻撃モードの時に敵を破壊できます
        if (AttackMode && !bakudan.activeInHierarchy)
        {
            //攻撃アニメーション
            anim.SetTrigger("Attack");

            //当たった敵に死亡フラグを立てます
            enemy.SetDie();

            //攻撃モード解除
            var sprite = GetComponent<SpriteRenderer>();
            sprite.color = Color.white;
            AttackMode = false;
        }
        else if (!AttackMode)
        {
            Debug.Log("ダメージを受ける");

            if (bakudan.activeInHierarchy)
            {
                //即死
                Debug.Log("バクダンが破裂します");

                //エフェクトを生成します
                var b = bakudan.GetComponent<InstantEffect>();
                b.EffectInstant(transform.position);
            }
        }
    }

    /// <summary>
    /// 敵弾に衝突
    /// </summary>
    void HitEnemyShot()
    {
        Debug.Log("ダメージを受ける");
        PlaySE(3);
    }

    /// <summary>
    /// あるアイテムに触れたら処理します
    /// </summary>
    void GetItemSwitch(Collider2D col)
    {
        //解析したアイテム名
        switch (ItemName(col.name))
        {
            //攻撃
            case "Food":
                //敵を倒せるようになります
                NowAttackMode(col);
                break;

            //鍵
            case "Key":
                //鍵を取得　
                //ステージ内の特定の場所へいけるようになります
                HaveKey(col);
                break;

            //道具
            case "Bakudan":
                //爆弾に触れた時に処理します
                Bakudan(col);
                break;
        }
    }

    /// <summary>
    /// 名前を解析します
    /// </summary>
    /// <param name="name">アイテム名</param>
    string ItemName(string name)
    {
        return _ = Regex.Match(name, @"(.+)_").Groups[1].Value;
    }

    /// <summary>
    /// 現在アタックモード
    /// </summary>
    void NowAttackMode(Collider2D col)
    {
        //攻撃モードになります
        AttackMode = true;
        //取得したアイテムを削除します
        Destroy(col.gameObject);
        //攻撃モードの色に変えます
        var sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(1, 0.6f, 0, 1);
        //SEを鳴らします
        PlaySE(1);
    }

    /// <summary>
    /// 鍵を取得
    /// </summary>
    void HaveKey(Collider2D col)
    {
        //鍵の種類に応じてカウントを所字数を上げます
        pi.GetKindKey(col.gameObject.name);

        //サウンドを鳴らします
        PlaySE(2);

        //取得した鍵を削除します
        Destroy(col.gameObject);
    }

    /// <summary>
    /// ゴール
    /// </summary>
    void GoalClear(Collider2D col)
    {
        anim.SetBool("Goal", true);

        //もし攻撃モードだった場合のため色を変更処理します
        var sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.white;

        //ステートを変更します
        currentState = State.goal;

        //取得したゴールオブジェクトを削除します
        Destroy(col.gameObject);

        //ステージの状態を更新します
        var sm = GameObject.Find("StageManager").GetComponent<StageManager>();
        //ジャンプした数を評価します(----> 各ステージのスコアに反映されます)
        sm.StageUpdate(GameManager.Instance.StageNo, jumpCount);

        //シーンを遷移します
        //Goalアニメーション時間分のタイムラグを与えてから遷移
        sm.GameEnd(anim, "Goal");
    }

    /// <summary>
    /// 爆弾がイベントものかどうかで処理を分けます
    /// </summary>
    /// <param name="name">爆弾名</param>
    void Bakudan(Collider2D col)
    {
        //爆弾の状態
        //nomalかev
        var condition = Regex.Match(col.name, @"_(.+)\(Clone\)").Groups[1].Value;

        switch (condition)
        {
            //通常の爆弾
            case "Nomal":
                //bakudanを表示
                //投げること可能
                TakeBakudan(col.gameObject);
                break;

            //イベント式の爆弾
            case "Ev":
                //イベント開始
                BakudanEvent(col);
                break;
        }
    }

    /// <summary>
    /// 爆弾を取得したときの処理
    /// </summary>
    void TakeBakudan(GameObject b)
    {
        //bakudanをプレイヤーの頭の上に表示します
        bakudan.SetActive(true);
        //爆弾を削除します
        Destroy(b);
    }

    /// <summary>
    /// 爆弾イベントの時一回限り呼ばれます
    /// </summary>
    void BakudanEvent(Collider2D col)
    {
        #region 処理流れ

        //黒いeventnの爆弾を取る

        //イベントパート
        //fontの変更
        //----> 「なんにゃー、このくだもの？？」(ネコのセリフ)

        //テキストの方でWaitを書けます(animationの時間分)
        // 爆弾の色を徐々に元の色に戻していきます

        //----> 「にゃ！にゃ！？　ばくだんにゃーーー！！！」

        //解説
        //---> fontを変更
        //----> 「ジャンプ位置でない所で右クリックすることで投げることができます」
        //----> 「敵を倒すこともできるので快感を味わってみてください。
        //        ただし、自分もまき壊れることもあるのでお気をつけて。」
        //----> 「爆弾は何度も生成されるので、何回でも使うことができます」

        #endregion

        //タイムスケールで時間を止めます
        Time.timeScale = 0;

        //表示用テキストCanvas
        var cavas = GameObject.Find("DescriptionCanvas");
        //親から表示用テキストを取得
        var tm = cavas.transform.Find("NovelFrame").GetComponent<TextManager>();
        //Ev爆弾のノベルパートを取得
        var b = col.GetComponent<BakudanEv>();

        //読み込むテキストを表示用UIの方に格納します
        tm.SetEvText(b.EvBakudan());

        //NovelFrameを徐々に表示します
        tm.GetImage();

        tm.SetAction(b.Actions());
    }

    /// <summary>
    /// イベント用で爆弾を表示します
    /// </summary>
    public void ActiveBakudan()
    {
        bakudan.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        //特定の鍵を持っている場合、それに応じたブロックを破壊できます
        if (col.gameObject.CompareTag("Block"))
        {
            if (!touch)
            {
                touch = true;
                actionEv.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        //イベントブロックと離れた
        if (col.gameObject.CompareTag("Block"))
        {
            touch = false;
            actionEv.SetActive(false);

            var bS = col.GetComponent<BlocksScript>();
            bS.InActive();
        }
    }

    #endregion
}

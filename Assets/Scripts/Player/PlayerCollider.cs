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
        if (col.gameObject.CompareTag("EnemyShot")) HitEnemyShot(col);

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
        //ただしバクダンを所持していない時のみ
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
            if (bakudan.activeInHierarchy)
            {
                //エフェクトを生成します
                var b = bakudan.GetComponent<InstantEffect>();
                b.EffectInstant(transform.position);

                //敵のダメージを受けます
                Damage(5);
                return;
            }

            //敵のダメージを受けます
            Damage(enemy.Damage());
        }
    }

    /// <summary>
    /// 敵弾に衝突
    /// </summary>
    void HitEnemyShot(Collider2D col)
    {
        var es = col.GetComponent<EnemyShot>();

        //敵弾のダメージを受けます     
        Damage(es.Damage());
    }

    /// <summary>
    /// あるアイテムに触れたら処理します
    /// </summary>
    void GetItemSwitch(Collider2D col)
    {
        //解析したアイテム名
        switch (RE.GetName(col.name))
        {
            //攻撃
            case "Food":
                //食べたもの二応じて処理を変えます
                FoodSwitch(col);
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
    /// 場合分けした食べ物アイテムを処理します
    /// </summary>
    void FoodSwitch(Collider2D col)
    {
        switch (RE.NameAnalysis(col.name))
        {
            //攻撃可能アイテム
            case "AttackItem":
                GetAttackMode(col);
                break;

            case "Heart":
                GetHeart(col);
                break;
        }
    }

    /// <summary>
    /// 現在アタックモード
    /// </summary>
    void GetAttackMode(Collider2D col)
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
    /// ハートを取得したときに処理します
    /// </summary>
    void GetHeart(Collider2D col)
    {
        //体力を回復します
        pi.Hm.IncreaseHP(1);

        //取得したオブジェクトを破壊します
        Destroy(col.gameObject);

        //SEを鳴らします
        PlaySE(4);
    }

    /// <summary>
    /// 鍵を取得
    /// </summary>
    void HaveKey(Collider2D col)
    {
        //鍵の種類に応じてカウントの所字数を上げます
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
        currentState = PlayerState.goal;

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
        switch (RE.NameAnalysis(col.name))
        {
            //通常の爆弾
            case "Nomal":
                //bakudanを表示
                //投げること可能
                TakeBakudan(col.gameObject);
                break;

            //イベント式の爆弾
            case "Ev":
                //Ev爆弾のノベルパートを取得
                var b = col.GetComponent<BakudanEv>();

                //イベント開始
                ADVSystem.StartADV(b.ADVPart(), b.Actions());
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
    /// イベント用で爆弾を表示します
    /// </summary>
    public void ActiveBakudan()
    {
        bakudan.SetActive(true);
    }

    /// <summary>
    /// ダメージを受けたら処理します
    /// </summary>
    void Damage(int amount)
    {
        //ダメージSEを鳴らします
        PlaySE(3);

        //敵のダメージを受けます
        if (pi.Hm.DamageHP(amount) <= 0)
        {
            //0以下になったら負け
            DiePlayer();
        }
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

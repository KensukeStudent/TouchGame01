using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
#pragma warning disable 649

#region Kind詳細
//nomal--->通常
//ev   --->特定の敵を全部倒せ！@evPの時はListの中に入れて何かの難易度を上げる
//どくろshotの場合弾の速度を上昇(Listの中身がなくなるごとに速度が減少)
#endregion

/// <summary>
/// 敵の種類
/// </summary>
public enum EnemyKind
{
    nomal,
    ev
}

/// <summary>
/// 敵クラス
/// </summary>
public class Enemy : MonoBehaviour
{
    /// <summary>
    /// 敵がイベント持ちか
    /// </summary>
    public EnemyKind CurrentKind { private set; get; }

    [SerializeField] GameObject explosion;

    /// <summary>
    /// 倒せる敵かどうか
    /// </summary>
    public bool Defeat { private set; get; }

    /// <summary>
    /// 親の時に子の敵を格納します
    /// </summary>
    /// 名前にPがついていれば親の敵
    List<GameObject> dMan = new List<GameObject>();
    /// <summary>
    /// 管理する親が存在
    /// </summary>
    protected Enemy MyP { private set; get; }

    /// <summary>
    /// 死亡フラグ
    /// </summary>
    public bool Die { private set; get; } = false;

    /// <summary>
    ///破壊されたときに呼ばれる関数 
    /// </summary>
    public void Explosion()
    {
        //この敵がイベント敵なら処理します
        if(CurrentKind == EnemyKind.ev)
        {
            //イベントの敵が倒されたら開くゴールオブジェクトをFindします
            var goalName = "Goal_" + Regex.Match(name, @"\d+").ToString();
            var goal = GameObject.Find(goalName).GetComponent<GoalBlock>();

            //この敵が倒されたフラグを入れます
            goal.EnemyGoal(gameObject);
        }
        Instantiate(explosion, transform.position, Quaternion.identity);

        //自分を管理しているフロアから自分を削除します
        var floor = transform.root.GetComponent<Floor>();
        floor.RemoveObj(gameObject);

        Destroy(gameObject);
    }

    /// <summary>
    /// 管理される親がいる場合処理します
    /// </summary>
    protected void HaveParent()
    {
        //親に子の敵(この敵)が倒されたフラグを入れます
        MyP.dMan.Remove(gameObject);
    }

    /// <summary>
    /// 親に格納されている子が倒されたときに処理します
    /// <code>この処理はそれぞれの敵によって違います</code>
    /// </summary>
    protected virtual void EventParent()
    {
        Debug.Log("それぞれの敵に応じて処理内容が違います");
    }

    /// <summary>
    /// 死亡フラグをつけます
    /// </summary>
    public void SetDie()
    {
        Die = true;
    }

    /// <summary>
    /// 敵の種類を入れます
    /// </summary>
    public void SetEnemyKind(EnemyKind kind)
    {
        CurrentKind = kind;
    }

    /// <summary>
    /// この敵は倒せる敵かを初期値で決めます
    /// </summary>
    protected void DefeatThisEnemy(bool b)
    {
        Defeat = b;
    }

    /// <summary>
    /// 親の敵
    /// 格納された子の敵数に応じて何かの減少処理がされます
    /// </summary>
    protected void SetChildEnemy()
    {
        var enem = GameObject.FindGameObjectsWithTag("Enemy");
        //親取得(同じフロア内の敵を検知)
        var root = transform.parent;
        //名前
        var cName = Regex.Replace(name, @"P", "", RegexOptions.Singleline);
        //名前から取得

        //Dokuro1---->名前+event番号
        foreach (var c in enem)
        {
            //部屋の番号を取得します
            var rootC = c.transform.root;
            //親が同じ且つ指定の名前
            if (rootC.name == root.name && c.name == cName)
            {
                dMan.Add(c);
                var e = c.GetComponent<Enemy>();
                //子に親(自分)を指定
                e.MyP = GetComponent<Enemy>();
            }
        }
    }
}

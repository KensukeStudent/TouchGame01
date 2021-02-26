using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゴールの種類
/// </summary>
public enum goalKind
{
    enemy,
    key
}

/// <summary>
/// イベントのブロック
/// </summary>
public class GoalBlock : BlocksScript
{
    /// <summary>
    /// ゴール条件
    /// </summary>
    goalKind currentCondition;

    /// <summary>
    /// 特定の敵をすべて倒したらゴールブロックが破壊されます
    /// </summary>
    public List<GameObject> EvEnemy { private set; get; } = new List<GameObject>();

    /// <summary>
    /// 特定の鍵を入手していたらゴールブロックが破壊されます
    /// </summary>
    public GameObject EvKey { private set; get; }


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        //ゴール条件のオブジェクトを取得します。
        GoalCondition();
    }

    /// <summary>
    /// ゴールの条件に合わせた処理をします
    /// </summary>
    void GoalCondition()
    {
        switch (currentCondition)
        {
            case goalKind.enemy:
                //イベントの敵を入れます
                var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                //親取得
                var root = transform.root;
                    
                foreach (var enemy in enemies)
                {
                    //敵を管理しているフロア番号を取得
                    var rootE = enemy.transform.root;

                    //番号が同じであれば
                    if(root == rootE)
                    {
                        var eS = enemy.GetComponent<Enemy>();
                        // 敵もイベント属性 且つ イベントのナンバーが同じ
                        if (eS.CurrentKind == EnemyKind.ev && EventNumber(eS.name) == EventNumber(name)) 
                            //イベントの敵を格納します
                            EvEnemy.Add(enemy);
                    }
                }
                break;
           
            case goalKind.key:
                //破壊することのできる鍵を取得します
                EvKey = GameObject.FindGameObjectWithTag("EventKey");
                break;
        }
    }

    /// <summary>
    /// 初期値でゴールの種類を指定します
    /// </summary>
    /// <param name="kind"></param>
    public void GoalKind(goalKind kind)
    {
        currentCondition = kind;
    }

    /// <summary>
    /// イベント敵の場合ゴールのリスト数を確認します
    /// </summary>
    public void EnemyGoal(GameObject g)
    {
        //格納している敵をリストから消します
        EvEnemy.Remove(g);

        //最後の敵だったらゴールブロックを破壊
        if (currentCondition == goalKind.enemy && EvEnemy.Count == 0)
            Destroy();
    }
}

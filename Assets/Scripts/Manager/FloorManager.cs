using UnityEngine;

/// <summary>
/// 全てのフロアを管理します
/// </summary>
public class FloorManager : MonoBehaviour
{
    /// <summary>
    /// 全フロアを格納
    /// </summary>
    public Floor[,] Floors { private set; get; }

    //↑二次元配列で格納する必要あり。
    //移動で下に移動したときにオブジェクトが非表示にされない可能性あり

    /// <summary>
    /// そのステージでのPlayerが生成されたフロア番号
    /// </summary>
    public int PlayerFloor { private set; get; } = 0;

    void Start()
    {
        var stageX = StageCreator.StageX;
        //ステージのフロア数を取得
        SetFloors(stageX);
        //プレイヤーが現在いるフロア以外非表示にする
        FloorAllInActive();
    }

    /// <summary>
    /// 各フロアのオブジェクトを取得
    /// </summary>
    void SetFloors(int stageX)
    {
        //現在のフロア分取得
        var floorObj = GameObject.FindGameObjectsWithTag("Floor");
        //ステージの高さ
        var stageH = StageCreator.FloorCount / stageX;
        //各フロア管理スクリプト取得
        Floors = new Floor[stageH, stageX];

        //二次元のFloorsにステージにあるフロアを管理してもらいます。
        for (int i = 0; i < floorObj.Length; i++)
        {
            //縦の番地
            var h = i / stageX;
            //横の番地
            var w = i % stageX;

            Floors[h, w] = floorObj[i].GetComponent<Floor>();
        }

        //各フロアへ取得したオブジェクトを割り当てます
        GetObjToFloors(stageX);
    }

    /// <summary>
    /// 取得したオブジェクトを指定の親のリストへ入れます
    /// </summary>
    void GetObjToFloors(int stageX)
    {
        string[] tags = { "Enemy" };

        for (int i = 0; i < tags.Length; i++)
        {
            //指定のオブジェクトを取得
            var go = GameObject.FindGameObjectsWithTag(tags[i]);
            for (int j = 0; j < go.Length; j++)
            {
                //指定の親のフロア番号へオブジェクトを割り当てます
                var parentNo = int.Parse(go[j].transform.parent.name);

                //親のナンバーから二次元配列の位置を求めます
                //ステージの高さ
                var stageH = parentNo / stageX;  //フロアが1から始まるので - 1を先にします
                var stageW = parentNo % stageX;

                Floors[stageH, stageW].SetFloorChildObj(go[j]);
            }
        }
    }

    /// <summary>
    /// プレイヤーがいるフロア以外Floorに格納されているオブジェクトを非表示にする
    /// </summary>
    void FloorAllInActive()
    {
        //配列0番目の要素分回します
        for (int i = 0; i < Floors.GetLength(0); i++)
            
            //配列1番目の要素分回します
            for (int j = 0; j < Floors.GetLength(1); j++)

                //Floors[i,j]のFloorObjにある要素分回します
                for (int k = 0; k < Floors[i, j].FloorObj.Count; k++)
                {
                    if (PlayerFloor != j)
                        Floors[i, j].FloorObj[k].SetActive(false);
                }
    }

    /// <summary>
    /// プレイヤーのいるフロア番号を獲得
    /// </summary>
    /// <param name="fn">文字型の数字</param>
    public void SetPlayerFloor(string fn)
    {
        PlayerFloor = int.Parse(fn);
    }
}

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
    int playerFloor = 0;

    void Start()
    {
        var c = GameObject.Find("StageCreator").GetComponent<StageCreator>();
        //ステージのフロア数を取得
        SetFloors(c);
        //プレイヤーが現在いるフロア以外非表示にする
        FloorAllInActive();
    }

    /// <summary>
    /// 各フロアのオブジェクトを取得
    /// </summary>
    void SetFloors(StageCreator c)
    {
        //現在のフロア分取得
        var floorObj = GameObject.FindGameObjectsWithTag("Floor");
        //ステージの高さ
        var stageH = c.FloorCount % c.StageX + 1;//配列の要素を必ず1開けるため +1 します
        //各フロア管理スクリプト取得
        Floors = new Floor[stageH,c.StageX];

        for (int i = 0; i < Floors.GetLength(0); i++)
        {
            for (int j = 0; j < Floors.GetLength(1); j++)
            {
                Floors[i,j] = floorObj[j].GetComponent<Floor>();
            }   
        }

        //各フロアへ取得したオブジェクトを割り当てます
        GetObjToFloors(c);
    }

    /// <summary>
    /// 取得したオブジェクトを指定の親のリストへ入れます
    /// </summary>
    void GetObjToFloors(StageCreator c)
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
                var stageH = parentNo % c.StageX - 1;  //フロアが1から始まるので - 1を先にします
                var stageW = parentNo % c.StageX;

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
                    if (playerFloor != j)
                        Floors[i, j].FloorObj[k].SetActive(false);
                }
    }

    /// <summary>
    /// プレイヤーのいるフロア番号を獲得
    /// </summary>
    /// <param name="fn"></param>
    public void SetPlayerFloor(string fn)
    {
        playerFloor = int.Parse(fn);
    }
}

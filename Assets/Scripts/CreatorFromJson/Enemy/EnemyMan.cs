using System;
using System.Text.RegularExpressions;

/// <summary>
/// Jsonから参照した値を敵につけるクラス
/// </summary>
public class EnemyMan
{
    /// <summary>
    /// どくろ(弾)
    /// </summary>
    public static void DokuroShot(DokuroShot dokuro, DokuroEnemy list,int floorNum, int num)
    {
        var d = list.floor[floorNum];
        var s = d.shot[0];
        var count = num;
        var name = s.name;

        //Jsonのデータを入れる
        dokuro.SetShotStates(d, s, count, name);
    } 

    /// <summary>
    /// どくろの弾を飛ばす方向を解析
    /// </summary>
    public static bool[] SetDirect(D_Shot s,int count)
    {
        //初期値として右上左下方向のブール値を入れます
        var pos = new bool[4];

        //文字を指定
        string[] array = { "R", "U", "L", "D" };
        var direct = s.shotPos[count];
        MatchCollection m = Regex.Matches(direct,@"[A-Z]");

        //R->右,U->上,L->左,D->下
        //の文字があればブール値をtrueにする
        foreach (var data in m)
        {
            int ret = Array.IndexOf(array, data.ToString());
            pos[ret] = true;
        }
        return pos;
    }
}

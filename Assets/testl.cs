using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class testl : MonoBehaviour
{
    [SerializeField] GameObject child;

    private void Start()
    {
        var c = child.transform;
        var list = new List<Transform>();

        do
        {
            var parent = c.transform.parent;

            if (!Num(parent.name))
            {
                list.Add(parent);
                c = parent;
            }
            else break;

        } while (true);
    }


    /// <summary>
    /// 数値のみの取得
    /// </summary>
    /// <param name="scr"></param>
    /// <returns></returns>
    bool Num(string s)
    {
        //数値でない文字があるかを判別それを反転させて数値のみかを取得
        return _ = !Regex.IsMatch(s, @"[^0-9]");
    }
}

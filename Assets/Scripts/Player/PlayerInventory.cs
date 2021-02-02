using UnityEngine;
using System.Text.RegularExpressions;
using System;
using TMPro;

/// <summary>
/// プレイヤーの状態を管理するクラス
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    //鍵
    int[] keysCounter = { 0, 0, 0, 0 };

    /// <summary>
    /// 鍵の種類に応じて取得値を増やします
    /// </summary>
    /// <param name="keyName"></param>
    public void GetKindKey(string keyName)
    {
        string[] keysName = { "Blue", "Green", "Red", "Yellow"};
        var ret = GetNumber(keysName,NameAnalysis(keyName));
        //取得した種類の鍵の値を増やします。
        keysCounter[ret]++;
        GetTextUI(keysName[ret], keysCounter[ret]);
    }

    /// <summary>
    /// 鍵の種類に応じて取得値を減らします
    /// </summary>
    /// <param name="BlockName"></param>
    public void UseKindKey(string blockName,GameObject block)
    {
        var bS = block.GetComponent<BlocksScript>();
        string[] blocksName = { "Blue", "Green", "Red", "Yellow" ,"Goal"};
        var ret = GetNumber(blocksName, NameAnalysis(blockName));

        switch (blocksName[ret])
        {
            case "Blue":
            case "Green":
            case "Red":
            case "Yellow":
                if (keysCounter[ret] > 0)
                {
                    keysCounter[ret]--;
                    GetTextUI(blocksName[ret], keysCounter[ret]);
                    bS.Destroy();
                }
                else bS.SetText();
                break;

            case "Goal":
                bS.SetText();
                break;
        }
    }

    /// <summary>
    /// 名前の解析
    /// </summary>
    string NameAnalysis(string name)
    {
        return _= Regex.Match(name, @"([A-Z]|[a-z])+").ToString();
    }

    /// <summary>
    /// 指定の色が何番目のリストに存在するかを求めます
    /// </summary>
    /// <param name="array">配列</param>
    /// <param name="name">名前</param>
    /// <returns></returns>
    int GetNumber(string[] array,string name)
    {
        return _ = Array.IndexOf(array, name);
    }

    /// <summary>
    /// 指定のオブジェクトの数値を変更します
    /// </summary>
    /// <param name="name">オブジェクト名</param>
    /// <param name="count">オブジェクトの所持数</param>
    void GetTextUI(string name,int count)
    {
        string path = string.Format("Key{0}Cont", name);
        var keyText = GameObject.Find(path).GetComponent<TMP_Text>();
        keyText.text = "×" + count;
    }
}

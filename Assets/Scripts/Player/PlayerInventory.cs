using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// プレイヤーのアイテムを管理するクラス
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    #region 配列内の鍵情報

    //0 -----> blueKey
    //1 -----> greenKey
    //2 -----> redKey
    //3 -----> yellowKey

    #endregion

    //鍵
    int[] keysCounter = { 0, 0, 0, 0 };

    /// <summary>
    /// ゴール鍵の番号
    /// </summary>
    List<string> goalKey = new List<string>();

    /// <summary>
    /// 鍵の種類に応じて取得値を増やします
    /// </summary>
    /// <param name="keyName"></param>
    public void GetKindKey(string keyName)
    {
        //鍵名
        string[] keysName = { "Blue", "Green", "Red", "Yellow", "Goal" };

        //GoalKeyの場合は鍵名をリストに入れてreturnします
        if (NameAnalysis(keyName) == keysName[4])
        {
            goalKey.Add(keyName);
            return;
        }

        //配列の何番目の名前かを数字で返値で出します
        //配列名にすることでkeysCounterと連携します
        var ret = GetArrayNumber(keysName,NameAnalysis(keyName));
        
        //取得した種類の鍵の値を増やします。
        keysCounter[ret]++;
        //Textに鍵の数を表示します
        GetTextUI(keysName[ret], keysCounter[ret]);
    }

    /// <summary>
    /// 鍵の種類に応じて取得値を減らします
    /// </summary>
    /// <param name="blockName"></param>
    public void UseKindKey(GameObject block)
    {
        //現在鍵を使って破壊するブロック
        var bS = block.GetComponent<BlocksScript>();
        //ブロック名
        string[] blocksName = { "Blue", "Green", "Red", "Yellow", "Goal" ,"default" };
        //配列の何番目の文字かを返値で番号を返します
        var ret = GetArrayNumber(blocksName, block.name);

        switch (blocksName[ret])
        {
            case "Blue":
            case "Green":
            case "Red":
            case "Yellow":
                if (keysCounter[ret] > 0)
                {
                    //使った鍵を減らします
                    keysCounter[ret]--;
                    //Text表示を減らし更新します
                    GetTextUI(blocksName[ret], keysCounter[ret]);
                    //ブロックを破壊します
                    bS.Destroy();
                }
                else 
                    //鍵を持っていない(条件を達していない)場合は
                    //ヒントテキスト表示します
                    bS.SetText();

                break;

            //ゴールブロックの時
            case "Goal":
                //指定のゴールブロックと同じ名前の鍵があれば破壊します
                GoalBlock(bS);
                break;

            //通常のブロックではない場合
            #region 例外ブロック
            //イベント付きのブロック
            //爆破で突破ブロック
            #endregion

            case "default":
                //ゴールブロックを破壊するためのヒントを表示します
                bS.SetText();
                
                break;
        }
    }

    /// <summary>
    /// 名前の解析
    /// プレイヤ―が取得するアイテム
    /// </summary>
    string NameAnalysis(string name)
    {
        //大文字、小文字のローマ字を抽出します
        return _= Regex.Match(name, @"_(.+)(\(Clone\)|[0-9])").Groups[1].Value;
    }

    /// <summary>
    /// 指定の色が何番目のリストに存在するかを求めます
    /// </summary>
    /// <param name="array">配列</param>
    /// <param name="name">名前</param>
    /// <returns></returns>
    int GetArrayNumber(string[] array,string name)
    {
        //配列内の番号を出力します
        var no = Array.IndexOf(array, name);

        if (no < 0) return _= 4;

        return no;
    }

    /// <summary>
    /// 指定のオブジェクトの数値を変更します
    /// </summary>
    /// <param name="name">オブジェクト名</param>
    /// <param name="count">オブジェクトの所持数</param>
    void GetTextUI(string name,int count)
    {
        //更新するTextコンポーネントを取得するためのパスを指定
        string path = string.Format("Key{0}Cont", name);
        var keyText = GameObject.Find(path).GetComponent<TMP_Text>();
        //この形で文字を出力します
        keyText.text = "×" + count;
    }

    /// <summary>
    /// ゴールブロック名とゴール鍵名が一致していたらブロックを破壊します
    /// </summary>
    void GoalBlock(BlocksScript block)
    {
        //ゴールブロックのフロア番号を取得します
        var keyName = "Key_Goal" + GetNo(block.name);

        //goalKeyのリストに同じ名前があるかを判定します
        var listNum = goalKey.IndexOf(keyName);

        //あればブロックを破壊します
        if (listNum >= 0)
        {
            goalKey.Remove(goalKey[listNum]);

            block.Destroy();
        }
        //なければテキストを表示します
        else
        {
            block.SetText();
        }
    }

    /// <summary>
    /// 番号を取得します
    /// </summary>
    string GetNo(string name)
    {
        //数値だけ取得
        return _ = Regex.Match(name, @"[0-9]+").ToString();
    }
}

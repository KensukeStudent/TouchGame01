using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

/// <summary>
/// 正規表現を使って解析するクラス
/// </summary>
public class RE : MonoBehaviour
{
    /// <summary>
    /// アンダーバー前までの文字を取得します
    /// </summary>
    /// <param name="name">アンダーバー含む文字</param>
    public static string GetName(string name)
    {
        return _ = Regex.Match(name, @"(.+)_").Groups[1].Value;
    }

    /// <summary>
    /// 「(」と「[0-9]」を含まないグループを取得
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string NameAnalysis(string name)
    {
        //大文字、小文字のローマ字を抽出します
        return _ = Regex.Match(name, @"_([^(0-9]+)").Groups[1].Value;
    }

    /// <summary>
    /// 番号を取得します
    /// </summary>
    /// <returns></returns>
    public static string GetNo(string name)
    {
        //数値だけ取得
        return _ = Regex.Match(name, @"[0-9]+").ToString();
    }
}

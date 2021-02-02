using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
#pragma warning disable 649

/// <summary>
/// このブロックが持っている情報をプレイヤーへ伝えるクラス
/// </summary>
public class BlocksScript : MonoBehaviour
{
    TMP_Text nameText;
    TMP_Text senText;
    GameObject canvas;

    /// <summary>
    /// このブロック役割をプレイヤーに知らせます
    /// </summary>
    string role = "";
    /// <summary>
    /// ブロック名
    /// </summary>
    string name = "";

    /// <summary>
    /// 次のフロアの足場を展開
    /// </summary>
    bool floorSet = false;

    /// <summary>
    /// 次の面に移動できる足場
    /// </summary>
    GameObject meatObj;
    /// <summary>
    /// 移動方向を示す猫
    /// </summary>
    GameObject catArrow;

    protected virtual void Awake()
    {
        nameText = GameObject.Find("Name").GetComponent<TMP_Text>();
        senText = GameObject.Find("Text").GetComponent<TMP_Text>();

        canvas = GameObject.Find("DescriptionCanvas");
    }

    /// <summary>
    /// 初期値で別ファイルで書いた説明を入れます
    /// </summary>
    /// <param name="description">ブロック説明</param>
    /// <param name="parent">指定親オブジェクト</param>
    public void SetBlock(string description,string bName,string setFloor)
    {
        var name = Regex.Match(description, @"^@(.+){").Groups[1].Value;
        var role = Regex.Match(description, @"{(.+)}").Groups[1].Value;

        this.role = role;
        this.name = name;
        gameObject.name = bName;

        if (string.IsNullOrEmpty(setFloor)) return;
        //指定の親オブジェクト下から

        //パスの指定
        var pathJ = string.Format("Jump_{0}", setFloor);
        var pathA = string.Format("Arrow_{0}", setFloor);
        var root = transform.root;
        //オブジェクトの検索
        var floorJump = root.transform.Find(pathJ);
        var floorArrow = root.transform.Find(pathA);
        //オブジェクトの代入
        meatObj = floorJump.gameObject;
        catArrow = floorArrow.gameObject;

        floorSet = true;
    }

    /// <summary>
    /// このブロックの役割を表示します
    /// </summary>
    public void SetText()
    {
        canvas.SetActive(true);
        nameText.text = name;
        senText.text = role;
    }

    /// <summary>
    /// Canvasを非表示にします
    /// </summary>
    public void InActive()
    {
        canvas.SetActive(false);
    }

    /// <summary>
    /// 次のフロアへ行く足場を展開
    /// </summary>
    public void ActiveFloor()
    {
        meatObj.SetActive(true);
        catArrow.SetActive(true);
    }

    public void Destroy()
    {
        if (floorSet) ActiveFloor();
        Destroy(gameObject);
    }

    /// <summary>
    /// 自分の名前についているイベント番号を取得
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    protected string EventNumber(string name)
    {
        return _= Regex.Match(name, @"\d+").ToString();
    }
}

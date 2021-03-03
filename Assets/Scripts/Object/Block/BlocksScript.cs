using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
#pragma warning disable 649

/// <summary>
/// このブロックが持っている情報をプレイヤーへ伝えるクラス
/// </summary>
public class BlocksScript : MonoBehaviour
{
    /// <summary>
    /// textを持つ親
    /// </summary>
    GameObject back;
    TMP_Text nameText;
    TMP_Text senText;

    string roleName = "";
    /// <summary>
    /// このブロック役割をプレイヤーに知らせます
    /// </summary>
    string role = "";

    /// <summary>
    /// 次のフロアの足場を展開
    /// </summary>
    string floorSet = "";

    /// <summary>
    /// 次の面に移動できる足場
    /// </summary>
    GameObject meatObj;
    /// <summary>
    /// 移動方向を示す猫
    /// </summary>
    GameObject catArrow;

    AudioSource aud;
    [SerializeField]AudioClip clip;

    protected virtual void Awake()
    {
        //順を追って探します
        var canvas = GameObject.Find("DescriptionCanvas");
        back = canvas.transform.Find("Back").gameObject;
        senText = back.transform.Find("Text").GetComponent<TMP_Text>();
        var image = back.transform.Find("Image").gameObject;
        nameText = image.transform.Find("Name").GetComponent<TMP_Text>();
    }

    protected virtual void Start()
    {
        aud = GetComponent<AudioSource>();
        //フロア移動の足場を取得します
        GetFloor();
    }

    /// <summary>
    /// 初期値で別ファイルで書いた説明を入れます
    /// </summary>
    /// <param name="description">ブロック説明</param>
    public void SetBlock(string description,string bName,string setFloor)
    {
        var name = Regex.Match(description, @"^@(.+){").Groups[1].Value;
        var role = Regex.Match(description, @"{(.+)}",RegexOptions.Singleline).Groups[1].Value;

        //ブロック名
        roleName = name;
        //プレイヤに知らせるこのオブジェクトの破壊条件
        this.role = role;
        
        //gameObjectの名前
        this.name = bName;

        //ブロックが破壊された後、フロア移動の床を表示する場合処理します
        if (string.IsNullOrEmpty(setFloor)) return;
        //移動するフロア床名を入れます
        floorSet = setFloor;
    }

    /// <summary>
    /// 展開するフロア移動の足場を取得します
    /// </summary>
    void GetFloor()
    {
        if (string.IsNullOrEmpty(floorSet)) return;
        //指定の親オブジェクト下から

        //パスの指定
        var pathJ = string.Format("Jump_{0}", floorSet);
        var pathA = string.Format("Arrow_{0}", floorSet);
        var root = transform.root;
        //オブジェクトの検索
        var floorJump = root.transform.Find(pathJ);
        var floorArrow = root.transform.Find(pathA);
        //オブジェクトの代入
        meatObj = floorJump.gameObject;
        catArrow = floorArrow.gameObject;

        meatObj.SetActive(false);
        catArrow.SetActive(false);
    }

    /// <summary>
    /// このブロックの役割を表示します
    /// </summary>
    public void SetText()
    {
        back.SetActive(true);
        nameText.text = roleName;
        senText.text = role;
    }

    /// <summary>
    /// Canvasを非表示にします
    /// </summary>
    public void InActive()
    {
        back.SetActive(false);
    }

    /// <summary>
    /// 次のフロアへ行く足場を展開
    /// </summary>
    public void ActiveFloor()
    {
        meatObj.SetActive(true);
        catArrow.SetActive(true);
    }

    /// <summary>
    /// 破壊します
    /// </summary>
    public virtual void Destroy()
    {
        //次のフロアを出すブロックなら処理します
        if (!string.IsNullOrEmpty(floorSet)) ActiveFloor();

        //SEを鳴らします
        aud.PlayOneShot(clip);

        //当たり判定を切ります
        GetComponent<BoxCollider2D>().enabled = false;

        //スプライトを切ります
        GetComponent<SpriteRenderer>().enabled = false;

        //レイヤーを変更します
        gameObject.layer = LayerMask.NameToLayer("Default");

        //SEが流れ終わってから破壊します
        Destroy(gameObject, clip.length);
    }

    /// <summary>
    /// 自分の名前についているイベント番号を取得
    /// </summary>
    /// <param name="name">自分の名前</param>
    protected string EventNumber(string name)
    {
        return _= Regex.Match(name, @"\d+").ToString();
    }
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class TitleUIManager : StandaloneInputModule
{
    /// <summary>
    /// 現在選択されているUIを記録します
    /// </summary>
    GameObject memory;
    /// <summary>
    /// 画像を変更したUI記録します
    /// </summary>
    Image changeUI;
    [SerializeField] Sprite defaultCat;
    /// <summary>
    /// ボタンの種類によって入れる画像を変えます
    /// </summary>
    [SerializeField] Sprite[] cats;

    private void Update()
    {
        CheckUI();
    }

    /// <summary>
    /// 選択されているUIを記録します
    /// </summary>
    void CheckUI()
    {
        //ヒットしたUIを入れます
        memory = GetCurrentFocusedGameObject();

        //ヒットしたものがない又は
        //ヒットした名前と記録している名前が異なるなら
        if(changeUI && (!memory || !UIForChnageCat(memory)))
        {
            changeUI.sprite = defaultCat;
            changeUI = null;
        }

        //①ヒットしたものがあり、変更したUIを記録しているなら処理を開始します
        //②猫の画像を変えるためのUIにヒットしている
        if (memory && !changeUI && UIForChnageCat(memory))
        {
            //名前によって変更するUIを変えます
            string[] catsUI = { "Start_Cat", "Continue_Cat", "End_Cat" };

            //ヒット先にrootを取得します
            var root = memory.transform.root;

            //ヒット先の名前から管理しているオブジェクトにアクセスします
            changeUI = GetUIChar(memory.name, root);

            //変更する画像に合った配列番号を取得します
            var index = Array.IndexOf(catsUI, changeUI.name);

            //名前によって入れる画像を変えます
            changeUI.sprite = cats[index];
        }
    }

    /// <summary>
    /// ヒットした名前に特定の名前が入っている
    /// </summary>
    bool UIForChnageCat(GameObject hitUI)
    {
        var flag = false;

        //hitUIが無ければfalseを返します
        if (!hitUI) return flag;

        //Strat,Continue,Endにヒットしていればtrueを返します
        return _= Regex.IsMatch(hitUI.name, @"(Start|Continue|End)Text");
    }

    /// <summary>
    /// 名前から指定の猫UIを取得します
    /// </summary>
    Image GetUIChar(string uiName, Transform parent)
    {
        //アクセスする名前を取得
        var accessName = Regex.Match(uiName, @"(.+)Text").Groups[1].Value;

        //名前からStrat,Continue,Endのどれかにアクセスします
        var man = parent.transform.Find(accessName);

        //アクセス名を作成します
        var catUI = accessName + "_Cat";

        //画像を変更するUIにアクセスします
        return _ = man.transform.Find(catUI).GetComponent<Image>();
    }
}

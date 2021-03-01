using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 破壊したら扉を開ける鍵を表示するクラス
/// </summary>
public class Pot : MonoBehaviour
{
    /// <summary>
    /// 壊されたら表示する鍵
    /// </summary>
    [SerializeField] GameObject key;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    /// <summary>
    /// 鍵を入れます
    /// </summary>
    public void InitSetKey(GameObject key)
    {
        this.key = key;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            //鍵を持っているなら生成します
            if (key) Instantiate(key, transform.position, Quaternion.identity);

            //アニメーション再生
            anim.SetBool("Break", true);

            //コライダーを切ります
            GetComponent<BoxCollider2D>().enabled = false;

            var aud = GetComponent<AudioSource>();
            aud.Play();

            //SE終了後、破壊します
            Destroy(gameObject, aud.clip.length);
        }
    }
}

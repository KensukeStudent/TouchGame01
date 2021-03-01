using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum MoveState
{
    rigid,
    trans
} 

public class testt : MonoBehaviour
{
    [SerializeField] MoveState state;

    Rigidbody2D rb2d;

    float speed = 5;

    int scale = 1;
    [SerializeField] bool animFlag = false;

    Animator anim;
    private AnimatorClipInfo[] animator_clipinfo1;//AnimatorClipInfo型の変数を宣言
    private string clip_name;//string型の変数を宣言　アニメーションクリップ名前取得用
    
    [SerializeField] Animation anima;

    [SerializeField] GameObject obj;

    private void OnEnable()
    {
        Debug.Log("Enable");
        obj = GameObject.Find("obj");
    }

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();

        //anima = GetComponent<Animation>();
        //Debug.Log(anima["Arrow"].length);

        //anima["Arrow"].normalizedTime = 0.6f;

        //Time.timeScale = 0;

        anim = GetComponent<Animator>();

        //var anim = GetComponent<Animator>();
        //var info = anim.GetCurrentAnimatorStateInfo(0);

        var anima = GetAnimation(anim, "Arrow");

        frameRate = anima.frameRate / 1000;
        Debug.Log("Start");
    }

    /// <summary>
    /// フレーム数を記録
    /// </summary>
    float frame = 0.0f;
    /// <summary>
    /// アニメーション再生のフレームレートを定義します
    /// </summary>
    float frameRate = 0.012f;
    /// <summary>
    /// アニメーションフレーム数の終わり
    /// </summary>
    const float endFrame = 1.0f;

    /// <summary>
    /// タイムスケールが0の時にアニメーションの再生をします
    /// </summary>
    void UnScaleAnim()
    {
        //指定のフレーム数を上げます
        frame += frameRate;

        if (frame >= endFrame)
        {
            anim.speed = 0;
            return;
        }

        //フレーム位置でアニメーションの再生を行います
        anim.Play("arrow", -1, frame);
    }

    /// <summary>
    /// アニメーターの中にある指定のアニメーションを取得します
    /// </summary>
    /// <param name="animator">検索するアニメーター</param>
    /// <param name="clipname">取得するクリップ名</param>
    /// <returns>animationClip</returns>
    AnimationClip GetAnimation(Animator animator, string clipname)
    {
        //指定するanimatorに入っている全てアニメーションを表示します
        var ac = animator.runtimeAnimatorController;

        //acの配列内から指定のアニメーションと同じものをclipに入れます
        return _ = Array.Find(ac.animationClips, anima => anima.name == clipname);
    }

    [Range(0.01f,1.0f)]
    [SerializeField] float range;

    private void Update()
    {
        if (animFlag)
        {
            scale = 1;
            obj.SetActive(false);
        }
        else
        {
            scale = 0;
        }

        if (scale == 0)
        {
            //タイムスケール0でアニメーションの再生
             UnScaleAnim();
        }

        //if (state == MoveState.trans) TransMove();
    }

    private void FixedUpdate()
    {
        //if (state == MoveState.rigid) RigidMove();
    }

    void RigidMove()
    {
        var pos = rb2d.velocity;

        pos.x = speed;
        rb2d.velocity = pos;
    }

    void TransMove()
    {
        var pos = transform.position;

        pos.x += Time.deltaTime * speed;
        transform.position = pos;
    }
}

using UnityEngine;

/// <summary>
/// Time.Scaleが0でもアニメーションさせるクラス
/// </summary>
public class TimeZeroAnim : MonoBehaviour
{
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

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

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
    /// 指定のアニメーションクリップの再生時間を取得します
    /// </summary>
    /// <param name="anim">指定のアニメーター</param>
    /// <param name="clipName">クリップ名</param>
    /// <returns></returns>
    public static float GetAnimTime(Animator anim, string clipName)
    {
        //指定するanimatorに入っている全てアニメーションを表示します
        var ac = anim.runtimeAnimatorController;

        //acの配列内から指定のアニメーションと同じものをclipに入れます
        var clip = System.Array.Find(ac.animationClips, anima => anima.name == clipName);

        return clip.length;
    }
}

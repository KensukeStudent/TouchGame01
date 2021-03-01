
/// <summary>
/// サウンドのインターフェイス
/// </summary>
interface IAudio
{
    /// <summary>
    /// 効果音を鳴らします
    /// </summary>
    /// <param name="clipNo">audioClip番号</param>
    /// <param name="vol">音量</param>
    void PlaySE(int clipNo, float vol = 1.0f);
}
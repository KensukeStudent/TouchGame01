using UnityEngine;

#region カメラ値
//ステージ１X 18 Y 0
#endregion

/// <summary>
/// プレイヤーを追尾するカメラクラス
/// </summary>
public class CameraContorller : MonoBehaviour
{
    GameObject player;
    /// <summary>
    /// カメラの上限
    /// </summary>
    public Vector2 MaxC { private set; get; } = new Vector2(0,0);
    /// <summary>
    /// カメラの下限
    /// </summary>
    public Vector2 MinC { private set; get; } = new Vector2(0,0);

    /// <summary>
    /// 移動する方向を指定
    /// </summary>
    string dire = "";
    /// <summary>
    /// 現在カメラが移動中か
    /// </summary>
    bool autoMove = false;
    /// <summary>
    /// オート移動速度
    /// </summary>
    const float moveSpeed = 30f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (autoMove) CamMove();
        else CamToPlayer();
    }

    /// <summary>
    /// プレイヤーを追尾カメラ処理
    /// </summary>
    void CamToPlayer()
    {
        //プレイヤーのPosition取得
        var playerPosX = player.transform.position.x;
        var playerPosY = player.transform.position.y;
        //追尾可能範囲の制限をしたPositionを取得
        //カメラのPosition(Z)取得
        var posZ = transform.position.z;
        //プレイヤーのPositionをカメラにセット
        var CameraPosX = Mathf.Clamp(playerPosX, MinC.x, MaxC.x);
        var CameraPosY = Mathf.Clamp(playerPosY, MinC.y, MaxC.y);
        transform.position = new Vector3(CameraPosX, CameraPosY, posZ);
    }

    /// <summary>
    /// 別のステージの面に移動したときに自動でCameraのMaxとMinの位置に移動させます
    /// </summary>
    void CamMove()
    {
        var camVec = transform.position;

        switch (dire)
        {
            //右方向に移動
            case "R":
            //左方向に移動
            case "L":
                SetVec(dire,ref camVec.x, MinC.x);
                break;

            //下方向に移動
            case "D":
            //上方向に移動
            case "U":
                SetVec(dire,ref camVec.y, MinC.y);
                break;
        }
         transform.position = camVec;
    }

    /// <summary>
    /// 移動する値を代入
    /// </summary>
    /// <param name="direction">移動方向</param>
    /// <param name="vec">自分の座標値</param>
    /// <param name="nextVex">代入する座標値</param>
    void SetVec(string direction, ref float vec, float nextVex)
    {
        switch (direction)
        {
            //プラスの値
            case "R":
            case "U":
                if (vec < nextVex) vec += Time.deltaTime * moveSpeed;
                else autoMove = false;
                break;

            //マイナスの値
            case "D":
            case "L":
                if (vec > nextVex) vec -= Time.deltaTime * moveSpeed;
                else autoMove = false;
                break;
        }

        if(!autoMove) vec = nextVex;
    }

    /// <summary>
    /// 移動向きと最大最小の座標を代入
    /// </summary>
    public void SetDirectVec(string direction, Vector2 max, Vector2 min)
    {
        //値の代入
        dire = direction;
        MaxC = max;
        MinC = min;
        //オート移動開始
        autoMove = true;
    }
}

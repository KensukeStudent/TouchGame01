﻿using UnityEngine;

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
    public Vector2 MaxC { private set; get; } = new Vector2(0, 0);
    /// <summary>
    /// カメラの下限
    /// </summary>
    public Vector2 MinC { private set; get; } = new Vector2(0, 0);

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

    AudioSource aud;
    /// <summary>
    /// AudioClip
    /// </summary>
    [SerializeField] AudioClip[] clipA;

    /// <summary>
    /// SeClip
    /// </summary>
    [SerializeField] AudioClip clipSE;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        //初期座標を入れます
        SetInit();

        //現在のステージ番号を取得
        var no = GameManager.Instance.StageNo;

        aud = GetComponent<AudioSource>();
        //ステージに応じたBGMを割り当てます
        aud.clip = clipA[no];

        //ステージに応じた音量を設定します
        var vol = new float[2] { 0.5f, 0.8f };
        aud.volume = vol[no];

        aud.Play();
    }

    private void Update()
    {
        //フロア移動でオートで移動します
        if (autoMove) CamMove();
        //それ以外はプレイヤーを追います
        else CamToPlayer();
    }

    /// <summary>
    /// 初期値に最大最小のXYを入れます
    /// </summary>
    void SetInit()
    {
        //最大最小の初期位置テーブル
        Vector2[] vecMax = { new Vector2(-1.0f, 0.5f),new Vector2(37.0f, 0.5f) };
        Vector2[] vecMin = { new Vector2(-1.0f, 0.5f), new Vector2(37.0f, 0.5f) };

        //現在のステージ番号を取得
        var fileNo = GameManager.Instance.StageNo;

        //座標を入れます
        MaxC = vecMax[fileNo];
        MinC = vecMin[fileNo];
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
                SetVec(dire, ref camVec.x, MinC.x);
                break;

            //左方向に移動
            case "L":
                SetVec(dire,ref camVec.x, MaxC.x);
                break;

            //下方向に移動
            case "D":
                SetVec(dire, ref camVec.y, MaxC.y);
                break;

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
    /// <param name="nextVec">代入する座標値</param>
    void SetVec(string direction, ref float vec, float nextVec)
    {
        switch (direction)
        {
            //プラスの値
            case "R":
            case "U":
                if (vec < nextVec)
                {
                    vec += Time.deltaTime * moveSpeed;

                    //次の値がnextVecを超えていたら
                    //値をnextVecにします
                    if (vec > nextVec) vec = nextVec;
                }
                else
                    autoMove = false;
                break;

            //マイナスの値
            case "D":
            case "L":
                if (vec > nextVec)
                {
                    vec -= Time.deltaTime * moveSpeed;

                    //次の値がnextVecを未満なら
                    //値をnextVecにします
                    if (vec < nextVec) vec = nextVec;
                }
                else
                    autoMove = false;
                break;
        }

        //値を代入します
        if(!autoMove) vec = nextVec;
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

        //移動サウンドを鳴らします
        aud.PlayOneShot(clipSE); //SE音量はBGMに依存させます
    }
}

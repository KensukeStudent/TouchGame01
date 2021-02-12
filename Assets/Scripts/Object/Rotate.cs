using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    float rotSpeed = 0.5f;
    float speed = 0.1f;

    private void Update()
    {
        RotateObj();
        //円のように回転する
        //transform.Translate(speed, 0, 0);
        //Z軸方向に進む
        //transform.Translate(-speed, 0, 0,Space.World);
    }

    /// <summary>
    /// 回転させる
    /// </summary>
    void RotateObj()
    {
        Vector3 rVec = transform.localEulerAngles;
        rVec.z = rotSpeed;

        transform.Rotate(rVec);
    }
}

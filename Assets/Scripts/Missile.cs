using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // 미사일을 회전하는 스크립트


    void Update()
    {
        transform.Rotate(Vector3.right * 30 * Time.deltaTime);
    }
}

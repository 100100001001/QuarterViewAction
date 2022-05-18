using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target; // 공전 목표
    public float orbitSpeed; // 공전 속도
    Vector3 offset;          // 목표와의 거리

    void Start()
    {
        offset = transform.position - target.position;
    }

    void Update()
    {
        transform.position = target.position + offset;
        transform.RotateAround(target.position,
                               Vector3.up,
                               orbitSpeed * Time.deltaTime); // RotateAround() 타겟 주위를 회전하는 함수
        offset = transform.position - target.position;
    }
}

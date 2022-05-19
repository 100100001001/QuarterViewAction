using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    void Start()
    {
        StartCoroutine(Explosion());
    }

    // 시간차 폭발을 위한 코루틴 선언
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;         // 물리적 속도 초기화
        rigid.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        effectObj.SetActive(true);



        // 수류탄 피격
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
                                                     15,
                                                     Vector3.up,
                                                     0f,
                                                     LayerMask.GetMask("Enemy"));
        // SphereCastAll(시작 위치, 반지름, 쏘는 방향, 레이를 쏘는 길이, 레이어 마스크) : 구체 모양의 레이캐스팅 (모든 오브젝트)

        // 수류탄 범위 적들의 피격함수를 호출
        foreach (RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };  // 무기 타입
    public Type type;                   // 무기 타입
    public int damage;                  // 데미지
    public float rate;                  // 공속

    public int maxAmmo;                 // 최대 탄약
    public int curAmmo;                 // 현재 탄약

    public BoxCollider meleeArea;       // 범위
    public TrailRenderer trailEffect;   // 효과 (잔상을 그려주는 컴포넌트)

    public Transform bulletPos;  // 총알이 생성되어야할 위치
    public GameObject bullet;    // 총알
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing"); // 코루틴 실행 함수
        }

        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing() // IEnumerator 열거형 함수 클래스
    {
        // yield 결과를 전달하는 키워드 
        // yield return null; // 1프레임 대기
        // yield return new WaitForSeconds(0.1f); // 0.1초 대기
        // yield break; 코루틴 탈출

        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.4f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.4f);
        trailEffect.enabled = false;
    }
    // 일반 함수   : User() 메인 루틴 -> Swing() 서브 루틴 -> User() 메인 루틴
    // 코루틴 함수 : User() 메인 루틴 + Swing() 코루틴 (Co-Op) ---- 동시 실행

    IEnumerator Shot()
    {
        // 1. 총알 발사
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;

        // 2. 탄피 배출
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // 회전
    }
}

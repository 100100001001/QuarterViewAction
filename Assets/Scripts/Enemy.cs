using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material; // Material은 MeshRenderer에서 접근 가능
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;

            // 현재 위치에 피격 위치를 빼서 반작용 방향 구하기
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec));
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;

            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 14;

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;

            rigid.AddForce(reactVec * 5, ForceMode.Impulse);

            Destroy(gameObject, 4);
        }
    }

}

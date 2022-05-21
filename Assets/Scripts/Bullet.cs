using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee; 

    void OnCollisionEnter(Collision collision)
    {
        // �ٴڿ� ������ 3�� �ڿ� �����
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ���������� �ƴϰ�, ���� ������ �ٷ� �����
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}

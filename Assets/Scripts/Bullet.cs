using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

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
        // ���� ������ �ٷ� �����
        if (other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}

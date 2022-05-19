using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;

    void OnCollisionEnter(Collision collision)
    {
        // 바닥에 닿으면 3초 뒤에 사라짐
        if (collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject, 3);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 벽에 닿으면 바로 사라짐
        if (other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}

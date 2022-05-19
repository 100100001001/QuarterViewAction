using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };  // ���� Ÿ��
    public Type type;                   // ���� Ÿ��
    public int damage;                  // ������
    public float rate;                  // ����

    public int maxAmmo;                 // �ִ� ź��
    public int curAmmo;                 // ���� ź��

    public BoxCollider meleeArea;       // ����
    public TrailRenderer trailEffect;   // ȿ�� (�ܻ��� �׷��ִ� ������Ʈ)

    public Transform bulletPos;  // �Ѿ��� �����Ǿ���� ��ġ
    public GameObject bullet;    // �Ѿ�
    public Transform bulletCasePos;
    public GameObject bulletCase;

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing"); // �ڷ�ƾ ���� �Լ�
        }

        else if (type == Type.Range && curAmmo > 0)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing() // IEnumerator ������ �Լ� Ŭ����
    {
        // yield ����� �����ϴ� Ű���� 
        // yield return null; // 1������ ���
        // yield return new WaitForSeconds(0.1f); // 0.1�� ���
        // yield break; �ڷ�ƾ Ż��

        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.4f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.4f);
        trailEffect.enabled = false;
    }
    // �Ϲ� �Լ�   : User() ���� ��ƾ -> Swing() ���� ��ƾ -> User() ���� ��ƾ
    // �ڷ�ƾ �Լ� : User() ���� ��ƾ + Swing() �ڷ�ƾ (Co-Op) ---- ���� ����

    IEnumerator Shot()
    {
        // 1. �Ѿ� �߻�
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;

        // 2. ź�� ����
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); // ȸ��
    }
}

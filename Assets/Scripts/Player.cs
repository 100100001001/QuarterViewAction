using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades; // 공전하는 물체(수류탄)을 컨트롤 하기 위한 배열
    public int hasGrenades;       // 수류탄
    public GameObject grenadeObj; // 슈류탄을 저장할 변수
    public Camera followCamera;   // 공격 시 마우스로 회전 가능하도록 하기 위함


    public int ammo;        // 탄약
    public int coin;        // 동전
    public int health;      // 체력

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAxis;
    float vAxis;

    bool wDown;   // 이동
    bool jDown;   // 점프
    bool fDown;   // 공격
    bool gDown;   // 수류탄
    bool rDown;   // 총알 재장전
    bool iDown;   // 아이템 먹기
    bool sDown1;  // 무기1
    bool sDown2;  // 무기2
    bool sDown3;  // 무기3

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady = true; // 공격 준비
    bool isBorder;           // 벽 충돌 플래그

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;

    GameObject nearObject;      // 트리거 된 아이템 저장
    Weapon equipWeapon;         // 기존에 장착된 무기 저장
    int equipWeaponIndex = -1;
    float fireDelay;            // 공격 딜레이

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();     // 수류탄
        Attack();      // 공격 함수
        Reload();      // 총알 재장전
        Dodge();
        Swap();        // 무기 교체 함수
        Interation();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");    // 이동
        vAxis = Input.GetAxisRaw("Vertical");      // 이동
        wDown = Input.GetButton("Walk");           // shift 누를 때만 작동하도록
        jDown = Input.GetButtonDown("Jump");       // 점프
        fDown = Input.GetButton("Fire1");          // 공격
        gDown = Input.GetButtonDown("Fire2");          // 수류탄
        rDown = Input.GetButtonDown("Reload");     // 총알 재장전
        iDown = Input.GetButtonDown("Interation"); // 아이템 먹기

        // 장비(무기) 단축키
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");


    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        // 무기 교체 or 장전 중 or 공격 중에는 이동 불가
        if (isSwap || isReload || !isFireReady)
            moveVec = Vector3.zero;
        
        if (!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        // 1. 키보드에 의한 회전 (LooAt 나아가는 방향을 바라 봄)
        transform.LookAt(transform.position + moveVec);

        // 2. 마우스에 의한 회전
        if (fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); // ScreenPointToRay() 스크린에서 월드로 Ray를 쏘는 함수
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100)) // out : return처럼 반환값을 주어진 변수에 저장하는 키워드
            {
                Vector3 nextVec = rayHit.point - transform.position;
                // rayHit.point 레이가 닿았던 지점

                nextVec.y = 0; // RaycastHit의 높이는 무시하도록 Y축 값을 0으로 초기화
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Grenade()
    {
        if (hasGrenades == 0)
            return;

        if (gDown && !isReload && !isSwap)
        {
            // 마우스 위치에 바로 던짐

            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition); // ScreenPointToRay() 스크린에서 월드로 Ray를 쏘는 함수
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100)) // out : return처럼 반환값을 주어진 변수에 저장하는 키워드
            {
                Vector3 nextVec = rayHit.point - transform.position;
                // rayHit.point 레이가 닿았던 지점

                nextVec.y = 10; // RaycastHit의 높이는 무시하도록 Y축 값을 0으로 초기화

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay; // 공격 가능 여부

        // 조건이 충족되면 무기에 있는 함수 실행
        if (fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if (equipWeapon == null) // 장착 무기가 없을 경우 return
            return;
        if (equipWeapon.type == Weapon.Type.Melee) // 단거리 무기 장착했을 경우 return
            return;
        if (ammo == 0) // 총알이 단 한개도 없을 경우 return
            return;

        if (rDown && !isJump && !isDodge && !isSwap && !isFireReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 3f);
        }

    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;


        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            // 이미 장착되어 있을 경우 기존의 아이템 비활성화
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Interation()
    {
        if (iDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero; // angularVelocity 물리 회전 속도
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        // DrawRay(시작 위치, 쏘는 방향 * 레이의 길이, 색깔) : Scene내에서 Ray를 보여주는 함수

        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch(item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades)
                        hasGrenades = maxHasGrenades;

                    break;
            }
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}

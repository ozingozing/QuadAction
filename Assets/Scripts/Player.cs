using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    //캐릭컨트롤러
    public CharacterController controller;
    private Rigidbody rb;
    public bool isDead;

    public AudioSource jumpSound;

    //모든 벡터값
    private Vector3 currentMoveVelocity;
    private Vector3 moveDampVelocity;
    private Vector3 moveVector;
    private Vector3 dodgeVector;
    private Vector3 currentForceVelocity;
    private Vector3 PlayerInput;

    //move관련 변수
    private float currentSpeed;
    private float moveSmoothTime;
    private float walkSpeed;
    private float runSpeed;

    private bool isBorder;
    private bool runDown;

    //점프관련변수
    private float gravityStrength;
    private float jumpStrength;

    public bool jumpDown;
    public bool isJump;
    private bool isDodge;

    //피격관련 변수
    private bool isDamage;
    MeshRenderer[] meshs;
    private bool isHit;
    public bool isBossAtk;

    //공격관련 변수
    public bool fireDown;
    private bool grenadeDown;
    private bool isFireReady;
    private bool reloadDown;
    private bool isReload;
    private float fireDelay;


    //애니메이션관련 변수
    private Animator anim;
    public Animation ani;

    //item관련 변수
    public GameObject[] weapons;
    public GameObject nearObject;
    public Weapon equipWeapon;
    public GameObject[] grenades;
    public GameObject grenadeObject;

    private int equipWeaponIndex = -1;
    public int ammo;
    public int coin;
    public int health;
    public int score;

    public int hasGrenades;
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    public bool[] hasWeapons;
    private bool interactionDown;
    public bool isShop;
    public bool isInteraction = false;
    private bool swapDown1;
    private bool swapDown2;
    private bool swapDown3;
    private bool isSwap;

    private void Awake()
    {
        if(Player.instance == null) Player.instance = this;

        ani = this.GetComponentInChildren<Animation>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        moveSmoothTime = 0.1f;
        walkSpeed = 10;
        runSpeed = 20;
        gravityStrength = 9.81f;
        jumpStrength = 15;
        isFireReady = true;

        if (PlayerPrefs.HasKey("MaxScore")) PlayerPrefs.SetInt("MaxScore", 0);

        Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        //PlayerPrefs.SetInt("MaxScore", 125134);
    }
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.gameObject.tag == "Wall")
        {
            Debug.Log("collision");
            ContactPoint cp = collision.GetContact(0);
            Vector3 dir = transform.position - cp.point;
            rb.AddForce(dir.normalized * 30, ForceMode.Impulse);
        }
        else if(collision.gameObject.tag == "Enemy")
        {
            rb.velocity = Vector3.zero;
            FreezeVelocity();
        }
        
    }
    private void FixedUpdate()
    {
        Dodge();
        Jump();
        Move();
        PlayerLook.instance.Look();
        Attack();
        StopToWall();
        Grenade();
    }

    private void OnDrawGizmos()
    {
        float sphereScale = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        if(true == Physics.SphereCast(transform.position, sphereScale, Vector3.forward, out RaycastHit hit, 0.5f))
        {
            Gizmos.DrawWireSphere(transform.position + transform.forward * hit.distance, 5);
        }
    }

    void Update()
    {
        FreezeVelocity();
        Reload();
        GetInput();
        Interation();
        Swap();
        PlayerLook.instance.CamSelect();
    }

    void FreezeVelocity()
    {
        rb.angularVelocity = Vector3.zero;
    }

    void GetInput()
    {

        PlayerInput = new Vector3
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = 0,
            z = Input.GetAxisRaw("Vertical")
        };

        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        runDown = Input.GetKey(KeyCode.LeftShift);
        interactionDown = Input.GetButtonDown("Interation");

        fireDown = Input.GetButton("Fire1");
        grenadeDown = Input.GetButtonDown("Fire2");
        reloadDown = Input.GetButtonDown("Reload");

        jumpDown = Input.GetButtonDown("Jump");

        swapDown1 = Input.GetButtonDown("Swap1");
        swapDown2 = Input.GetButtonDown("Swap2");
        swapDown3 = Input.GetButtonDown("Swap3");
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    void Move()
    {
        if (isDead) return;
        else
        {
            if (PlayerLook.instance.nowCamNum == 0 && !isInteraction)
            {

                Vector3 direction = PlayerInput.normalized;
                moveVector = transform.TransformDirection(direction);

                if (isDodge)
                {
                    moveVector = dodgeVector;
                }

                if (isSwap || !isFireReady || isReload)
                {
                    moveVector = Vector3.zero;
                }

                currentMoveVelocity = Vector3.SmoothDamp(
                 currentMoveVelocity,
                 moveVector * currentSpeed,
                 ref moveDampVelocity,
                 moveSmoothTime);

                if (!isBorder && !isHit)
                {
                    controller.Move(currentMoveVelocity * Time.deltaTime);
                }

            }
            else if ((PlayerLook.instance.nowCamNum == 1 || PlayerLook.instance.nowCamNum == 2) && !isInteraction)
            {
                moveVector = PlayerInput.normalized;
                if (isDodge)
                {
                    moveVector = dodgeVector;
                }

                if (isSwap || !isFireReady || isReload)
                {
                    moveVector = Vector3.zero;
                }

                if (!isBorder && !isHit)
                {
                    transform.position += moveVector * currentSpeed * Time.deltaTime;
                }


                transform.LookAt(transform.position + moveVector);
            }


            anim.SetBool("isWalk", moveVector != Vector3.zero);
            anim.SetBool("isRun", runDown);
        }
    }

    void Jump()
    {
        if (isDead) return;
        else
        {
            
            Ray groundCheckRay = new Ray(transform.position, Vector3.down);

            if (Physics.Raycast(groundCheckRay, 0.5f)
                && (!isJump && !isDodge)
                && (currentSpeed == walkSpeed)
                && (moveVector == Vector3.zero))
            {
                currentForceVelocity.y = -2f;

                if (Input.GetKey(KeyCode.Space))
                {
                    jumpSound.Play();
                    currentForceVelocity.y = jumpStrength;
                    anim.SetBool("isJump", true);
                    anim.SetTrigger("doJump");
                    isJump = true;
                }
            }
            else if (!isHit)
            {
                currentForceVelocity.y -= gravityStrength * Time.deltaTime;
                controller.Move(currentForceVelocity * Time.deltaTime);
            }
            if (!Input.GetKey(KeyCode.Space) && Physics.Raycast(groundCheckRay, 0.5f) && isJump)
            {
                JumpOut();
            }

            
        }
    }



    void Grenade()
    {
        if (isDead) return;
        if (hasGrenades == 0) return;

        if(grenadeDown && !isReload && !isSwap)
        {
            if(PlayerLook.instance.nowCamNum == 0)
            {
                GameObject instantGrenade = Instantiate(grenadeObject, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();

                rigidGrenade.AddForce(transform.forward * 10 + transform.up * 10, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
            else if(PlayerLook.instance.nowCamNum == 1)
            {
                Ray ray = PlayerLook.instance.arrCam[1].ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    Vector3 nextVec = hit.point - transform.position;
                    nextVec.y = 10;

                    GameObject instantGrenade = Instantiate(grenadeObject, transform.position, transform.rotation);
                    Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                    rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                    rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                    hasGrenades--;
                    grenades[hasGrenades].SetActive(false);
                }
            }
        }
    }
    void Attack()
    {
        if (isDead) return;
        if (equipWeapon == null) return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fireDown && isFireReady && !isDodge && !isSwap && !isShop)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if (isDead) return;
        if (equipWeapon == null) return;

        if (equipWeapon.type == Weapon.Type.Melee) return;

        if (ammo == 0) return;

        if(reloadDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 2);
        }
    }

    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.currentAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }

    void Dodge()
    {
        if (isDead) return;
        if (Input.GetKey(KeyCode.W) && PlayerLook.instance.nowCamNum == 0)
        {
            if (jumpDown && (moveVector != Vector3.zero) && (!isJump && !isDodge))
            {
                dodgeVector = moveVector;
                currentSpeed *= 2;
                walkSpeed *= 2;
                runSpeed *= 2;
                anim.SetTrigger("doDodge");
                isDodge = true;
                Invoke("DodgeOut", 0.75f);
            }
        }
        else if(PlayerLook.instance.nowCamNum == 1)
        {
            if (jumpDown && (moveVector != Vector3.zero) && (!isJump && !isDodge))
            {
                dodgeVector = moveVector;
                walkSpeed *= 2;
                runSpeed *= 2;
                anim.SetTrigger("doDodge");
                isDodge = true;
                Invoke("DodgeOut", 0.75f);
            }
        }
    }

    void DodgeOut()
    {
        currentSpeed /= 2;
        walkSpeed /= 2;
        runSpeed /= 2;
        isDodge = false;
    }
    void JumpOut()
    {
        isJump = false;
        anim.SetBool("isJump", false);
    }

    void SwapOut()
    {
        isSwap = false;
    }

    
   
    void Swap()
    {
        if (isDead) return;
        if (swapDown1 && (!hasWeapons[0]|| equipWeaponIndex == 0))
        {
            return;
        }
        else if(swapDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
        {
            return;
        }
        else if(swapDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
        {
            return;
        }

        int weaponIdex = -1;
        if (swapDown1) weaponIdex = 0;
        if (swapDown2) weaponIdex = 1;
        if (swapDown3) weaponIdex = 2;

        if ((swapDown1 || swapDown2 || swapDown3) && !isJump && !isDodge && !isSwap)
        {
            if(equipWeapon != null) equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIdex;
            equipWeapon = weapons[weaponIdex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }

    void Interation()
    {
        if (isDead) return;
        if (interactionDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
            else if(nearObject.tag == "Shop")
            {
                isShop = true;
                isInteraction = true;
                nearObject.GetComponent<Shop>().Enter();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        else
        {
            if (other.tag == "Item")
            {
                Item item = other.GetComponent<Item>();
                switch (item.type)
                {
                    case Item.Type.Ammo:
                        this.ammo += item.value;
                        if (ammo > maxAmmo) ammo = maxAmmo;
                        break;
                    case Item.Type.Coin:
                        this.coin += item.value;
                        if (coin > maxCoin) coin = maxCoin;
                        break;
                    case Item.Type.Heart:
                        this.health += item.value;
                        if (health > maxHealth) health = maxHealth;
                        break;
                    case Item.Type.Grenade:
                        if (hasGrenades == maxHasGrenades) return;
                        grenades[hasGrenades].SetActive(true);
                        this.hasGrenades += item.value;
                        if (hasGrenades > maxHasGrenades) hasGrenades = maxHasGrenades;
                        break;
                }
                Destroy(other.gameObject);
            }
            else if (other.tag == "EnemyBullet" || other.tag == "BossRock")
            {
                
                if (!isDamage)
                {
                    isHit = true;
                    Bullet enemyBullet = other.GetComponent<Bullet>();
                    health -= enemyBullet.damage;


                    isBossAtk = other.name == "BossMeleeArea";

                    if (other.tag == "BossRock")
                    {
                        controller.enabled = false;
                        isBossAtk = true;
                    }

                    if (!isDead)
                    {
                        StartCoroutine(OnDamage(isBossAtk, other));
                    }
                    else return;
                }

                if (other.GetComponent<Rigidbody>() != null)
                {
                    Destroy(other.gameObject);
                }
            }
        }
        
    }

    IEnumerator OnDamage(bool isBossAtk1, Collider other)
    {
        if (isDead) yield break;
        else
        {
            isDamage = true;
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.yellow;
            }

            if (isBossAtk1 && (other.name == "BossMeleeArea"))
            {
                Debug.Log("맞음");
                controller.enabled = false;
                Vector3 dir = (other.transform.position - this.transform.position).normalized;
                dir.y = 0;

                if ((Boss.Instance.transform.position.x == this.transform.position.x)
                    && (Boss.Instance.transform.position.z == this.transform.position.z))
                {
                    Debug.Log("2");
                    dir = Boss.Instance.transform.forward.normalized;
                    rb.AddForce(dir * 20, ForceMode.Impulse);
                }
                else
                {
                    Debug.Log("33333333333");
                    rb.AddForce(dir * -20, ForceMode.Impulse);
                }
            }

            if (health <= 0 && !isDead)
            {
                OnDie();
                yield return new WaitForSeconds(1.2f);
                Time.timeScale = 0;
            }
            yield return new WaitForSeconds(0.3f);

            isDamage = false;
            rb.velocity = Vector3.zero;
            controller.enabled = true;
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.white;
                isHit = false;
            }
        }
        
    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        GameManager.Instance.GameOver();
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop")
        {
            nearObject = other.gameObject;
            Debug.Log(nearObject.tag);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
        else if(other.tag == "Shop")
        {
            Shop.Instance.Exit();
            nearObject = null;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}

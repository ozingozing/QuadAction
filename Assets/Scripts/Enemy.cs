using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class Enemy : MonoBehaviour
{
    public static Enemy instance;
    public enum Tybe { A, B, C, D};
    public Tybe enemyTybe;
    public int maxHealth;
    public int currentHealth;
    public GameManager manager;
    public Transform target;
    public bool isChase;
    public BoxCollider meleeArea;
    public bool isAttack;
    public GameObject bullet;
    public bool isDead;
    public GameObject[] coins;
    public int score;


    public Rigidbody rb;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;

    private void OnDisable()
    {
        isDead = false;
        isAttack = false;
        isChase = false;
        currentHealth = maxHealth;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
    }

    private void OnEnable()
    {
        this.transform.localPosition = Spawner.Instance.Spawn().gameObject.transform.position;
        this.transform.localRotation = Spawner.Instance.Spawn().gameObject.transform.rotation;
        nav.enabled = true;
        this.gameObject.layer = 12;
        if (enemyTybe != Tybe.D) Invoke("ChaseStart", 2);
    }

    private void Awake()
    {
        if(instance == null) Enemy.instance = this;

        rb = this.GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        isDead = false;
        if (enemyTybe != Tybe.D) Invoke("ChaseStart", 2);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    private void Update()
    {
        if (nav.enabled && enemyTybe != Tybe.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
        
    }

    void Targeting()
    {
        if(!isDead && enemyTybe != Tybe.D)
        {
            float targetRadius = 0;
            float targetRange = 0;

            switch (enemyTybe)
            {
                case Tybe.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Tybe.B:
                    targetRadius = 1f;
                    targetRange = 12f;
                    break;
                case Tybe.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }

            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
                                                         targetRadius, transform.forward,
                                                         targetRange, LayerMask.GetMask("Player"));
            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
        
        switch(enemyTybe)
        {
            case Tybe.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case Tybe.B:
                yield return new WaitForSeconds(0.1f);
                rb.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rb.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;
            case Tybe.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet, transform.position + (Vector3.up * 2), transform.rotation);
                Rigidbody rbBullet = instantBullet.GetComponent<Rigidbody>();
                rbBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }

        

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    void FreezeVelocity()
    {
        if(rb.velocity == Vector3.zero)
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
        }

        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            currentHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));
        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            currentHealth -= bullet.damage;
            Destroy(other.gameObject);
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));
        }
        
    }

    

    public void HitByGrenade(Vector3 explosionPos)
    {
        currentHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        if (currentHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.white;
            }
        }
        else
        {
            foreach(MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.gray;
            }

            gameObject.layer = 13;
            isDead = true;
            isChase = false;
            
            
            nav.enabled = false;
            anim.SetTrigger("doDie");
            Player.instance.score += score;
            

            switch(enemyTybe)
            {

                case Tybe.A:
                    if (manager.enemyCnt_A == 0) break;
                    manager.enemyCnt_A--;
                    break;
                case Tybe.B:
                    if (manager.enemyCnt_B == 0) break;
                    manager.enemyCnt_B--;
                    break;
                case Tybe.C:
                    if (manager.enemyCnt_C == 0) break;
                    manager.enemyCnt_C--;
                    break;
            }

            if(isGrenade)
            {
                reactVec = reactVec.normalized;
                //수류탄과 방향
                Vector3 vec = (transform.position - Grenade.instance.transform.position).normalized;
                //그 방향을 90도로 돌림 -> 90도 돌린 벡터축을 기준으로 오브젝트가 돌아감
                Vector3 dir = Quaternion.AngleAxis(90, Vector3.up) * vec;

                reactVec += Vector3.up * 3;

                rb.freezeRotation = false;
                rb.AddForce(reactVec * 5, ForceMode.Impulse);
                rb.AddTorque(dir * 15, ForceMode.Impulse);

            }
            else
            {
                this.rb.AddForce(Vector3.up * 3, ForceMode.Impulse);
            }

            this.gameObject.SetActive(false);
            //Destroy(gameObject, 4);

            int ranCoin = Random.Range(0, 3);
            yield return new WaitForSeconds(1.5f);
            Instantiate(coins[ranCoin], new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
        }
        
    }

    

    


}

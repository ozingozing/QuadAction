using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
{
    public static BossMissile Instance;
    public Transform target;
    NavMeshAgent nav;

    void Awake()
    {
        if(Instance == null) BossMissile.Instance = this;
        nav = GetComponent<NavMeshAgent>();
        StartCoroutine(LifeTime());
    }

    public void BossDeadCheck()
    {
        if (Boss.Instance.isDead)
        {
            Debug.Log("º¸½ºÁ×À½");
            this.gameObject.SetActive(false);
        }
    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(7f);
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        nav.SetDestination(target.position);
    }
}

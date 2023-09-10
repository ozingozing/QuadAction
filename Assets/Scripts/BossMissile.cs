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
        Application.targetFrameRate = 144;
        if (Instance == null) BossMissile.Instance = this;
        nav = GetComponent<NavMeshAgent>();
        StartCoroutine(LifeTime());
    }

    public void BossDeadCheck()
    {

        Debug.Log("º¸½ºÁ×À½");
        this.nav.isStopped = true;
        Destroy(this.gameObject);

    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(7f);
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if(Boss.Instance.isDead)
        {
            BossDeadCheck();
        }
        else
        nav.SetDestination(target.position);
    }
}

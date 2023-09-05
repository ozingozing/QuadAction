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
    }


    void Update()
    {
        nav.SetDestination(target.position);
    }
}

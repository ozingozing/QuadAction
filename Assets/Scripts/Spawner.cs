using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance;
    public Transform[] spawnPoint;

    float timer;

    private void Awake()
    {
        if(Instance == null) Spawner .Instance = this;
        //spawnPoint = GetComponentsInChildren<Transform>();
    }
    void Update()
    {
        
    }

    public Transform Spawn()
    {
        //GameObject enemy = GameManager.Instance.pool.Get(UnityEngine.Random.Range(0,4));
        //enemy.transform.position = spawnPoint[UnityEngine.Random.Range(1, spawnPoint.Length)].transform.position;
        return spawnPoint[UnityEngine.Random.Range(0, 4)];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Incetance;
    public EnemyPool enemyPool;
    //다양한 프리팹 종류
    public GameObject[] prefabs;
    //다양한 프리팹들 중 1개의 종류, 그리고 그 1개 종류의 프리팹이 몇개난 있는지
    public List<GameObject>[] pools;

    private void Awake()
    {
        if (Incetance == null) PoolManager.Incetance = this;
    }

    public virtual GameObject Get(int index)
    {
        Debug.Log("PoolManager 자식 Get함수를 쓰렴~");
        return null;
    }

}

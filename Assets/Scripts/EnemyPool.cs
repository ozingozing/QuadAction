using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : PoolManager
{
    private void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];

        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }

        Debug.Log(pools.Length);
    }


    public override GameObject Get(int index)
    {
        Debug.Log("EnemyPool Get()");
        GameObject select = null;

        //... 선택한 풀에 놀고 있는(비활성화된) 겜오브젝트 접근

        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                //... 발견하면 select변수에 할당
                select = item;
                select.SetActive(true);
                break;
            }
        }


        //... 다 쓰고 있는 상태이면(전부 활성화된 상태)?


        if (!select)
        {
            //... 새롭게 생성하고 select에 할당
            select = Instantiate(prefabs[index],
                                Spawner.Instance.Spawn().gameObject.transform.position,
                                Spawner.Instance.Spawn().gameObject.transform.rotation,
                                this.transform);
            pools[index].Add(select);
        }

        return select;
    }
}

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

        //... ������ Ǯ�� ��� �ִ�(��Ȱ��ȭ��) �׿�����Ʈ ����

        foreach (GameObject item in pools[index])
        {
            if (!item.activeSelf)
            {
                //... �߰��ϸ� select������ �Ҵ�
                select = item;
                select.SetActive(true);
                break;
            }
        }


        //... �� ���� �ִ� �����̸�(���� Ȱ��ȭ�� ����)?


        if (!select)
        {
            //... ���Ӱ� �����ϰ� select�� �Ҵ�
            select = Instantiate(prefabs[index],
                                Spawner.Instance.Spawn().gameObject.transform.position,
                                Spawner.Instance.Spawn().gameObject.transform.rotation,
                                this.transform);
            pools[index].Add(select);
        }

        return select;
    }
}

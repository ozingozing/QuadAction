using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Incetance;
    public EnemyPool enemyPool;
    //�پ��� ������ ����
    public GameObject[] prefabs;
    //�پ��� �����յ� �� 1���� ����, �׸��� �� 1�� ������ �������� ��� �ִ���
    public List<GameObject>[] pools;

    private void Awake()
    {
        if (Incetance == null) PoolManager.Incetance = this;
    }

    public virtual GameObject Get(int index)
    {
        Debug.Log("PoolManager �ڽ� Get�Լ��� ����~");
        return null;
    }

}

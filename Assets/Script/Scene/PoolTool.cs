using System;
using UnityEngine;
using UnityEngine.Pool;

public class PoolTool : MonoBehaviour
{
    public GameObject objPrefab;

    private ObjectPool<GameObject> pool;

    private void Start()
    {
        //初始化对象池
        pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(objPrefab, transform),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize:20
        );

        PreFillPool(5);
    }

    //预先填满对象池
    private void PreFillPool(int count)
    {
        var preFillArray = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            preFillArray[i] = pool.Get();
        }

        foreach (var item in preFillArray)
        {
            pool.Release(item);
        }
    }

    //从对象池获取对象
    public GameObject GetObjectFromPool()
    {
        return pool.Get();
    }

    //将对象释放回对象池
    public void ReleaseObjectPool(GameObject obj)
    {
        pool.Release(obj);
    }
}

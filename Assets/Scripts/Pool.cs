using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    protected internal List<PoolObject> inactiveObjects;
    protected internal List<PoolObject> activeObjects;
    public bool IsPoolEmpty => inactiveObjects.Count == 0;
    public int LeftInPool => inactiveObjects.Count;

    protected internal void InitializePool() {
        // create a pool from the children of the transform
        inactiveObjects = new List<PoolObject>();
        activeObjects = new List<PoolObject>();

        for (int i = 0; i < transform.childCount; i++) {
            Transform child = transform.GetChild(i);
            if (child.TryGetComponent<PoolObject>(out PoolObject obj)) {
                inactiveObjects.Add(obj);
            }
        }
    }
    protected internal PoolObject TakeFromPool(int index) {
        PoolObject obj = inactiveObjects[index];

        inactiveObjects.RemoveAt(index);
        activeObjects.Add(obj);

        obj.TakeFromPool(this);
        return obj;
    }

    protected internal PoolObject Take() {
        return TakeFromPool(0);
    }

    public void ReturnToPool(PoolObject obj) {
        // find object in active objects
        activeObjects.Remove(obj);
        inactiveObjects.Add(obj);
    }

    public void Clear() {
        Stack<PoolObject> objectsToKill = new Stack<PoolObject>();
        foreach (PoolObject activeObject in activeObjects) {
            objectsToKill.Push(activeObject);
        }
        while (objectsToKill.Count > 0) {
            objectsToKill.Pop().Die();
        }
    }
}

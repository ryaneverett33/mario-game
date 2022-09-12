using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    Vector2 originalPoolPosition;
    bool inPool = true;
    Pool owningPool;
    // public GameObject gameObject;
    // Transform transform => gameObject.transform;

    public void TakeFromPool(Pool pool) {
        originalPoolPosition = transform.position;
        owningPool = pool;

        transform.parent = null;
        inPool = false;
    }
    void PutBackInPool() {
        transform.position = originalPoolPosition;
        transform.parent = owningPool.transform;

        originalPoolPosition = Vector2.zero;
        owningPool.ReturnToPool(this);
        inPool = true;
    }

    public void Die() {
        PutBackInPool();
        
        // Return to Object Pool
        gameObject.SetActive(false);
    }
}

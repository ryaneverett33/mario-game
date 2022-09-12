using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsPool : Pool
{
    void Awake() {
        InitializePool();
    }

    public GameObject GetExplosion() {
        if (IsPoolEmpty) { return null; }

        return Take().gameObject;
    }
}

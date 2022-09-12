using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : Pool
{
    private System.Random randomObj;

    void Awake() {
        randomObj = new System.Random();
        InitializePool();
    }

    public GameObject GetRandomEnemy() {
        if (IsPoolEmpty) { return null; }

        return TakeFromPool(randomObj.Next(LeftInPool)).gameObject;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : PoolObject
{
    public int PointsAmount;

    private float startTime;
    private bool usable;
    private float lengthOfLife = 6;

    // Start is called before the first frame update
    void OnEnable() {
        transform.localScale = new Vector3(5, 5, 1);
        usable = true;
        startTime = Time.fixedTime;
    }

    void Update() {
        if ((Time.fixedTime - startTime) > lengthOfLife && usable) {
            usable = false;
            StartCoroutine(fadeOut());
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        Die();
    }

    IEnumerator fadeOut() {
        float start = transform.localScale.x;
        float currentScaleNorm = transform.localScale.x;
        Vector3 currentScale = transform.localScale;

        for (int i = 0; i < 30; i++) {
            currentScaleNorm = Mathf.SmoothDamp(start, 0.5f, ref start, 1.8f);
            currentScale.x = currentScaleNorm;
            currentScale.y = currentScaleNorm;
            transform.localScale = currentScale;
            yield return new WaitForSeconds(2 / 30);
        }
        Die();
    }
}

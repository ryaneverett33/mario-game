using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : PoolObject
{
    float startTime;
    float lengthOfLife = 3;
    static float FiringSpeed = 60;
    readonly static float baseFiringSpeed = 60;
    // Start is called before the first frame update
    void OnEnable()
    {
        startTime = Time.fixedTime;
        Vector2 velocity = Vector2.zero;
        velocity.y = -1 * FiringSpeed;
        GetComponent<Rigidbody2D>().velocity = velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.fixedTime - startTime) > lengthOfLife) {
            Die();
        }
    }

    public static void RecalculateDifficulty(int difficulty) {
        float difficultyTime = (float)difficulty / 50;
        FiringSpeed = Mathf.Lerp(baseFiringSpeed, 250, difficultyTime);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        // Explode and die
        foreach (ContactPoint2D contact in collision.contacts) {
            GameObject explosion = GameManager.getEffectsPool().GetExplosion();
            explosion.transform.position = contact.point;
            explosion.SetActive(true);
        }
        Die();
    }
}

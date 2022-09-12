using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : PoolObject
{
    float startTime;
    float lengthOfLife = 3;
    static float Mass = 5;
    static float GravityScale = 5;
    readonly static float baseMass = 5;
    readonly static float baseGravityScale = 5;
    // Start is called before the first frame update
    void OnEnable()
    {
        startTime = Time.fixedTime;
        GetComponent<Rigidbody2D>().mass = Mass;
        GetComponent<Rigidbody2D>().gravityScale = GravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.fixedTime - startTime) > lengthOfLife) {
            Die();
        }
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

    public static void RecalculateDifficulty(int difficulty) {
        float difficultyTime = (float)difficulty / 50;
        Mass = Mathf.Lerp(baseMass, 100, difficultyTime);
        GravityScale = Mathf.Lerp(baseGravityScale, 25, difficultyTime);
    }
}

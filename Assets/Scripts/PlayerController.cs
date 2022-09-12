using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float CurrentSpeed;
    public int horizontalMove = 0;
    public int horizontalDecreaseAmnt = 25;

    private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer renderer;
    private Vector2 velocity = Vector2.zero;
    private float targetSpeed = 130; 
    private float movementSmoothing = 0.15f;
    private float brakeSmoothing = 0.05f;
    public int turnAroundSpeed = 50;
    private bool facingRight = true;
    private bool alive = true;

    private int animatorRunningHash;
    private int animatorTurnAroundHash;
    private int animatorDeadHash;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();

        // animator behavior hashes
        animatorRunningHash = Animator.StringToHash("running");
        animatorTurnAroundHash = Animator.StringToHash("turnAround");
        animatorDeadHash = Animator.StringToHash("dead");
    }

    // Update is called once per frame
    void Update()
    {
        CurrentSpeed = rigidbody.velocity.x < 0 ? (rigidbody.velocity.x * -1) : rigidbody.velocity.x;
        animator.SetBool(animatorRunningHash, CurrentSpeed > 3.0f);

#if !UNITY_IOS && !UNITY_ANDROID
        horizontalMove = (int)(Input.GetAxis("Horizontal") * 100);
#endif

        if (Input.GetKeyDown(KeyCode.P)) {
            GameManager.getInstance().TogglePause();
        }
    }
    void FixedUpdate() {
        if (!alive) {
            return;
        }
        // draw mario the right way
        if ((int)horizontalMove > 5 && !facingRight) {
            flip();
        }
        else if ((int)horizontalMove < -5 && facingRight) {
            flip();
        }

        if ((int)horizontalMove > 5 || (int)horizontalMove < -5) {
            Vector2 targetVelocity = new Vector2(targetSpeed * ((float)horizontalMove / 100), rigidbody.velocity.y);
			rigidbody.velocity = Vector2.SmoothDamp(rigidbody.velocity, targetVelocity, ref velocity, movementSmoothing);
        }
        else {
            Vector2 targetVelocity = new Vector2(0, rigidbody.velocity.y);
            rigidbody.velocity = Vector2.SmoothDamp(rigidbody.velocity, targetVelocity, ref velocity, brakeSmoothing);
        }
        
        // decrease horizontalmove for touch controls
        if (horizontalMove < 0) {
            int amount = horizontalDecreaseAmnt;
            if (horizontalMove + amount > 0) {
                amount = 0 - horizontalMove;
            }
            horizontalMove += amount;
        }
        else if (horizontalMove > 0) {
            int amount = horizontalDecreaseAmnt;
            if (horizontalMove - amount < 0) {
                amount = horizontalMove;
            }
            horizontalMove -= amount;
        }
    }

    void flip() {
        renderer.flipX = facingRight;
        facingRight = !facingRight;

        if ((int)CurrentSpeed > turnAroundSpeed) {
            animator.SetTrigger(animatorTurnAroundHash);
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        if (!alive) { return; }
        if (col.gameObject.tag == "enemy") {
            if (GameManager.getInstance().PlayerHit(col.gameObject.name)) {
                // die
                animator.SetBool(animatorDeadHash, true);
                alive = false;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collider) {
        if (!alive) { return; }
        if (collider.gameObject.tag == "collectable") {
            if (collider.gameObject.TryGetComponent<Coin>(out Coin coinComp)) {
                GameManager.getInstance().PlayerAddPoints(coinComp.PointsAmount);
            }
        }
    }

    public void MoveLeft() {
        horizontalMove -= 100;
        horizontalMove = Mathf.Clamp(horizontalMove, -100, 100);
    }
    
    public void MoveRight() {
        horizontalMove += 100;
        horizontalMove = Mathf.Clamp(horizontalMove, -100, 100);
    }

    public void Reset() {
        animator.SetBool(animatorDeadHash, false);
        alive = true;
    }
}

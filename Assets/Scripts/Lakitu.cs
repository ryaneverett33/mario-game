using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lakitu : MonoBehaviour
{
    public float CurrentSpeed;
    public int LakituIndex;
    public AudioClip CannonNoise;

    [SerializeField] private float moveSpeed = 30;
    [SerializeField] private float dropRadius = 10;
    [SerializeField] private float movementSmoothing = 0.05f;
    [SerializeField] private float cooldownTime = 2;
    [SerializeField] private float patrolMoveSpeed = 30;

    private readonly float baseMoveSpeed = 30;
    private readonly float baseDropRadius = 10;
    private readonly float baseCooldownTime = 2;
    private readonly float basePatrolMoveSpeed = 30;
    private Animator animController;
    private Rigidbody2D rigidbody;
    private GameObject playerObj;
    private bool droppingItem = false;
    private Vector2 velocity = Vector2.zero;
    private GameObject dropPointTransform;

    private AudioSource audioPlayer;

    // Bounds Info
    public float safetyBoundMargin = 25;
    private float screenMiddle, leftBound, rightBound, leftSafetyBound, rightSafetyBound;
    private float lastPatrolPoint; 

    // Start is called before the first frame update
    void Start()
    {
        animController = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        playerObj = GameManager.getInstance().Player;
        dropPointTransform = transform.GetChild(0).gameObject;
        audioPlayer = GetComponent<AudioSource>();

        // Calculate boundaries (addition because we cross the y axis)
        screenMiddle = GameManager.getInstance().RightWall.transform.position.x + 
                       GameManager.getInstance().LeftWall.transform.position.x;
        float boundarySpan = (GameManager.getInstance().RightWall.transform.position.x - screenMiddle) / 2;
        float[] boundPositions = {
            GameManager.getInstance().LeftWall.transform.position.x,
            screenMiddle - boundarySpan,
            screenMiddle,
            screenMiddle + boundarySpan
        };

        leftBound = boundPositions[LakituIndex];
        rightBound = boundPositions[LakituIndex] + boundarySpan;
        leftSafetyBound = LakituIndex == 0 ? leftBound : leftBound + safetyBoundMargin;
        rightSafetyBound = LakituIndex == 3 ? rightBound : rightBound - safetyBoundMargin; 
        
        //Debug.Log($"{LakituIndex} Middle of screen: {screenMiddle}");
        //Debug.Log($"{LakituIndex} Boundaries: [{leftBound}, {rightBound}], Safety Boundaries: [{leftSafetyBound}, {rightSafetyBound}]");

        // Set last patrol point
        lastPatrolPoint = rightSafetyBound;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentSpeed = rigidbody.velocity.x < 0 ? (rigidbody.velocity.x * -1) : rigidbody.velocity.x;
    }

    void FixedUpdate() {
        if (!droppingItem) {
            // check if player is beneath lakitu
            if (isLakituAtX(playerObj.transform.position.x)) {
                rigidbody.velocity = Vector2.zero;
                StartCoroutine(dropItem());
            }
            else {
                if (isPlayerInQuadrant()) {
                    // move lakitu towards player
                    moveTowardsX(playerObj.transform.position.x, moveSpeed);
                }
                else {
                    // patrol lakitu
                    if (isLakituAtX(lastPatrolPoint)) {
                        lastPatrolPoint = (lastPatrolPoint == leftSafetyBound) ? rightSafetyBound : leftSafetyBound;
                    }

                    // move towards lastPatrolPoint
                    moveTowardsX(lastPatrolPoint, patrolMoveSpeed);
                }
            }
        }
    }

    private bool isPlayerInQuadrant() {
        return playerObj.transform.position.x > leftBound &&
                playerObj.transform.position.x < rightBound;
    }
    private bool isLakituAtX(float x) {
        return x < (transform.position.x + dropRadius) &&
               x > (transform.position.x - dropRadius);
    }
    private void moveTowardsX(float x, float moveSpeed) {
        float direction = x > transform.position.x ? 1 : -1;
        Vector2 targetVelocity = new Vector2(moveSpeed * direction, rigidbody.velocity.y);
        rigidbody.velocity = Vector2.SmoothDamp(rigidbody.velocity, targetVelocity, ref velocity, movementSmoothing);
    }

    public void RecalculateDifficulty(int difficulty) {
        float difficultyTime = (float)difficulty / 50;
        float extendedDiffTime = (float)difficulty / 75;
        moveSpeed = Mathf.Lerp(baseMoveSpeed, 250, difficultyTime);
        dropRadius = Mathf.Lerp(baseDropRadius, 1.0f, difficultyTime);
        cooldownTime = Mathf.Lerp(baseCooldownTime, 0.01f, extendedDiffTime);
        patrolMoveSpeed = Mathf.Lerp(basePatrolMoveSpeed, 45, difficultyTime);
    }

    IEnumerator dropItem() {
        animController.SetTrigger("Drop");
        droppingItem = true;
        yield return new WaitForSeconds(0.5f);

        GameObject newEnemy = GameManager.getEnemyPool().GetRandomEnemy();
        newEnemy.transform.position = dropPointTransform.transform.position;
        newEnemy.SetActive(true);
        audioPlayer.PlayOneShot(CannonNoise);

        // cooldown
        yield return new WaitForSeconds(cooldownTime);
        droppingItem = false;
    }
}

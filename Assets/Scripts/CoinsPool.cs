using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsPool : Pool
{
    public GameObject Ground;

    private System.Random randomObj;
    private readonly float hoverOffset = 10;
    private readonly float wallOffset = 5;
    private readonly float coinRadius = 5;
    private float leftWallPos, rightWallPos, groundPos;
    private static float coinHeight = 0;

    void Awake() {
        randomObj = new System.Random();
        Transform aCoin = transform.GetChild(0);

        GameObject LeftWall = GameManager.getInstance().LeftWall;
        GameObject RightWall = GameManager.getInstance().RightWall;
        leftWallPos = LeftWall.transform.position.x;
        leftWallPos += (LeftWall.GetComponent<BoxCollider2D>().size.x / 2);
        leftWallPos += wallOffset;

        rightWallPos = RightWall.transform.position.x;
        rightWallPos -= (RightWall.GetComponent<BoxCollider2D>().size.x / 2);
        rightWallPos -= wallOffset;

        groundPos = Ground.transform.position.y;
        groundPos += Ground.GetComponent<BoxCollider2D>().offset.y; // The ground object has a broken pivot point so pos.y is a lie
        groundPos += (Ground.GetComponent<BoxCollider2D>().size.y / 2) ;

        //Debug.Log($"Left Wall: {leftWallPos}, Right Wall: {rightWallPos}, GroundPos: {groundPos}");
        InitializePool();
    }

    public void SpawnCoin() {
        // spawn coin
        if (IsPoolEmpty) { return; }
        GameObject coin = Take().gameObject;

        if (coinHeight == 0) {
            coinHeight = coin.GetComponent<BoxCollider2D>().size.y;
        }

        // move coin to position
        // position is above ground randomly between left and right wall
        Vector2 position = Vector2.zero;
        position.y = groundPos + coinHeight + hoverOffset;

        // safe area between walls
        float span = rightWallPos - leftWallPos;
        for (int attempt = 0; attempt < 5; attempt++) {
            float offsetFromLeftWall = (float)(randomObj.NextDouble() * span);
            position.x = offsetFromLeftWall + leftWallPos;  // random starts at 0, so shift back starting position to the left wall
            
            // are we overlapping another coin?
            if (Physics2D.BoxCast(position, new Vector2(coinHeight, coinHeight), 0, Vector2.right, coinRadius, 7) || 
                Physics2D.BoxCast(position, new Vector2(coinHeight, coinHeight), 0, Vector2.left, coinRadius, 7)) {
                continue;
            }
        }

        int pointsAmount = 0;
        for (int i = 0; i < 5; i++) {
            pointsAmount = randomObj.Next(50);
            if (pointsAmount != 0) { break; }
        }
        coin.GetComponent<Coin>().PointsAmount = pointsAmount;

        coin.transform.position = position;
        coin.SetActive(true);
    }
}

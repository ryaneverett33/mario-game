using System;
using UnityEngine;

[Serializable]
public class LeaderboardScore {
    public string enemy, name;
    public int score;

    public LeaderboardScore(string enemy, string name, int score) {
        this.enemy = enemy;
        this.name = name;
        this.score = score;
    }
    public LeaderboardScore() {}
}
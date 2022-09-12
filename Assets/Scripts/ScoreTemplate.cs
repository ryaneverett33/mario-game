using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTemplate : MonoBehaviour
{
    public Text PlaceText, PlayerName, Score;
    public Image EnemyImage, Panel;
    void Awake()
    {
        Transform panelObj = transform.GetChild(0);
        Panel = panelObj.GetComponent<Image>();
        PlaceText = panelObj.GetChild(0).GetComponent<Text>();
        EnemyImage = panelObj.GetChild(1).GetComponent<Image>();
        PlayerName = panelObj.GetChild(2).GetComponent<Text>();
        Score = panelObj.GetChild(3).GetComponent<Text>();
    }
}

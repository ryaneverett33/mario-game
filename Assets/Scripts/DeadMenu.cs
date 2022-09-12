using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DeadMenu : MonoBehaviour
{
    private Button restartBtn, leaderboardBtn, submitBtn;
    private InputField nameInput;
    private GameObject highScoreTextObj, failedTextObj;
    private LeaderboardMenu leaderboardMenu;
    private string killedByEnemy;
    private int playerScore;
    
    // Start is called before the first frame update
    void Awake()
    {
        restartBtn = transform.GetChild(1).GetComponent<Button>();
        leaderboardBtn = transform.GetChild(2).GetComponent<Button>();
        highScoreTextObj = transform.GetChild(3).gameObject;
        nameInput = transform.GetChild(4).GetComponent<InputField>();

        Transform submitBtnObj = transform.GetChild(5);
        submitBtn = submitBtnObj.GetComponent<Button>();
        failedTextObj = submitBtnObj.GetChild(1).gameObject;

        Debug.Log(transform.parent.transform.GetChild(1).gameObject);
        leaderboardMenu = transform.parent.transform.GetChild(1).GetComponent<LeaderboardMenu>();

        restartBtn.onClick.AddListener(restartClick);
        leaderboardBtn.onClick.AddListener(leaderboardClick);
        submitBtn.onClick.AddListener(submitClick);
    }

    void restartClick() {
        GameManager.getInstance().Restart();
    }
    void leaderboardClick() {
        leaderboardMenu.Show();
    }
    void submitClick() {
        if (nameInput.text.Length == 0) {
            return;
        }
        submitBtn.interactable = false;
        nameInput.interactable = false;
        StartCoroutine(submitScore(nameInput.text));
    }
    public void Show(bool newHighScore, int score, string enemy) {
        gameObject.SetActive(true);
        submitBtn.interactable = true;
        nameInput.interactable = true;
        playerScore = score;
        killedByEnemy = enemy;
        highScoreTextObj.SetActive(newHighScore);
        failedTextObj.SetActive(false);
    }
    public void Show() {
        gameObject.SetActive(true);
    }
    public void Hide() {
        gameObject.SetActive(false);
    }

    IEnumerator submitScore(string name) {
        LeaderboardScore submission = new LeaderboardScore(killedByEnemy, name, playerScore);
        string json = JsonUtility.ToJson(submission);

        using (UnityWebRequest www = UnityWebRequest.Put($"{GameManager.ServerURL}/submit", json))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                failedTextObj.SetActive(true);
            }
        }
    }
}
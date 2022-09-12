using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LeaderboardMenu : MonoBehaviour
{
    private Button closeButton;
    private Text statusText;
    private List<LeaderboardScore> scores;
    private List<GameObject> scoreObjects;
    public GameObject ScoreTemplatePrefab;
    private GameObject scrollView, scrollViewContent;
    private DeadMenu deadMenu;

    public Sprite bulletBillImage, spikesImage, fireballImage;
    // Start is called before the first frame update
    void Awake()
    {
        scores = new List<LeaderboardScore>();
        scoreObjects = new List<GameObject>();
        deadMenu = transform.parent.GetChild(2).GetComponent<DeadMenu>();
        closeButton = transform.GetChild(2).GetComponent<Button>();
        statusText = transform.GetChild(3).GetComponent<Text>();

        //scrollContent = transform.GetChild(1).transform.GetChild(0).transform.GetChild(0);
        scrollView = transform.GetChild(1).gameObject;
        scrollViewContent = scrollView.transform.GetChild(0).transform.GetChild(0).gameObject;


        closeButton.onClick.AddListener(closeClick);
    }

    public void Show() {
        gameObject.SetActive(true);
        deadMenu.Hide();
        StartCoroutine(getScores());
    }
    
    void closeClick() {
        gameObject.SetActive(false);
        deadMenu.Show();
    }
    void clearScores() {
        scores.Clear();
        foreach (GameObject obj in scoreObjects) {
            GameObject.Destroy(obj);
        }
        scoreObjects.Clear();
    }

    IEnumerator getScores() {
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{GameManager.ServerURL}/scores?type=text"))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success) {
                Debug.Log(webRequest.downloadHandler.text);
                string[] scoresText = webRequest.downloadHandler.text.Split('\n');
                clearScores();
                foreach (string scoreText in scoresText) {
                    if (scoreText.Length == 0) { continue; }

                    LeaderboardScore parsedObj = new LeaderboardScore();
                    int currIndex = 0;
                    for (int i = 0; i < 3; i++) {
                        currIndex += 1;
                        int nextIndex = scoreText.IndexOf('"', currIndex);
                        string substring = scoreText.Substring(currIndex, nextIndex - currIndex);
                        currIndex += (nextIndex - currIndex) + 1;
                        currIndex += 1;

                        if (i == 0) {
                            parsedObj.score = int.Parse(substring);
                        }
                        else if (i == 1) { parsedObj.enemy = substring; }
                        else { parsedObj.name = substring; }
                    }
                    scores.Add(parsedObj);
                }
                statusText.gameObject.SetActive(false);
                yield return displayScores();
            }
            else {
                statusText.gameObject.SetActive(false);
                statusText.text = $"Failed to load scores, response {webRequest.responseCode}";
            }
        }
    }
    IEnumerator displayScores() {
        for (int i = 0; i < scores.Count; i++) {
            GameObject tmp = GameObject.Instantiate(ScoreTemplatePrefab, scrollViewContent.transform);
            ScoreTemplate obj = tmp.GetComponent<ScoreTemplate>();
            LeaderboardScore score = scores[i];
            obj.PlayerName.text = score.name;
            obj.PlaceText.text = $"#{i + 1}";
            obj.Score.text = score.score.ToString("D7");
            if (score.enemy == "fireball") {
                obj.EnemyImage.sprite = fireballImage;
            }
            else if (score.enemy == "spikes") {
                obj.EnemyImage.sprite = spikesImage;
            }
            else {
                obj.EnemyImage.sprite = bulletBillImage;
            }

            if (i % 2 != 0) {
                Color currentColor = obj.Panel.color;
                currentColor.a = 175;
                obj.Panel.color = currentColor;
            }
            scoreObjects.Add(tmp);
        }
        yield return new WaitForEndOfFrame();
    }
}

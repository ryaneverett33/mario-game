using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject LeftWall;
    public GameObject RightWall;
    public GameObject Player;
    public GameObject LeftLakitu, LeftLeftLakitu, RightLakitu, RightRightLakitu;
    public GameObject UI;
    public GameObject EnemyPool;
    public GameObject EffectsPool;
    public GameObject CoinsPool;

    // Sounds
    public AudioClip PauseSound, GameOverSound, CoinNoise, ExplosionSound;
    public AudioClip[] Themes;
    private int themeIndex = 0;
    private AudioSource musicPlayer, sfxPlayer;

    private PlayerController playerController;
    private Lakitu[] LakituControllers;
    private GameObject inGameUI, leaderboardMenu;
    private DeadMenu deadMenu;
    private Button leftBtn, rightBtn;
    private Text healthValue, pointsValue, pauseText;
    
    // Player Stats
    private int playerHealth = 3;           // start with three lives
    private int playerPoints = 0;
    private int lastHighScore = 0;
    private string lastEnemyName;

    public bool isPaused;

    // Game Stats
    public TimerDriver TimerDriver;
    public float SpawnRate = 1;
    public float DifficultyIncreaseRate = 2;
    private int StartingDifficulty = 1;
    public int CurrentDifficulty;

    // Pools
    private EnemyPool enemyPool; 
    private EffectsPool effectsPool;
    private CoinsPool coinsPool;

    public static readonly string ServerURL = "http://changer098.duckdns.org/projects/mario-game/leaderboard";
    private static GameManager managerObj;
    private System.Random randomObj;

    void Awake() {
        managerObj = this;
        randomObj = new System.Random();

        // setup edge colliders
        Camera cam = Camera.main;
        Vector3 left = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 right = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, cam.nearClipPlane));
        left.y = LeftWall.transform.position.y;
        left.z = LeftWall.transform.position.z;
        right.y = RightWall.transform.position.y;
        right.z = RightWall.transform.position.z;
        LeftWall.transform.position = left;
        RightWall.transform.position = right;
        isPaused = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        // get components
        playerController = Player.GetComponent<PlayerController>();
        LakituControllers = new Lakitu[4];
        LakituControllers[0] = LeftLakitu.GetComponent<Lakitu>();
        LakituControllers[1] = LeftLeftLakitu.GetComponent<Lakitu>();
        LakituControllers[2] = RightLakitu.GetComponent<Lakitu>();
        LakituControllers[3] = RightRightLakitu.GetComponent<Lakitu>();
        TimerDriver = GetComponent<TimerDriver>();

        // get UI components
        inGameUI = UI.transform.GetChild(0).gameObject;
        leaderboardMenu = UI.transform.GetChild(1).gameObject;
        deadMenu = UI.transform.GetChild(2).gameObject.GetComponent<DeadMenu>();
        pointsValue = inGameUI.transform.GetChild(2).GetComponent<Text>();
        healthValue = inGameUI.transform.GetChild(4).GetComponent<Text>();
        leftBtn = inGameUI.transform.GetChild(0).GetComponent<Button>();
        rightBtn = inGameUI.transform.GetChild(1).GetComponent<Button>();
        pauseText = inGameUI.transform.GetChild(6).GetComponent<Text>();
        
        // if mobile, enable touchscreen buttons
        if (SystemInfo.deviceType == DeviceType.Handheld) {
            leftBtn.gameObject.SetActive(true);
            rightBtn.gameObject.SetActive(true);
        }

        // setup click listeners
        leftBtn.onClick.AddListener(playerController.MoveLeft);
        rightBtn.onClick.AddListener(playerController.MoveRight);

        // get pool components
        enemyPool = EnemyPool.GetComponent<EnemyPool>();
        effectsPool = EffectsPool.GetComponent<EffectsPool>();
        coinsPool = CoinsPool.GetComponent<CoinsPool>();

        // Physics ignores
        Physics2D.IgnoreLayerCollision(6, 3);
        Physics2D.IgnoreLayerCollision(6, 7);

        // Start timers
        TimerDriver.CreateTimer(SpawnRate, SpawnCoinsTimer);
        TimerDriver.CreateTimer(DifficultyIncreaseRate, IncreaseDifficultyTimer);

        // Music
        AudioSource[] sources = GetComponents<AudioSource>();
        musicPlayer = sources[0];
        sfxPlayer = sources[1];

        CurrentDifficulty = StartingDifficulty - 1;
        IncreaseDifficultyTimer();
        coinsPool.SpawnCoin();
        coinsPool.SpawnCoin();
        coinsPool.SpawnCoin();

        musicPlayer.clip = Themes[0];
        themeIndex = 0;
        musicPlayer.Play();
    }

    void Update() {
        if (!musicPlayer.isPlaying && Time.timeScale != 0) {
            // Start playing next song
            themeIndex += 1;
            if (themeIndex >= Themes.Length) { themeIndex = 0; }

            musicPlayer.clip = Themes[themeIndex];
            musicPlayer.Play();
        }
    }

    private void gameSetup() {
        TimerDriver.ResetAllTimers();
        CurrentDifficulty = StartingDifficulty - 1;
        IncreaseDifficultyTimer();
        coinsPool.SpawnCoin();
        coinsPool.SpawnCoin();
        coinsPool.SpawnCoin();
        playerHealth = 3;
        playerPoints = 0;
        healthValue.text = playerHealth.ToString("D2");
        pointsValue.text = playerPoints.ToString("D7");
    }

    public bool PlayerHit(string enemyName) {
        playerHealth -= 1;
        lastEnemyName = enemyName;
        healthValue.text = playerHealth.ToString("D2");
        if (playerHealth == 0) {
            playerDied();
        }
        else {
            sfxPlayer.PlayOneShot(ExplosionSound);
        }
        return playerHealth == 0;
    }

    public void PlayerAddPoints(int points) {
        playerPoints += points;
        pointsValue.text = playerPoints.ToString("D7");
        sfxPlayer.PlayOneShot(CoinNoise, 0.3f);
    }

    // Timer events
    public void SpawnCoinsTimer() {
        int numCoinsToSpawn = randomObj.Next(1, 5);
        for (int i = 0; i < numCoinsToSpawn; i++) {
            coinsPool.SpawnCoin();
        }
    }
    public void IncreaseDifficultyTimer() {
        CurrentDifficulty += 1;
        Debug.Log($"Increased difficulty to {CurrentDifficulty}");
        LakituControllers[0].RecalculateDifficulty(CurrentDifficulty);
        LakituControllers[1].RecalculateDifficulty(CurrentDifficulty);
        LakituControllers[2].RecalculateDifficulty(CurrentDifficulty);
        LakituControllers[3].RecalculateDifficulty(CurrentDifficulty);
        Fireball.RecalculateDifficulty(CurrentDifficulty);
        BulletBill.RecalculateDifficulty(CurrentDifficulty);
        Spikes.RecalculateDifficulty(CurrentDifficulty);
    }

    public static GameManager getInstance() {
        return managerObj;
    }
    public static EnemyPool getEnemyPool() {
        return managerObj.enemyPool;
    }
    public static EffectsPool getEffectsPool() {
        return managerObj.effectsPool;
    }
    public static PlayerController getPlayerController() {
        return managerObj.playerController;
    }
    public void Restart() {
        coinsPool.Clear();
        enemyPool.Clear();
        effectsPool.Clear();
        deadMenu.Hide();
        gameSetup();
        Time.timeScale = 1;
        playerController.Reset();
        CurrentDifficulty = 0;
        IncreaseDifficultyTimer();
        sfxPlayer.Stop();
        musicPlayer.clip = Themes[0];
        themeIndex = 0;
        musicPlayer.Play();
    }
    public bool TogglePause() {
        isPaused = !isPaused;

        if (isPaused) {
            Time.timeScale = 0;
            sfxPlayer.PlayOneShot(PauseSound);
            musicPlayer.Pause();
        }
        else {
            Time.timeScale = 1;
            musicPlayer.UnPause();
        }
        pauseText.gameObject.SetActive(isPaused);
        return isPaused;
    }
    private void playerDied() {
        // handle the player death
        Time.timeScale = 0;
        musicPlayer.Stop();
        sfxPlayer.PlayOneShot(GameOverSound);
        bool newHighScore = false;
        if (playerPoints > lastHighScore) {
            lastHighScore = playerPoints;
            newHighScore = true;
        }
        string cleanedEnemyName = lastEnemyName;
        if (lastEnemyName.Contains("bulletbill")) { cleanedEnemyName = "bulletbill"; }
        else if (lastEnemyName.Contains("spikes")) { cleanedEnemyName = "spikes"; }
        else if (lastEnemyName.Contains("fireball")) { cleanedEnemyName = "fireball"; }

        deadMenu.Show(newHighScore, playerPoints, cleanedEnemyName);
    }
}

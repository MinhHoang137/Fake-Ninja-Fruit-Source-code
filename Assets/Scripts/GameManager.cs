using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // to manipulate TextMeshPro
using UnityEngine.SceneManagement; // tp manipulate scene
using UnityEngine.UI; // to manipulate button
using System.Security.Cryptography;

public class GameManager : MonoBehaviour
{
    public List<GameObject> targets;
    private float spawnRate = 1.5f;
    public float currentSpawnRate;
    private float survivalSpawnRate = 1;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameFinishedText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI gamePausedText;
    public TextMeshProUGUI lifeText;
    public Button restartButton;
    public Button continueButton;
    public Button quitButton;
    public bool isGameActive;
    public GameObject titleScreen;
    private int score = 0;
    private float timeDisplay;
    public int lives;

    public float difficulty; // difficulty that player chose
    private int currentDiff; // difficulty increase over time
    private float increaseRate = 0.995f;

    public bool hasPowerup = false;
    public bool activatePowerup = false;
    public bool frenzyMode;
    private float powerupFrequency = 20;
    private float currentCoolDownPowerup;
    private float svFrenzyCoolDown = 10f;
    private float currentSvFrenzyCoolDown = 0; // cool down frenzy mode for survival mode
    // Start is called before the first frame update
    void Start()
    {
        isGameActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && isGameActive) { Pause(); }
        if (difficulty != 0)
        {
            if (isGameActive)
            {
                timeDisplay -= Time.deltaTime;
                timerText.text = "Time Remain: " + Mathf.Floor(timeDisplay * 100) / 100;
            }
            if (timeDisplay < 0)
            {
                timeDisplay = 0;
                timerText.text = "Time Remain: " + timeDisplay;
                GameFinished();
            }
        }
        else
        {
            if (isGameActive)
            {
                timeDisplay += Time.deltaTime;
                timerText.text = "Time Survived: " + Mathf.Floor(timeDisplay * 100) / 100;
                AddLives(0);
                if (lives == 0) { GameOver(); }
            }
        }
        if (isGameActive)
        {
            if (hasPowerup)
            {
                currentCoolDownPowerup = powerupFrequency;
            }
            else
            {
                currentCoolDownPowerup -= Time.deltaTime;
                if (currentCoolDownPowerup < 0)
                {
                    hasPowerup = true;
                    currentCoolDownPowerup = powerupFrequency;
                }
            }
            if (this.frenzyMode) { 
                Target.frenzyMode = true; 
                Debug.Log(Target.frenzyMode);
            }
            if (!Target.frenzyMode) { currentSvFrenzyCoolDown = svFrenzyCoolDown; }
            if (Target.frenzyMode)
            {
                currentSvFrenzyCoolDown -= Time.deltaTime;
                if (currentSvFrenzyCoolDown <= 0)
                {
                    Target.frenzyMode = false;
                    currentSvFrenzyCoolDown = svFrenzyCoolDown;
                }
            }
        }
    }
    IEnumerator SpawnTarget(float _spawnRate)
    {
        while (isGameActive)
        {
            int index;
            if (this.frenzyMode)
            {
                currentSpawnRate = 0.1f;
                yield return new WaitForSeconds(currentSpawnRate);
                index = Random.Range(1, targets.Count);
                Instantiate(targets[index]);
            }
            else
            {
                currentSpawnRate = _spawnRate * Mathf.Pow(increaseRate, currentDiff);
                yield return new WaitForSeconds(currentSpawnRate);
                if (hasPowerup)
                {
                    index = Random.Range(1, targets.Count);
                }
                else { index = Random.Range(0, targets.Count); }
                Instantiate(targets[index]);
            } 
        }
    }
    public void AddTime(float timeToAdd) 
    {
        timeDisplay += timeToAdd;
    }
    public void AddLives(int livesToAdd)
    {
        if (lives <= 0) return;
        lives += livesToAdd;
        lifeText.text = "Lives: " + lives;
    }
    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }
    public void IncreaseDifficulty()
    {
        currentDiff++;
    }
    public void GameOver()
    {
        CancelInvoke("IncreaseDifficulty");
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        StartCoroutine(IsGameActive(false));
    }
    public void RestartGame()
    {
        restartButton.gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void StartGame(float _difficulty, float _timeRange)
    {
        difficulty = _difficulty;
        currentDiff = 0;
        score = 0;
        isGameActive = true;
        hasPowerup = false;
        frenzyMode = false;
        currentCoolDownPowerup = powerupFrequency;
        InvokeRepeating("IncreaseDifficulty", 1, 1); // decrease spawn rate every second
        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        titleScreen.SetActive(false);
        gameFinishedText.gameObject.SetActive(false);
        if (difficulty != 0)
        {
            timeDisplay = _timeRange;
            StartCoroutine(SpawnTarget(spawnRate / difficulty));
        }
        else
        {
            lives = 3; // 3 lives
            timeDisplay = 0;
            lifeText.gameObject.SetActive(true);
            StartCoroutine(SpawnTarget(survivalSpawnRate)); //1s: spawn rate of survival mode at the start
        }
        UpdateScore(0);
    }
    public void Pause()
    {
        StopCoroutine(SpawnTarget(spawnRate / difficulty));
        CancelInvoke("IncreaseDifficulty");
        continueButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        gamePausedText.gameObject.SetActive(true);
        isGameActive = false;
    }
    public void Continue()
    {
        continueButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        gamePausedText.gameObject.SetActive(false) ;
        isGameActive = true;
        InvokeRepeating("IncreaseDifficulty", 0.7f, 1);
        if (difficulty != 0) { StartCoroutine(SpawnTarget(spawnRate / difficulty)); }
        else { StartCoroutine(SpawnTarget(survivalSpawnRate)); }
    }
    public void Exit()
    {
        Application.Quit();
    }
    IEnumerator IsGameActive(bool _isGameActive)
    {
        yield return new WaitForSeconds(0.1f);
        isGameActive = _isGameActive;
    }
    void GameFinished()
    {
        isGameActive = false;
        gameFinishedText.gameObject.SetActive(true);
        titleScreen.gameObject.SetActive(true);
        titleText.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(true);
        CancelInvoke("IncreaseDifficulty");
    } 
    
}

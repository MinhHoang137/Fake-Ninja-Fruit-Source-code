using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerupManager : MonoBehaviour
{
    public GameManager gameManager;
    public TextMeshProUGUI powerupText;
    public FadingText fadingText;
    public AudioClip powerupSound;
    private AudioSource audioSource;
    private float coolDownFrenzy = 5f;
    private float currentFrenzy;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameActive)
        {
            if (gameManager.activatePowerup)
            {
                powerupText.gameObject.SetActive(true);
                SetPowerup();
                audioSource.PlayOneShot(powerupSound, 1);
                gameManager.activatePowerup = false;
            }
            if (!gameManager.frenzyMode) { currentFrenzy = coolDownFrenzy; }
            if (gameManager.frenzyMode)
            {
                currentFrenzy -= Time.deltaTime;
                if (currentFrenzy <= 0)
                {
                    gameManager.frenzyMode = false;
                    currentFrenzy = coolDownFrenzy;
                }
            }
        }

    }
    void SetPowerup()
    {
        int powerupType;
        if (gameManager.difficulty != 0)
        {
            powerupType = Random.Range(1, 3);
        }
        else
        {
            powerupType = Random.Range(0, 2);
        }
        if (powerupType == 2)
        {
            IncreaseTimeRemaining();
        }
        else if (powerupType == 1)
        {
            SetFrenzyMode();
        }
        else if (powerupType == 0)
        {
            AddLive();
        }
    }
    void IncreaseTimeRemaining()
    {
        float timeToAdd = Random.Range(5, 16) * 1.0f;
        // if the powerup is increase remaining time, time remain += 10
        gameManager.AddTime(timeToAdd);
        powerupText.text = "Add Time: " + timeToAdd;
        fadingText.isActive = true;
    }
    void SetFrenzyMode()
    {
        gameManager.frenzyMode = true;
        Target.frenzyMode = true;
        fadingText.isActive = true;
        powerupText.text = "Frenzy Mode";
    }
    void AddLive()
    {
        int livesToAdd = Random.Range(1, 3);
        fadingText.isActive = true;
        powerupText.text = "Add Lives: " + livesToAdd;
        gameManager.AddLives(livesToAdd); // add a life if get this powerup
    }
}

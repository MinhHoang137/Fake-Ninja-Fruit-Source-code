using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private Rigidbody targetRb;
    private GameManager gameManager;
    private AudioSource cameraAudioSource;
    private TrailRenderer trailRenderer;
    private float minSpeed = 12;
    private float maxSpeed = 16;
    private float maxTorque = 10f;
    private float xRange = 4.5f;
    private float ySpawnPos = -1.5f;
    private float xTilt = 2.5f;
    public float zPos;
    private bool hasPowerup;

    public int pointValue;
    public ParticleSystem explosionParticle;
    public static bool frenzyMode = false;
    // Start is called before the first frame update
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        targetRb = GetComponent<Rigidbody>();
        cameraAudioSource = Camera.main.GetComponent<AudioSource>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        transform.position = RandomSpawnPos();
        targetRb.AddForce(RandomForce(), ForceMode.Impulse);
        targetRb.AddTorque(RandomTorque(), RandomTorque(),
            RandomTorque(), ForceMode.Impulse);
        if (gameManager.hasPowerup) { this.hasPowerup = gameManager.hasPowerup; }
        if (this.hasPowerup)
        {
            trailRenderer.enabled = true;
            gameManager.hasPowerup = false;
        }
        else
        {
            trailRenderer.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -5)
        {
            if (gameManager.difficulty == 0 && !gameObject.CompareTag("Bad")
                && gameManager.isGameActive && !Target.frenzyMode)
            {
                gameManager.AddLives(-1);
            }
            Destroy(gameObject);
        }
    }
    Vector3 RandomForce()
    {
        Vector3 force;
        if (Mathf.Abs(transform.position.x) < xTilt)
        {
            force = new Vector3(Random.Range(-0.5f, 0.5f), 1, 0);
        }
        else if (transform.position.x > xTilt)
        {
            force = new Vector3(Random.Range(-0.5f, 0f), 1, 0);
        }
        else
        {
            force = new Vector3(Random.Range(0f, 0.5f), 1, 0);
        }
        force = force.normalized;
        return force * Random.Range(minSpeed, maxSpeed);
    }
    float RandomTorque()
    {
        return Random.Range(-maxTorque, maxTorque);
    }
    Vector3 RandomSpawnPos()
    {
        return new Vector3(Random.Range(-xRange, xRange), ySpawnPos, zPos);
    }
    private void OnMouseEnter()
    {
        if (gameManager.isGameActive && Input.GetMouseButton(0))
        {
            Instantiate(explosionParticle, transform.position,
                explosionParticle.transform.rotation);
            gameManager.UpdateScore(pointValue);
            if (gameObject.CompareTag("Bad"))
            {
                if (gameManager.difficulty != 0)
                {
                    gameManager.GameOver();
                    cameraAudioSource.mute = true;
                }
                else
                {
                    cameraAudioSource.Pause();
                    if (gameManager.lives == 0)
                    {
                        cameraAudioSource.mute = true;
                    }
                }
            }
            if (this.hasPowerup) { gameManager.activatePowerup = true; }
            StartCoroutine(DestroyObject());
        }
    }
    IEnumerator DestroyObject()
    {
        yield return new WaitForFixedUpdate();
        Destroy(gameObject);
    }
}

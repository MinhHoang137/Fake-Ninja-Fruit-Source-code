using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    public AudioClip[] slashSound;
    private AudioSource audioSource;
    private AudioSource cameraAudioSource;
    public GameManager gameManager;
    private TrailRenderer trailRenderer;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        cameraAudioSource = Camera.main.GetComponent<AudioSource>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) 
        {
            trailRenderer.enabled = true;
        }
        else
        {
            trailRenderer.enabled = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Input.GetMouseButton(0) && gameManager.isGameActive)
        {
            if (other.gameObject.CompareTag("Bad"))
            {
                audioSource.PlayOneShot(slashSound[1], 1);
                gameManager.AddLives(-1);
                if (gameManager.lives > 0) { StartCoroutine(TurnOnBgMusic()); }   
            }
            else if (gameManager.isGameActive)
            {
                audioSource.PlayOneShot(slashSound[0], 1);
            }
        } 
    }
    IEnumerator TurnOnBgMusic()
    {
        yield return new WaitForSeconds(3f); //3s: length of "Explosion"
        cameraAudioSource.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadingText : MonoBehaviour
{
    private float fadeTime = 1.5f;
    private float startFadeTime = 1f;
    private TextMeshProUGUI fadeAwayText;
    private float initialAlpha;
    public float currentFadeTime;
    public bool isActive;
    // Start is called before the first frame update
    void Start()
    {
        fadeAwayText = GetComponent<TextMeshProUGUI>();
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive == true)
        {
            currentFadeTime = fadeTime;
            initialAlpha = 1;
            isActive = false;
        }
        if (currentFadeTime < startFadeTime)
        {
            fadeAwayText.color = new Color(fadeAwayText.color.r, fadeAwayText.color.g,
                fadeAwayText.color.b, initialAlpha * currentFadeTime / startFadeTime);
            if (currentFadeTime == 0)
            {
                gameObject.SetActive(false);
            }
        }
        if (currentFadeTime > 0)
        {
            currentFadeTime -= Time.deltaTime;
        }
        if (currentFadeTime < 0)
        {
            currentFadeTime = 0;
        }
    }
}

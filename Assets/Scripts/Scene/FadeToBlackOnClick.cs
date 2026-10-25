using UnityEngine;
using UnityEngine.UI;

public class FadeToBlackOnClick : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image blackScreen;
    public float fadeInTime = 2f;

    private bool fading = false;
    private float timer = 0f;

    void Start()
    {
        // Start transparent
        SetAlpha(0f);
    }

    void Update()
    {
        if (fading)
        {
            timer += Time.deltaTime;

            float alpha = timer / fadeInTime;

            SetAlpha(alpha);

            if (timer >= fadeInTime)
            {
                SetAlpha(1f);
                fading = false;
            }
        }
    }

    // Call this from the button
    public void FadeIn()
    {
        timer = 0f;
        fading = true;
    }

    void SetAlpha(float alpha)
    {
        Color color = blackScreen.color;
        color.a = Mathf.Clamp01(alpha);
        blackScreen.color = color;
    }
}
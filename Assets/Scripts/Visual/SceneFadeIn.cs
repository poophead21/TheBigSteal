using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SceneFadeIn : MonoBehaviour
{
    [SerializeField] private float startDelay = 0.5f;   // Pause before fading starts
    [SerializeField] private float fadeDuration = 1.0f; // How long the fade takes

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        // Ensure the screen starts completely black and blocks raycasts
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private void Start()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        // Wait for the specified delay while remaining black
        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false; // Enable interaction with UI behind
    }
}
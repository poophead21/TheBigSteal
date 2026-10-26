using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;
    
    private GameObject fadeCanvas;
    [SerializeField] private Image fadeImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool automaticallyFadeInOnSceneLoad;
    private Coroutine fadeCoroutine;
    
    [SerializeField] private float fadeStartAlpha;
    [SerializeField] private float fadeEndAlpha;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float fadeDelayBeforeFade;

    /*private void Awake()
    { 
        if (Instance == null) 
        {
            Instance = this;
            
        } 
        else 
        {
            Destroy(gameObject);
            
        } 
        DontDestroyOnLoad(gameObject);
    }*/

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        if (automaticallyFadeInOnSceneLoad)
        {
            Debug.Log("Start fade");
            DoFade(fadeStartAlpha, fadeEndAlpha, fadeDuration, fadeDelayBeforeFade);
        }
    }
    
    public void DoFade(float startAlpha, float endAlpha, float duration, float delayBeforeFade)
    {
        Debug.Log("DoFade");
        if (canvasGroup == null)
        {
            return;
        }

        Debug.Log("AnimateFade");
        StartCoroutine(AnimateFade(startAlpha, endAlpha, duration, delayBeforeFade));
    }

    private IEnumerator AnimateFade(float startAlpha, float endAlpha, float duration, float delayBeforeFade)
    {
        fadeImage.enabled = true;
        canvasGroup.alpha = startAlpha;
        yield return null; //frame plays before continuing
        yield return new WaitForSeconds(delayBeforeFade);
        float timeElapsed = 0;
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float fadePercentage = timeElapsed / duration;
            fadePercentage = Mathf.Clamp01(fadePercentage);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, fadePercentage);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
        if (endAlpha <= 0)
        {
            fadeImage.enabled = false;
        }
    }
}

using System.Collections;
using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    [Header("Position References")]
    public Vector2 offScreenStart;
    public Vector2 centerScreen;
    public Vector2 offScreenEnd;

    [Header("Timing")]
    [Tooltip("Delay in seconds before the transition animation begins after scene start.")]
    public float startDelay = 1f;

    [Tooltip("Total time for the full movement animation.")]
    public float totalDuration = 3f;

    [Header("Easing Curves")]
    [Tooltip("Define the speed behavior over time (0 to 1).")]
    public float movementCurve; // Note: Ensure this is set to AnimationCurve in your inspector if needed

    [SerializeField] private AnimationCurve movementCurveAsset;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        // Set initial starting position immediately on scene start so it doesn't flash on screen during the delay
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = offScreenStart;
        }

        StartCoroutine(TransitionRoutine());
    }

    IEnumerator TransitionRoutine()
    {
        // Wait for the specified start delay before beginning movement
        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        float elapsedTime = 0f;

        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime;

            // Normalized time progresses smoothly from 0.0 to 1.0
            float t = elapsedTime / totalDuration;

            // Evaluate value from custom animation curve
            float curveValue = movementCurveAsset != null ? movementCurveAsset.Evaluate(t) : t;

            // Interpolate position based on curve value
            if (curveValue < 0.5f)
            {
                // First half: Move from off-screen to center
                float entryProgress = curveValue / 0.5f;
                rectTransform.anchoredPosition = Vector2.Lerp(offScreenStart, centerScreen, entryProgress);
            }
            else
            {
                // Second half: Move from center to off-screen end
                float exitProgress = (curveValue - 0.5f) / 0.5f;
                rectTransform.anchoredPosition = Vector2.Lerp(centerScreen, offScreenEnd, exitProgress);
            }

            yield return null;
        }

        // Ensure exact final position
        rectTransform.anchoredPosition = offScreenEnd;
    }
}
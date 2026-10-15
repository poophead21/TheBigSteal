using System.Collections;
using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    [Header("Position References")]
    public Vector2 offScreenStart;
    public Vector2 centerScreen;
    public Vector2 offScreenEnd;

    [Header("Timing")]
    public float totalDuration = 3f; // Total time for the full animation

    [Header("Easing Curves")]
    [Tooltip("Define the speed behavior over time (0 to 1).")]
    public AnimationCurve movementCurve;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        // Start the transition automatically for testing
        StartCoroutine(TransitionRoutine());
    }

    IEnumerator TransitionRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime;

            // Normalized time progresses smoothly from 0.0 to 1.0
            float t = elapsedTime / totalDuration;

            // Evaluate the value from your custom curve
            float curveValue = movementCurve.Evaluate(t);

            // Interpolate position based on the curve value
            if (curveValue < 0.5f)
            {
                // First half: Move from off-screen to center
                // Map curveValue (0.0 to 0.5) to a standard Lerp range (0.0 to 1.0)
                float entryProgress = curveValue / 0.5f;
                rectTransform.anchoredPosition = Vector2.Lerp(offScreenStart, centerScreen, entryProgress);
            }
            else
            {
                // Second half: Move from center to off-screen end
                // Map curveValue (0.5 to 1.0) to a standard Lerp range (0.0 to 1.0)
                float exitProgress = (curveValue - 0.5f) / 0.5f;
                rectTransform.anchoredPosition = Vector2.Lerp(centerScreen, offScreenEnd, exitProgress);
            }

            yield return null;
        }

        // Ensure it reaches the exact final destination
        rectTransform.anchoredPosition = offScreenEnd;
    }
}

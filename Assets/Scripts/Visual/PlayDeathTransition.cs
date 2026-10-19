using System.Collections;
using TMPro; // Required for TextMeshPro
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayDeathTransition : MonoBehaviour
{
    public static PlayDeathTransition Instance { get; private set; }

    [Header("Animation References")]
    [SerializeField] private Animation deathAnimation;
    [SerializeField] private string irisOpenClipName = "IrisOpen";
    [SerializeField] private string irisCloseClipName = "IrisClose";

    [Header("Text Overlay Settings")]
    [SerializeField] private CanvasGroup textCanvasGroup;
    [SerializeField] private TextMeshProUGUI transitionText;
    [SerializeField] private string deathMessage = "CAUGHT!";

    [Header("Transition Settings")]
    [SerializeField] private float transitionDelay = 1.0f; // Duration of closing iris/fade
    [SerializeField] private bool playIrisOpenOnSceneStart = true;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (deathAnimation == null)
        {
            deathAnimation = GetComponent<Animation>();
        }

        // Hide text on startup
        if (textCanvasGroup != null)
        {
            textCanvasGroup.alpha = 0f;
        }
    }

    private void Start()
    {
        if (playIrisOpenOnSceneStart)
        {
            PlayIrisOpen();
        }
    }

    public void PlayIrisOpen()
    {
        if (deathAnimation != null && !string.IsNullOrEmpty(irisOpenClipName))
        {
            deathAnimation.Play(irisOpenClipName);
        }
    }

    public void TriggerDeathSequence()
    {
        if (isTransitioning) return;
        StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        isTransitioning = true;

        // 1. Freeze player controls & velocity
        DisablePlayerControls();

        // 2. Set text message & fade it in
        if (transitionText != null)
        {
            transitionText.text = deathMessage;
        }

        // 3. Play Iris Close animation
        if (deathAnimation != null && !string.IsNullOrEmpty(irisCloseClipName))
        {
            deathAnimation.Play(irisCloseClipName);
        }

        // 4. Trigger screen fade and text fade-in
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.DoFade(0f, 1f, transitionDelay, 0f); 
        }

        float timer = 0f;
        while (timer < transitionDelay)
        {
            timer += Time.deltaTime;
            if (textCanvasGroup != null)
            {
                textCanvasGroup.alpha = Mathf.Clamp01(timer / (transitionDelay * 0.5f));
            }
            yield return null;
        }

        // 5. Reload active scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void DisablePlayerControls()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerController controllerScript = player.GetComponent<PlayerController>();
        if (controllerScript != null)
        {
            controllerScript.enabled = false;
        }

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}
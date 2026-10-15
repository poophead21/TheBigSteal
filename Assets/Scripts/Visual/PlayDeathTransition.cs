using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayDeathTransition : MonoBehaviour
{
    public static PlayDeathTransition Instance { get; private set; }

    [SerializeField] private Animation deathAnimation;
    [SerializeField] private float transitionDelay = 1.0f; // Time for iris animation to finish before scene reset

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
    }

    public void TriggerDeathSequence()
    {
        if (isTransitioning) return;
        StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        isTransitioning = true;

        // 0. Disable Player Controls & Reset Movement Velocity
        DisablePlayerControls();

        // 1. Play Iris Shot animation
        if (deathAnimation != null)
        {
            deathAnimation.Play();
        }

        // 2. Trigger screen fade out
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.DoFade(0f, 1f, transitionDelay, 0f); //[cite: 1]
        }

        // 3. Wait for animation and fade to finish
        yield return new WaitForSeconds(transitionDelay);

        // 4. Reload current active scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void DisablePlayerControls()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Disable PlayerMovement script if attached
        PlayerMovement movementScript = player.GetComponent<PlayerMovement>();
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        // Freeze CharacterController velocity
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        // Freeze Rigidbody velocity if using physics movement
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}
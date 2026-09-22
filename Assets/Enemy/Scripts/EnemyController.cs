using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyPatrol))]
[RequireComponent(typeof(FieldOfView))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float patrolSpeed = 3.5f;

    [Header("Investigation Settings")]
    [Tooltip("How long the enemy WAITS looking around at its spot before moving to the last known position.")]
    [SerializeField] private float hesitationDelay = 1.5f;

    [Tooltip("EXACT TOTAL TIME (in seconds) the enemy will spend searching/investigating after losing sight before returning to patrol.")]
    [SerializeField] private float investigationDuration = 6.0f;

    [Header("Rotation Settings")]
    [SerializeField] private float turnSpeed = 8f;

    [Header("UI Visual Indicators")]
    [Tooltip("Question mark icon placed above enemy's head.")]
    [SerializeField] private GameObject questionMarkIcon;

    [Tooltip("Exclamation mark icon placed above enemy's head when chase triggers.")]
    [SerializeField] private GameObject exclamationMarkIcon;

    [Tooltip("How long (in seconds) the question mark stays visible after spotting the player, even if line of sight is broken.")]
    [SerializeField] private float questionMarkDuration = 1.5f;

    [Tooltip("How long (in seconds) the exclamation mark stays visible when chase begins.")]
    [SerializeField] private float exclamationMarkDuration = 2.0f;

    [Header("Animation Reference")]
    [SerializeField] private Animator anim;

    [Header("Audio Sources & Clips")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [Space(5)]
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private AudioClip alertSoundClip;
    [SerializeField] private AudioClip chaseSoundClip;

    private EnemyPatrol patrolScript;
    private FieldOfView fovScript;
    private NavMeshAgent agent;

    private Vector3 lastKnownPosition;
    private bool hasLastKnownPos = false;
    private float investigationTimer = 0f;
    private float questionMarkTimer = 0f;
    private float exclamationMarkTimer = 0f;

    private bool playedAlertSound = false;
    private bool playedChaseSound = false;
    private bool isCurrentlyChasing = false;

    private void Awake()
    {
        patrolScript = GetComponent<EnemyPatrol>();
        fovScript = GetComponent<FieldOfView>();
        agent = GetComponent<NavMeshAgent>();

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (footstepAudioSource == null && audioSources.Length > 0)
            footstepAudioSource = audioSources[0];

        if (sfxAudioSource == null)
        {
            if (audioSources.Length > 1)
                sfxAudioSource = audioSources[1];
            else
                sfxAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (footstepClip != null && footstepAudioSource != null)
        {
            footstepAudioSource.clip = footstepClip;
            footstepAudioSource.loop = true;
        }

        if (sfxAudioSource != null)
        {
            sfxAudioSource.spatialBlend = 1.0f; // Full 3D spatial sound
            sfxAudioSource.playOnAwake = false;
        }

        SetIconActive(questionMarkIcon, false);
        SetIconActive(exclamationMarkIcon, false);
    }

    private void Update()
    {
        // 1. DIRECT SIGHT: Player spotted
        if (fovScript.canSeePlayer && fovScript.playerRef != null)
        {
            patrolScript.enabled = false;
            investigationTimer = 0f;

            // Trigger Alert Sound ONCE when line of sight is gained
            if (!playedAlertSound)
            {
                PlaySFX(alertSoundClip);
                playedAlertSound = true;

                // Start Question Mark timer (will run out independently of sight)
                questionMarkTimer = questionMarkDuration;
            }

            lastKnownPosition = fovScript.playerRef.transform.position;
            hasLastKnownPos = true;

            LookAtTarget(lastKnownPosition);

            if (fovScript.isPlayerDetected)
            {
                // Chase officially initiated -> Report to MusicManager
                if (!isCurrentlyChasing)
                {
                    isCurrentlyChasing = true;
                    if (MusicManager.Instance != null)
                        MusicManager.Instance.ReportChaseState(true);
                }

                // Full chase started -> Cancel Question Mark & Trigger Chase Sound
                questionMarkTimer = 0f;

                if (!playedChaseSound)
                {
                    PlaySFX(chaseSoundClip);
                    playedChaseSound = true;
                    exclamationMarkTimer = exclamationMarkDuration;
                }

                agent.isStopped = false;
                agent.speed = chaseSpeed;
                agent.SetDestination(lastKnownPosition);
            }
            else
            {
                agent.isStopped = true;
            }
        }
        // 2. LOST SIGHT: Wait hesitationDelay, then go to last known position
        else if (hasLastKnownPos)
        {
            patrolScript.enabled = false;
            investigationTimer += Time.deltaTime;

            // Stop reporting chase to MusicManager if sight was lost
            StopChaseState();

            playedAlertSound = false;
            playedChaseSound = false;

            if (investigationTimer >= investigationDuration)
            {
                AbandonSearchAndPatrol();
                return;
            }

            LookAtTarget(lastKnownPosition);

            if (investigationTimer < hesitationDelay)
            {
                agent.isStopped = true;
            }
            else
            {
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    agent.isStopped = true;
                }
                else
                {
                    agent.isStopped = false;
                    agent.speed = chaseSpeed;
                    agent.SetDestination(lastKnownPosition);
                }
            }
        }
        // 3. PATROL STATE
        else
        {
            StopChaseState();

            playedAlertSound = false;
            playedChaseSound = false;

            if (!patrolScript.enabled)
            {
                agent.speed = patrolSpeed;
                patrolScript.enabled = true;
            }
        }

        // Update UI icon countdowns
        UpdateIconTimers();

        // Drive Animation Bools & Footstep Audio
        UpdateAnimationsAndAudio();
    }

    private void UpdateIconTimers()
    {
        // Handle Exclamation Mark timer
        if (exclamationMarkTimer > 0f)
        {
            exclamationMarkTimer -= Time.deltaTime;
            SetIconActive(exclamationMarkIcon, true);
            SetIconActive(questionMarkIcon, false); // Exclamation mark takes priority
        }
        else
        {
            SetIconActive(exclamationMarkIcon, false);

            // Handle Question Mark timer (continues counting down independently of line of sight)
            if (questionMarkTimer > 0f)
            {
                questionMarkTimer -= Time.deltaTime;
                SetIconActive(questionMarkIcon, true);
            }
            else
            {
                SetIconActive(questionMarkIcon, false);
            }
        }
    }

    private void SetIconActive(GameObject icon, bool active)
    {
        if (icon != null && icon.activeSelf != active)
        {
            icon.SetActive(active);
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (sfxAudioSource != null && clip != null)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }

    private void UpdateAnimationsAndAudio()
    {
        if (agent == null) return;

        // Multi-fallback movement check to prevent velocity drops on low acceleration
        bool hasPathToFollow = agent.hasPath && !agent.isStopped && agent.remainingDistance > agent.stoppingDistance;
        bool isMovingByVelocity = agent.velocity.sqrMagnitude > 0.01f || agent.desiredVelocity.sqrMagnitude > 0.01f;
        bool isMoving = hasPathToFollow && isMovingByVelocity;

        bool isInspecting = hasLastKnownPos && !isMoving;

        if (anim != null)
        {
            anim.SetBool("isWalking", isMoving);
            anim.SetBool("isInspecting", isInspecting);
        }

        HandleFootstepAudio(isMoving);
    }

    private void HandleFootstepAudio(bool isMoving)
    {
        if (footstepAudioSource == null || footstepAudioSource.clip == null) return;

        if (isMoving)
        {
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Play();
            }

            // Pitch up slightly when chasing vs patrolling
            footstepAudioSource.pitch = (agent.speed == chaseSpeed) ? 1.25f : 1.0f;
        }
        else
        {
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
        }
    }

    private void LookAtTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position);
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    private void StopChaseState()
    {
        if (isCurrentlyChasing)
        {
            isCurrentlyChasing = false;
            if (MusicManager.Instance != null)
                MusicManager.Instance.ReportChaseState(false);
        }
    }

    private void AbandonSearchAndPatrol()
    {
        StopChaseState();

        hasLastKnownPos = false;
        investigationTimer = 0f;
        questionMarkTimer = 0f;
        exclamationMarkTimer = 0f;
        playedAlertSound = false;
        playedChaseSound = false;
        SetIconActive(questionMarkIcon, false);
        SetIconActive(exclamationMarkIcon, false);
        agent.speed = patrolSpeed;
        patrolScript.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (hasLastKnownPos)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(lastKnownPosition, 0.5f);
            Gizmos.DrawLine(transform.position, lastKnownPosition);
        }
    }
}
using System.Collections;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Maximum vision distance.")]
    public float radius = 10f;

    [Range(0, 360)]
    [Tooltip("Field of view cone angle in degrees.")]
    public float angle = 90f;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    [Header("Eye / Raycast Position")]
    [Tooltip("Height offset from ground level where the enemy's eyes are located.")]
    public float eyeHeight = 1.5f;

    [Header("Player Target Heights")]
    [Tooltip("Target height to aim raycast when player is standing.")]
    public float standingTargetHeight = 1.0f;

    [Tooltip("Target height to aim raycast when player is crouching behind cover.")]
    public float crouchingTargetHeight = 0.4f;

    [Header("Proximity Settings")]
    [Tooltip("Radius around enemy that instantly detects the player even if behind them.")]
    public float proximityAlertRadius = 3.0f;

    [Tooltip("Proximity radius around enemy where touching the player instantly kills them.")]
    public float killRadius = 1.5f;

    [Header("Alert Settings")]
    [Tooltip("Time in seconds the enemy must maintain line of sight before triggering a full chase.")]
    public float timeToDetect = 2.0f;

    [Header("Status (Read Only)")]
    public GameObject playerRef;
    public bool canSeePlayer;
    public bool isPlayerDetected;
    public float detectionTimer = 0f;

    /// <summary>
    /// Calculates eye origin position using basic offset math.
    /// </summary>
    public Vector3 GetEyePosition()
    {
        Vector3 pos = transform.position;
        return new Vector3(pos.x, pos.y + eyeHeight, pos.z);
    }

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    private void Update()
    {
        CheckProximityKill();
        UpdateDetectionTimer();
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Vector3 eyeOrigin = GetEyePosition();

        // 1. PROXIMITY ALERT CHECK (Detects player inside proximity radius regardless of angle)
        Collider[] proximityChecks = Physics.OverlapSphere(transform.position, proximityAlertRadius, targetMask);
        if (proximityChecks.Length > 0)
        {
            canSeePlayer = true;
            return;
        }

        // 2. REGULAR VISION CONE & RAYCAST CHECK
        Collider[] rangeChecks = Physics.OverlapSphere(eyeOrigin, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            GameObject targetObj = rangeChecks[0].gameObject;
            Vector3 targetPos = targetObj.transform.position;

            // Check if player is crouching
            bool isPlayerCrouching = false;
            PlayerMovement playerScript = targetObj.GetComponent<PlayerMovement>();
            if (playerScript != null)
            {
                isPlayerCrouching = playerScript.isCrouching;
            }

            // Adjust raycast target height
            float targetOffset = isPlayerCrouching ? crouchingTargetHeight : standingTargetHeight;
            Vector3 targetCheckPos = new Vector3(targetPos.x, targetPos.y + targetOffset, targetPos.z);

            Vector3 directionToTarget = (targetCheckPos - eyeOrigin).normalized;

            // Angle check inside FOV cone
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(eyeOrigin, targetCheckPos);

                // Editor Debug ray (Green = clear vision, Red = blocked by cover/wall)
#if UNITY_EDITOR
                Debug.DrawRay(eyeOrigin, directionToTarget * distanceToTarget,
                    !Physics.Raycast(eyeOrigin, directionToTarget, distanceToTarget, obstructionMask) ? Color.green : Color.red, 0.1f);
#endif

                // Obstruction raycast check
                if (!Physics.Raycast(eyeOrigin, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false; // Cover blocked raycast to player's current height
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else
        {
            canSeePlayer = false;
        }
    }

    private void UpdateDetectionTimer()
    {
        if (canSeePlayer)
        {
            detectionTimer += Time.deltaTime;

            if (detectionTimer >= timeToDetect)
            {
                isPlayerDetected = true;
            }
        }
        else
        {
            detectionTimer -= Time.deltaTime;
            if (detectionTimer <= 0f)
            {
                detectionTimer = 0f;
                isPlayerDetected = false;
            }
        }
    }

    private void CheckProximityKill()
    {
        if (playerRef == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerRef.transform.position);
        if (distanceToPlayer <= killRadius)
        {
            KillPlayer();
        }
    }

    private void KillPlayer()
    {
        Debug.Log("Player killed by proximity!");
        Destroy(playerRef);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 eyePos = GetEyePosition();

        // Eye Height Origin Point (Cyan)
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(eyePos, 0.15f);

        // Proximity Kill Radius (Red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRadius);

        // Proximity Alert Radius (Cyan Wireframe)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, proximityAlertRadius);

        // Vision Radius from Eye Level (Yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyePos, radius);
    }
}
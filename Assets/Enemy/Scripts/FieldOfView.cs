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

    [Header("Kill Radius Settings")]
    [Tooltip("Proximity radius around the enemy where touching the player instantly kills them.")]
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
    /// Calculates origin position at eye height using basic position math.
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

        Collider[] rangeChecks = Physics.OverlapSphere(eyeOrigin, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            GameObject targetObj = rangeChecks[0].gameObject;
            Vector3 targetPos = targetObj.transform.position;

            // 1. Check if the player is currently crouching
            bool isPlayerCrouching = false;
            PlayerMovement playerScript = targetObj.GetComponent<PlayerMovement>();
            if (playerScript != null)
            {
                isPlayerCrouching = playerScript.isCrouching;
            }

            // 2. Adjust target height based on crouch status
            float targetOffset = isPlayerCrouching ? crouchingTargetHeight : standingTargetHeight;
            Vector3 targetCheckPos = new Vector3(targetPos.x, targetPos.y + targetOffset, targetPos.z);

            Vector3 directionToTarget = (targetCheckPos - eyeOrigin).normalized;

            // 3. Angle check within FOV cone
            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(eyeOrigin, targetCheckPos);

                // Editor Debug line (Green = clear line of sight, Red = obstructed by wall/cover)
#if UNITY_EDITOR
                Debug.DrawRay(eyeOrigin, directionToTarget * distanceToTarget,
                    !Physics.Raycast(eyeOrigin, directionToTarget, distanceToTarget, obstructionMask) ? Color.green : Color.red, 0.1f);
#endif

                // 4. Raycast check against obstacles
                if (!Physics.Raycast(eyeOrigin, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false; // Cover blocked line of sight to the player's current height
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

        // Vision Radius from Eye Level (Yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyePos, radius);
    }
}
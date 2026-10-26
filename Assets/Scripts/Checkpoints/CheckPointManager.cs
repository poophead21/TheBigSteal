using System;
using System.Collections;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [SerializeField] private GameObject[] checkPointTriggers;
    private TriggerCheckpoint[] triggerCheckpoints;
    [SerializeField] private Transform player;
    private Vector3 playerStartPosition;
    private CharacterController playerController;

    public static CheckPointManager Instance;
    [SerializeField] private float waitBeforeTeleport = 1f;

    public delegate void OnCheckPointEvent();
    public event OnCheckPointEvent OnCheckPointActivated;

    private Coroutine activeTeleportCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerStartPosition = player.position;
        playerController = player.GetComponent<CharacterController>();
        triggerCheckpoints = new TriggerCheckpoint[checkPointTriggers.Length];

        for (int i = 0; i < checkPointTriggers.Length; i++)
        {
            if (checkPointTriggers[i] != null)
            {
                triggerCheckpoints[i] = checkPointTriggers[i].GetComponent<TriggerCheckpoint>();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadCheckpoint();
        }
    }

    public void LoadCheckpoint()
    {
        // Prevent starting multiple teleport routines simultaneously
        if (activeTeleportCoroutine != null)
        {
            StopCoroutine(activeTeleportCoroutine);
        }

        activeTeleportCoroutine = StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        // Wait for the specified delay before moving the player
        yield return new WaitForSeconds(waitBeforeTeleport);

        bool checkpointFound = false;

        // Search for the latest active checkpoint
        for (int i = triggerCheckpoints.Length - 1; i >= 0; i--)
        {
            if (triggerCheckpoints[i] != null && triggerCheckpoints[i].isTriggered)
            {
                TeleportToPosition(new Vector3(
                    triggerCheckpoints[i].transform.position.x,
                    triggerCheckpoints[i].transform.position.y + 1f,
                    triggerCheckpoints[i].transform.position.z
                ));

                checkpointFound = true;
                OnCheckPointActivated?.Invoke();
                break;
            }
        }

        // If no checkpoints were active, return to start position
        if (!checkpointFound)
        {
            TeleportToPosition(playerStartPosition);
        }

        activeTeleportCoroutine = null;
    }

    private void TeleportToPosition(Vector3 targetPosition)
    {
        playerController.enabled = false;
        player.position = targetPosition;
        playerController.enabled = true;
    }
}
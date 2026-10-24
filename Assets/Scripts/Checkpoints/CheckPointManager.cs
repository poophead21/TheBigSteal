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
    //function for do checkpoint
    //check at what checkpoint you are
    public static CheckPointManager Instance;
    [SerializeField] private float waitBeforeTeleport = 1f;
    
    public delegate void OnCheckPointEvent();
    public event OnCheckPointEvent OnCheckPointActivated;
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
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        playerStartPosition = player.position;
        playerController  = player.GetComponent<CharacterController>();
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
        //Debug.Log("Loading checkpoint");
        StartCoroutine(TeleportPlayer());

        bool disabled = false;
        for (int i = triggerCheckpoints.Length - 1; i >= 0; i--)
        {
            
            if (triggerCheckpoints[i] != null && triggerCheckpoints[i].isTriggered)
            {
                disabled = true;
            }
        }

        if (!disabled)
        {
            StartCoroutine(TeleportPlayerToStart());
        }
    }

    IEnumerator TeleportPlayerToStart()
    {
        playerController.enabled = false;
        yield return new WaitForSeconds(waitBeforeTeleport);
        player.position = playerStartPosition;
        playerController.enabled = true;
    }
    IEnumerator TeleportPlayer()
    {
        for (int i = triggerCheckpoints.Length - 1; i >= 0; i--)
        {
            if (triggerCheckpoints[i] != null && triggerCheckpoints[i].isTriggered)
            {
                playerController.enabled = false;
                yield return new WaitForSeconds(waitBeforeTeleport);
                player.position = new Vector3(triggerCheckpoints[i].transform.position.x, 1, player.position.z);
                playerController.enabled = true;
                
                OnCheckPointActivated?.Invoke();
                break;
            }
        }
        
    }
}

using System;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    [SerializeField] private GameObject[] checkPointTriggers;
    private TriggerCheckpoint[] triggerCheckpoints;
    [SerializeField] private Transform player;
    private CharacterController playerController;
    //function for do checkpoint
    //check at what checkpoint you are
    public static CheckPointManager Instance;
    
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
        for (int i = triggerCheckpoints.Length - 1; i >= 0; i--)
        {
            if (triggerCheckpoints[i] != null && triggerCheckpoints[i].isTriggered == true)
            {
                Debug.Log("OnCheckPointActivated?.Invoke();");
                playerController.enabled = false;
                //player.position = triggerCheckpoints[i].transform.position;
                player.position = new Vector3(triggerCheckpoints[i].transform.position.x, 1, player.position.z);
                playerController.enabled = true;
                
                OnCheckPointActivated?.Invoke();
                break;
            }
        }
    }
}

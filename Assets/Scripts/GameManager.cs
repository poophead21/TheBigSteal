using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public delegate void OnGameEvent();
    public event OnGameEvent OnGameStart;
    public event OnGameEvent OnCPActivated;
    
    public delegate void OnCheckPointEvent(int index);
    public event OnCheckPointEvent OnCPIndexIncrease;
    public event OnCheckPointEvent OnCPIndexReset;

    private int checkPointIndex;
    

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
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CPIndexIncrease();
        }
    }

    public void StartGame() //happens when start button is pressed in main menu
    {
        OnGameStart?.Invoke();
        CPIndexReset();
    }

    public void CPIndexReset() //method for reseting score when entering new scene
    {
        checkPointIndex = 0;
        OnCPIndexReset?.Invoke(checkPointIndex);
    }

    public void CPIndexIncrease() //method for increasing score per player interaction
    {
        checkPointIndex++;
        Debug.Log("CPIndexIncrease");
        OnCPIndexIncrease?.Invoke(checkPointIndex);

        if (checkPointIndex > 2)
        {
            OnCPActivated?.Invoke();
        }
    }
}

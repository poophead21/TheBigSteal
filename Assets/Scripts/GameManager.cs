using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public delegate void OnGameEvent();
    public event OnGameEvent OnCPActivated;

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
        CPIndexReset();
    }

    public void CPIndexReset() //method for reseting score when entering new scene
    {
        checkPointIndex = 0;
    }

    public void CPIndexIncrease() //method for increasing score per player interaction
    {
        checkPointIndex++;

        if (checkPointIndex >= 3)
        {
            OnCPActivated?.Invoke();
            checkPointIndex = 0;
        }
    }
}

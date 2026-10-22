using UnityEngine;

public class ItemCheckpoint : MonoBehaviour
{
    private Vector3 startLocation;
    private Quaternion startRotation;
    [SerializeField] private Vector3 correctRotation;
    [SerializeField] private Vector3 correctLocation;

    [SerializeField] private float positionLimit = 0.1f;

    //save starting location on scene start
    //save correct location on scene start
    // when reset, if not correct, go to start location, if correct stick to correct location

    private void OnEnable()
    {
        CheckPointManager.Instance.OnCheckPointActivated += ResetItems;
        CheckPointManager.Instance.OnCheckPointActivated += ResetEnemies;
    }

    private void OnDisable()
    {
        CheckPointManager.Instance.OnCheckPointActivated -= ResetItems;
        CheckPointManager.Instance.OnCheckPointActivated -= ResetEnemies;
    }
    private void Start()
    {
        startLocation = transform.position;
        startRotation = transform.rotation;
    }

    private void ResetItems()
    {
        float distance = Vector3.Distance(startLocation, transform.position);
        if (distance <= positionLimit)
        {
            transform.position = correctLocation;
            transform.rotation = Quaternion.Euler(correctRotation);
        }
        else
        {
            transform.position = startLocation;
            transform.rotation = startRotation;
        }
        
    }
    private void ResetEnemies()
    {
        transform.position = startLocation;
        transform.rotation = startRotation;
    }
}

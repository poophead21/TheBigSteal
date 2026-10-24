using System.Collections;
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
        Debug.Log("Reset Items");
        float distance = Vector3.Distance(startLocation, transform.position);
        if (distance <= positionLimit)
        {
            StartCoroutine(SetCorrectLocation());
            
        }
        else
        {
            StartCoroutine(SetStartLocation());
        }
        
    }
    private void ResetEnemies()
    {
        StartCoroutine(SetStartLocation());
    }

    IEnumerator SetCorrectLocation()
    {
        transform.position = correctLocation;
        transform.rotation = Quaternion.Euler(correctRotation);
        yield return new WaitForSeconds(1f);
    }
    IEnumerator SetStartLocation()
    {
        transform.position = startLocation;
        transform.rotation = startRotation;
        yield return new WaitForSeconds(1f);
    }
}

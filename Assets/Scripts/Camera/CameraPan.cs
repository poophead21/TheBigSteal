using UnityEngine;

public class CameraPan : MonoBehaviour
{
    [Header("Movement Settings")]
    public float distance = 5.0f;
    public float speed = 2.0f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // PingPong moves between 0 and distance linearly
        float xOffset = Mathf.PingPong(Time.time * speed, distance) - (distance / 2f);

        transform.position = new Vector3(startPosition.x + xOffset, startPosition.y, startPosition.z);
    }
}
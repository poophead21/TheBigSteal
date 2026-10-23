using UnityEngine;

public class MoveRight : MonoBehaviour
{
    [Tooltip("Speed of movement to the right (units per second)")]
    public float speed = 3.0f;

    void Update()
    {
        // Moves the object to the right along the X-axis smoothly over time
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
    }
}
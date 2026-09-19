using UnityEngine;

public class IsometricCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}

using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    [Tooltip("Optional: Drag your Camera here manually if Camera.main is returning null.")]
    [SerializeField] private Camera targetCamera;

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    // LateUpdate runs AFTER the Enemy's rotation updates in Update
    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                Debug.LogWarning("BillboardUI: No Camera found! Ensure your Camera has the 'MainCamera' tag or assign it manually in the Inspector.");
                return;
            }
        }

        // Force the Canvas rotation to match the camera orientation exactly, completely ignoring enemy rotation
        transform.rotation = targetCamera.transform.rotation;
    }
}
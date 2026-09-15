using UnityEngine;

public class Mirror : MonoBehaviour
{
    private bool isReflecting;
    private Light spotlight;
    private void Start()
    {
        spotlight = GetComponent<Light>();
    }
    private void LightEmitter()
    {
        if(isReflecting) spotlight.enabled = true;
    }

    private void Rotator()
    {
        transform.Rotate(0, 45, 0);
    }

}

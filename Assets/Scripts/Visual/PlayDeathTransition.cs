using UnityEngine;

public class PlayDeathTransition : MonoBehaviour
{
    //test death animation
    private Animation animation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animation = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            animation.Play();
        }
    }
}

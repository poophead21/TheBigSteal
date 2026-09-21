using Unity.VisualScripting;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float moveDistance = 2.0f;
    [SerializeField] private float moveTime = 1.0f;
    [SerializeField] private float interactDistance = 2.0f;
    [SerializeField] private GameObject player;

    private bool isBeingInteractedWith = false;
    private float moveSpeed;
    private Vector3 moveTo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveSpeed = moveDistance / moveTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBeingInteractedWith)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, moveTo, step);

            if (Vector3.Distance(transform.position, moveTo) < 0.001f)
            {
                isBeingInteractedWith = false;
            }

        } else {
            Vector3 playerToObject = transform.position - player.transform.position;


            if (playerToObject.magnitude > interactDistance) return;
            if (Vector3.Angle(player.transform.forward, playerToObject) > 45.0f) return;
            if (!player.GetComponent<PlayerMovement>().isInteracting) return;

            isBeingInteractedWith = true;

            Vector3 direction;
            if (Mathf.Abs(playerToObject.x) > Mathf.Abs(playerToObject.z))
            {
                direction = (playerToObject.x > 0.0f) ? new Vector3(1.0f, 0.0f, 0.0f) : new Vector3(-1.0f, 0.0f, 0.0f);
            } else
            {
                direction = (playerToObject.z > 0.0f) ? new Vector3(0.0f, 0.0f, 1.0f) : new Vector3(0.0f, 0.0f, -1.0f);
            }

            moveTo = transform.position + direction * moveDistance;
        }
    }
}

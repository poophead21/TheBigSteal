using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float interactDistance = 2.0f;
    [SerializeField] private float moveDistance = 2.0f;
    [SerializeField] private float moveTime = 1.0f;
    [SerializeField] private float rotateAngle = 90.0f;
    [SerializeField] private float rotateTime = 1.0f;
    [SerializeField] private GameObject player;

    private bool isBeingInteractedWith = false;
    private enum InteractionType { PUSH, ROTATE };
    private InteractionType interactionType = InteractionType.PUSH;
    private float moveSpeed;
    private float rotateSpeed;
    private Vector3 moveTo;
    private quaternion rotateTo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveSpeed = moveDistance / moveTime;
        rotateSpeed = rotateAngle / rotateTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBeingInteractedWith)
        {
            switch(interactionType)
            {
                case InteractionType.PUSH:
                    float moveStep = moveSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, moveTo, moveStep);

                    if (Vector3.Distance(transform.position, moveTo) < 0.001f)
                    {
                        isBeingInteractedWith = false;
                    }
                    break;
                case InteractionType.ROTATE:
                    float rotateStep = rotateSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateTo, rotateStep);

                    if (Quaternion.Angle(transform.rotation, rotateTo) < 0.01f)
                    {
                        isBeingInteractedWith = false;
                    }
                    break;
            }
            

        } else {
            Vector3 playerToObject = transform.position - player.transform.position;


            bool pushing = player.GetComponent<PlayerMovement>().isPushing;
            bool rotating = player.GetComponent<PlayerMovement>().isRotating;


            if (playerToObject.magnitude > interactDistance) return;
            if (Vector3.Angle(player.transform.forward, playerToObject) > 45.0f) return;

            if (!pushing && !rotating) return;

            isBeingInteractedWith = true;

            if (pushing)
            {
                interactionType = InteractionType.PUSH;
                Vector3 direction;
                if (Mathf.Abs(playerToObject.x) > Mathf.Abs(playerToObject.z))
                {
                    direction = (playerToObject.x > 0.0f) ? new Vector3(1.0f, 0.0f, 0.0f) : new Vector3(-1.0f, 0.0f, 0.0f);
                }
                else
                {
                    direction = (playerToObject.z > 0.0f) ? new Vector3(0.0f, 0.0f, 1.0f) : new Vector3(0.0f, 0.0f, -1.0f);
                }

                moveTo = transform.position + direction * moveDistance;
            } else if (rotating)
            {
                interactionType = InteractionType.ROTATE;

                rotateTo = transform.rotation * quaternion.RotateY(rotateAngle / 180 * Mathf.PI);
            }
        }
    }
}

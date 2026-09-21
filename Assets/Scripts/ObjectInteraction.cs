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

    private void OnTriggerEnter(Collider other)
    {
        if (isBeingInteractedWith && interactionType == InteractionType.PUSH)
        {
            resolveCollision(GetComponent<Collider>().bounds, other.bounds);
            isBeingInteractedWith = false;
        }
    }

    private void resolveCollision(Bounds bounds, Bounds otherBounds)
    {
        float xOverlap = 0.0f, zOverlap = 0.0f;

        if (bounds.min.x < otherBounds.min.x && otherBounds.min.x < bounds.max.x)
        {
            xOverlap = bounds.max.x < otherBounds.max.x ? bounds.max.x - otherBounds.min.x : otherBounds.max.x - otherBounds.min.x;
        } else if (otherBounds.min.x < bounds.min.x && bounds.min.x < otherBounds.max.x)
        {
            xOverlap = otherBounds.max.x < bounds.max.x ? bounds.min.x - otherBounds.max.x : otherBounds.min.x - bounds.max.x ;
        }

        if (bounds.min.z < otherBounds.min.z && otherBounds.min.z < bounds.max.z)
        {
            zOverlap = bounds.max.z < otherBounds.max.z ? bounds.max.z - otherBounds.min.z : otherBounds.max.z - otherBounds.min.z;
        }
        else if (otherBounds.min.z < bounds.min.z && bounds.min.z < otherBounds.max.z)
        {
            zOverlap = otherBounds.max.z < bounds.max.z ? bounds.min.z - otherBounds.max.z : otherBounds.min.z - bounds.max.z;
        }

        if (xOverlap != 0.0f && (zOverlap == 0.0f || Mathf.Abs(xOverlap) < Mathf.Abs(zOverlap)))
        {
            xOverlap += (xOverlap > 0.0f) ? 0.001f : -0.001f;
            transform.position -= new Vector3(xOverlap, 0.0f, 0.0f);
        } else if (zOverlap != 0.0f && (xOverlap == 0.0f || Mathf.Abs(zOverlap) < Mathf.Abs(xOverlap)))
        {
            zOverlap += (zOverlap > 0.0f) ? 0.001f : -0.001f;
            transform.position -= new Vector3(0.0f, 0.0f, zOverlap);
        }
    }
}

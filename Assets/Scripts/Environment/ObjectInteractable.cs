using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


public class ObjectInteractable : Interactable
{
    [Header("Variables")]
    [SerializeField] private float moveDistance = 2.0f;
    [SerializeField] private float moveTime = 1.0f;
    [SerializeField] private float rotateAngle = 90.0f;
    [SerializeField] private float rotateTime = 1.0f;
    [SerializeField] private bool pushable = true;
    [SerializeField] private bool rotatable = true;
    private PlayerController playerController;
    
    private const float EPSILON = 0.0001f;
    private bool isBeingInteractedWith = false;
    private float moveSpeed;
    private float rotateSpeed;
    private Vector3 moveTo;
    private quaternion rotateTo;
    private InteractionType interactionType;

    private BoxCollider _collider;
    
    //UI related
    [SerializeField] private MeshRenderer meshrenderer;
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private Outline outline;
    
    private AudioManager audioManager;

    void Start()
    {
        audioManager = GameObject.Find("SoundManager").GetComponent<AudioManager>();
        
        _collider = GetComponent<BoxCollider>();
        if (_collider == null)
            Debug.LogError("Interactable object " + this + " does not contain required box collider.");
        else if (Mathf.Abs(_collider.size.x - _collider.size.z) > EPSILON)
            Debug.LogWarning("Interactable object " + this + " is not square in the xz plane.");
        

        moveSpeed = moveDistance / moveTime;
        rotateSpeed = rotateAngle / rotateTime;
        
        
        //UI related
        outline  = GetComponent<Outline>();
        SetOutline(false);
    }

    private void SetOutline(bool enabled) //UI related
    {
        outline.enabled = enabled;
    }

    private void PushUpdate()
    {
        float moveStep = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, moveTo, moveStep);

        if (Vector3.Distance(transform.position, moveTo) < 0.001f)
        {
            transform.position = moveTo;
            isBeingInteractedWith = false;
        }
    }

    private void RotateUpdate()
    {
        float rotateStep = rotateSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotateTo, rotateStep);

        if (Quaternion.Angle(transform.rotation, rotateTo) < 0.01f)
        {
            transform.rotation = rotateTo;
            isBeingInteractedWith = false;
        }
    }

    private Vector3 DeterminePushingDirection(Vector3 playerToObject)
    {
        Vector3 direction;
        if (Mathf.Abs(playerToObject.x) > Mathf.Abs(playerToObject.z))
        {
            direction = (playerToObject.x > 0.0f) ? new Vector3(1.0f, 0.0f, 0.0f) : new Vector3(-1.0f, 0.0f, 0.0f);
        }
        else
        {
            direction = (playerToObject.z > 0.0f) ? new Vector3(0.0f, 0.0f, 1.0f) : new Vector3(0.0f, 0.0f, -1.0f);
        }
        return direction;
    }

    private void StartPushing(Vector3 playerToObject)
    {
        audioManager.PlayAudioClip(Sound.WoodScrape);
        interactionType = InteractionType.PRIMARY;
        moveTo = transform.position + DeterminePushingDirection(playerToObject) * moveDistance;
    }

    void Update()
    {
        if (!isBeingInteractedWith) return;
        
        switch (interactionType)
        {
            case InteractionType.PRIMARY:
                PushUpdate();
                break;
            case InteractionType.SECONDARY:
                RotateUpdate();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isBeingInteractedWith && interactionType == InteractionType.PRIMARY)
        {
            isBeingInteractedWith = false;
        }
        //UI component
        SetOutline(true);
        UICanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        //UI component
        SetOutline(false);
        UICanvas.SetActive(false);
    }

    private bool CanPush(Vector3 playerToObject)
    {
        if (!pushable) return false;
        
        Vector3 direction = DeterminePushingDirection(playerToObject);
        // assuming the collider is centered and square on the xz plane
        Vector3 origin = transform.position + (_collider.size.x / 2.0f * transform.localScale.x + EPSILON) * direction; 
        
        bool hit = Physics.Raycast(origin, direction, moveDistance - 2 * EPSILON);
        return !hit;
    }

    public override bool CanInteract(GameObject interactor, InteractionType interactionType)
    {
        switch (interactionType)
        {
            case InteractionType.PRIMARY: return CanPush(transform.position - interactor.transform.position);
            case InteractionType.SECONDARY: return rotatable;
        }

        return false;
    }

    public override void Interact(GameObject interactor, InteractionType playerInteractionType)
    {
        interactionType = playerInteractionType;
        Vector3 playerToObject = transform.position - interactor.transform.position;
        isBeingInteractedWith = true;

        if (interactionType == InteractionType.PRIMARY)
        {
            StartPushing(playerToObject);
        }
        else if (interactionType == InteractionType.SECONDARY)
        {
            audioManager.PlayAudioClip(Sound.WoodScrape);
            rotateTo = transform.rotation * quaternion.RotateY(rotateAngle / 180 * Mathf.PI);
        }
    }
}
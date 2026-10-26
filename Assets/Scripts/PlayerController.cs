using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;
    private Animator _animator;
    private AudioManager _audioManager;

    public bool isMovingForCutscene = true;
    
    [SerializeField] private float interactDistance = 2.0f;
    
    [Header("Variables")]
    [SerializeField] public float speed = 5f;
    private float baseSpeed;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private Transform cameraTransform;
    
    [Header("Velocity")]
    private Vector3 sideVelocity;
    private Vector3 forwardVelocity;
    private Vector3 input;
    private Vector3 velocity;
    private Vector3 movement;
    
    [Header("Vertical")]
    [SerializeField] private float gravity = 9.18f;
    [SerializeField] private float stickToGroundVelocity;
    private float verticalVelocity;

    [Header("Crouching")]
    [SerializeField] private Transform meshTransform;
    private Vector3 initMeshLocation;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private Vector3 crouchCenter = new Vector3(0f, 0.5f, 0f);
    private float standHeight;
    private Vector3 standCenter;

    [Header("Interaction")]
    private bool canAnimateInteract = true;
    [SerializeField] public float interactWaitTime = 2f;
    
    [Header("Booleans")]
    public bool isCrouching = false;
    public bool isRunning = false;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _audioManager = GameObject.Find("SoundManager").GetComponent<AudioManager>();
        
        baseSpeed = speed;
        
        //crouch
        standCenter = _characterController.center;
        standHeight = _characterController.height;

    }
    private void Update()
    {
        if (canAnimateInteract)
        {
            UpdateHorizontalVelocity();
            UpdateVerticalVelocity();
            ApplyTotalVelocity();

            CheckCrouchingRunning();
            UpdateController();
        }
        else if (!canAnimateInteract)
        {
            _animator.SetFloat("Velocity", 0, 0.1f, Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            var interactable = CanInteract(InteractionType.PRIMARY);

            if (interactable)
            {
                StartCoroutine(PlayerInteract(interactable, InteractionType.PRIMARY));
                canAnimateInteract = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            var interactable = CanInteract(InteractionType.SECONDARY);

            if (interactable)
            {
                StartCoroutine(PlayerInteract(interactable, InteractionType.SECONDARY));
                canAnimateInteract = false;
            }
        }
    }


    private void ApplyTotalVelocity()
    {
        if (isMovingForCutscene)
        {
            var totalVelocity = movement + verticalVelocity * Vector3.up;
            _characterController.Move(totalVelocity * Time.deltaTime);
            
        }
    }


    private void UpdateHorizontalVelocity()
    {
        float horizontalInput  = Input.GetAxisRaw("Horizontal");
        float verticalInput  = Input.GetAxisRaw("Vertical");
        
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        Vector3 movementDirection = cameraForward * verticalInput + cameraRight * horizontalInput;

        float movementAnimation = Mathf.Clamp01(movementDirection.magnitude);
        _animator.SetFloat("Velocity", movementAnimation, 0.1f, Time.deltaTime);

        if (movementDirection.sqrMagnitude < 0.01f)
        {
            movementDirection = Vector3.zero;
            movement = movementDirection * speed;
            _audioManager.StopPlayingAudioClip(Sound.Footsteps);
            return;
        }
        
        _audioManager.StartPlayingAudioClip(Sound.Footsteps);
        movementDirection.Normalize();
        
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
        movement = movementDirection * speed;
    }

    private void UpdateVerticalVelocity()
    {
        if (_characterController.isGrounded && _characterController.velocity.y < 0)
        {
            verticalVelocity = stickToGroundVelocity;
        }
        verticalVelocity -= gravity * Time.deltaTime;
    }

    private void CheckCrouchingRunning()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (isRunning)
            {
                isRunning = false;
            }
            
            isCrouching = !isCrouching;
            
            if (isCrouching)
            {
                Crouch();
            }
            else
            {
                GetUp();
            }
            
            _animator.SetBool("IsCrouching", isCrouching);
            _animator.SetBool("IsRunning", isRunning);

        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (isCrouching)
            {
                isCrouching = false;
            }
            
            isRunning = !isRunning;
            
            if (isRunning)
            {
                Run();
            }
            else
            {
                StopRunning();
            }
            
            _animator.SetBool("IsRunning", isRunning);
            _animator.SetBool("IsCrouching", isCrouching);
        }
    }
    private void Crouch()
    {
        speed = baseSpeed / 2f;
    }

    private void GetUp()
    {
        speed = baseSpeed;
    }
    private void Run()
    {
        speed = baseSpeed * 2f;
    }

    private void StopRunning()
    {
        speed = baseSpeed;
    }

    private void UpdateController()
    {
        Vector3 targetCenter = standCenter;
        float targetHeight = standHeight;

        if (isCrouching)
        {
            targetCenter = crouchCenter;
            targetCenter.y = standCenter.y - (standHeight - crouchHeight) * 0.5f;
            targetHeight = crouchHeight;
        }
        
        _characterController.height = Mathf.Lerp(_characterController.height, targetHeight, 5f * Time.deltaTime);
        _characterController.center = Vector3.Lerp(_characterController.center, targetCenter, 5f * Time.deltaTime);
    }
    IEnumerator PlayerInteract(Interactable interactable, InteractionType interactionType)
    {
        _animator.SetTrigger("Interact");
        interactable.Interact(gameObject, interactionType);
        yield return new WaitForSeconds(interactWaitTime);
        canAnimateInteract = true;
    }

    private Interactable CanInteract(InteractionType type)
    {
        if (!canAnimateInteract) return null;
        
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            var interactable = hit.collider.gameObject.GetComponent<Interactable>();
            if (interactable && hit.distance < interactDistance && interactable.CanInteract(gameObject, type))
            {
                return interactable;
            }
        }

        return null;
        
        // var colliders = Physics.OverlapBox(transform.position + transform.forward * interactDistance,
        //     new Vector3(_characterController.radius / 2.0f, _characterController.height / 1.5f, interactDistance));
        //
        // Interactable interactable = null;
        // float minimumDistance = interactDistance;
        // foreach (var collider in colliders)
        // {
        //     Vector3 direction = (collider.transform.position - transform.position).normalized;
        //     
        //     if (Physics.Raycast(transform.position, direction, out RaycastHit hit) &&  hit.distance < minimumDistance)
        //     {
        //         var foundInteractable = hit.collider.gameObject.GetComponent<Interactable>();
        //         if (foundInteractable && foundInteractable.CanInteract(gameObject, type))
        //         {
        //             minimumDistance =  hit.distance;
        //             interactable = foundInteractable;
        //         }
        //     }
        // }
        //
        // return interactable;
    }
}

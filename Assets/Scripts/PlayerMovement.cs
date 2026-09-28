using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController _characterController;
    private Animator _animator;
    private CapsuleCollider _capsuleCollider;
    
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
    
    
    
    [Header("Booleans")]
    public bool isCrouching = false;
    public bool isPushing = false;
    public bool isRotating = false;
    public bool isRunning = false;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        
        baseSpeed = speed;
        
        //crouch
        standCenter = _characterController.center;
        standHeight = _characterController.height;
    }
    private void Update()
    {
        UpdateHorizontalVelocity();
        UpdateVerticalVelocity();
        ApplyTotalVelocity();

        CheckCrouchingRunning();
        UpdateController();

        if (Input.GetKeyDown(KeyCode.E) && !isCrouching)
        {
            _animator.SetTrigger("Interact");
            isPushing = true;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            isPushing = false;
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isCrouching)
        {
            _animator.SetTrigger("Interact");
            isRotating = true;
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            isRotating = false;
        }
    }

    private void ApplyTotalVelocity()
    {
        var totalVelocity = movement + verticalVelocity * Vector3.up;
        _characterController.Move(totalVelocity * Time.deltaTime);
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
            return;
        }
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
}

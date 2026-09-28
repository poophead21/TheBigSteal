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
    }
    private void Update()
    {
        UpdateHorizontalVelocity();
        UpdateVerticalVelocity();
        //ApplyTotalVelocity();

        CheckCrouchingRunning();

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

    /*private void ApplyTotalVelocity()
    {
        characterController.Move(movement * Time.deltaTime);
    }*/


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

        if (movementDirection.sqrMagnitude < 0.01f) return;
        movementDirection.Normalize();
        
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
        movement = movementDirection * speed;
        
        _characterController.Move(movement * Time.deltaTime);
    }

    private void UpdateVerticalVelocity()
    {
        if (_characterController.isGrounded && _characterController.velocity.y < 0)
        {
            verticalVelocity = stickToGroundVelocity;
        }
        verticalVelocity -= gravity * Time.deltaTime;
        
        _characterController.Move(verticalVelocity * Vector3.up);
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
        //_characterController.height = 1f;
        speed = baseSpeed / 2f;
    }

    private void GetUp()
    {
        //_characterController.height = 2f;
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
}

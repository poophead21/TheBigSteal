using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] public float speed = 5f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private Transform cameraTransform;
    
    private Vector3 input;
    private Vector3 velocity;
    private CharacterController characterController;
    private Animator _animator;
    
    public bool isCrouching = false;
    public bool isPushing = false;
    public bool isRotating = false;
    public bool isRunning = false;
    
    private Vector3 sideVelocity;
    private Vector3 forwardVelocity;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        _animator  = GetComponent<Animator>();
    }
    private void Update()
    {
        UpdateVelocity();
        
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isRunning)
        {
            isCrouching = !isCrouching;
            _animator.SetBool("IsCrouching", isCrouching);

            if (isCrouching)
            {
                Crouch();
            }
            else
            {
                GetUp();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isCrouching)
        {
            isRunning = !isRunning;
            _animator.SetBool("IsRunning", isRunning);
            
            if (isRunning)
            {
                Run();
            }
            else
            {
                StopRunning();
            }
        }

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


    private void UpdateVelocity()
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
        
        float movementAnimation  = Mathf.Clamp01(movementDirection.magnitude);
        _animator.SetFloat("Velocity", movementAnimation, 0.1f, Time.deltaTime);

        if (movementDirection.sqrMagnitude < 0.01f) return;
        movementDirection.Normalize();
        
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
        //Vector3 movement = transform.forward * speed;
        Vector3 movement = movementDirection * speed;

        
        characterController.Move(movement * Time.deltaTime);
    }

    private void Crouch()
    {
        characterController.height = 1f;
        speed = speed / 2f;
    }

    private void Run()
    {
        speed = speed * 2f;
    }

    private void GetUp()
    {
        characterController.height = 2f;
        speed = speed * 2f;
    }

    private void StopRunning()
    {
        speed = speed / 2f;
    }
}

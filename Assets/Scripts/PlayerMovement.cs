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
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            _animator.SetBool("isCrouching", isCrouching);

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
            _animator.SetBool("isRunning", isRunning);
            
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

        if (movementDirection.sqrMagnitude < 0.01f) return;
        movementDirection.Normalize();
        
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
        Vector3 movement = transform.forward * speed;

        /*float animationVelocity = movement.magnitude;
        _animator.SetFloat("Velocity", Mathf.Clamp01(animationVelocity), 0.1f, Time.deltaTime);
        Debug.Log(animationVelocity);*/
        
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

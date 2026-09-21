using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] public float speed;
    [SerializeField] private float rotationSpeed = 360f;
    
    private Vector3 input;
    private Vector3 velocity;
    private CharacterController characterController;
    private Animator _animator;
    
    public bool isCrouching = false;
    public bool isRunning = false;
    
    private bool isTurned = false; 
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

        RotatePlayer();

        ApplyTotalVelocity();

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
        float forwardInput = Input.GetAxisRaw("Vertical");
        float sideInput = Input.GetAxisRaw("Horizontal");
        
       Vector3 movement = new Vector3(sideInput, 0, forwardInput);
        /*Quaternion rotation = Quaternion.LookRotation(movement);
        transform.rotation = rotation;*/
        if (movement != Vector3.zero)
        {
           Quaternion targetRotation = Quaternion.LookRotation(movement);
           transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        if (forwardInput < 0)
        {
            forwardInput *= -1f;
        }
        else if (sideInput < 0)
        {
            sideInput *= -1f;
        }

        if (forwardInput != 0 && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            sideInput = 0;
            _animator.SetFloat("Velocity", forwardInput, 0.1f, Time.deltaTime);
        }
        else if (sideInput != 0 && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)))
        {
            forwardInput = 0;
            _animator.SetFloat("Velocity", sideInput, 0.1f, Time.deltaTime);
        }
        
        velocity = forwardInput * transform.forward + sideInput * transform.forward;
    }
    
    private void ApplyTotalVelocity()
    {
        characterController.Move(velocity * speed * Time.deltaTime);
    }

    private void RotatePlayer()
    {
        
    }

    private void Look()
    {
        /*float rotationInput = Input.GetAxisRaw("Horizontal");
        
        if (rotationInput == 0) return;

        float rotation = rotationInput * rotationSpeed * Time.deltaTime;

        transform.Rotate(0f, rotation, 0f);*/

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

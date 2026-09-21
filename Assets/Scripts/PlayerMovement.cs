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
    
    private bool isCrouching = false;
    public bool isPushing = false;
    public bool isRotating = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        _animator  = GetComponent<Animator>();
    }
    private void Update()
    {
        UpdateVelocity();
        Look();
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
        float forwardInput = Input.GetAxisRaw("Vertical");
        
        velocity = transform.forward * forwardInput;

        _animator.SetFloat("yVelocity", forwardInput, 0.1f, Time.deltaTime);
    }
    
    private void ApplyTotalVelocity()
    {
        characterController.Move(velocity * speed * Time.deltaTime);
    }

    private void Look()
    {
        float rotationInput = Input.GetAxisRaw("Horizontal");
        
        if (rotationInput == 0) return;

        float rotation = rotationInput * rotationSpeed * Time.deltaTime;

        transform.Rotate(0f, rotation, 0f);

    }

    private void Crouch()
    {
        characterController.height = 1f;
        speed = speed / 2f;
    }

    private void GetUp()
    {
        characterController.height = 2f;
        speed = speed * 2f;
    }
}

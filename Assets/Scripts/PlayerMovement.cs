using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float speed;

    private Vector3 horizontalVelocity;
    private CharacterController characterController;
    
    private Animator _animator;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        _animator  = GetComponent<Animator>();
    }
    private void Update()
    {
        UpdateHorizontalVelocity();

        ApplyTotalVelocity();

        if (Input.GetKeyDown(KeyCode.E))
        {
            _animator.SetTrigger("Interact");
        }
    }


    private void UpdateHorizontalVelocity()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical"); 

        Vector3 horizontal = xInput * transform.right + yInput * transform.forward;
        if (horizontal.magnitude > 1) horizontal.Normalize();

        horizontal = new Vector3(horizontal.x * speed, 0, horizontal.z * speed);

        horizontalVelocity = horizontal;

        _animator.SetFloat("xVelocity", xInput, 0.1f, Time.deltaTime);
        _animator.SetFloat("yVelocity", yInput, 0.1f, Time.deltaTime);
    }


    private void ApplyTotalVelocity()
    {
        var totalMove = horizontalVelocity;

        characterController.Move(totalMove * Time.deltaTime);
    }
}

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float speed;

    private Vector3 horizontalVelocity;
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        UpdateHorizontalVelocity();

        ApplyTotalVelocity();
    }


    private void UpdateHorizontalVelocity()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical"); 

        Vector3 horizontal = xInput * transform.right + yInput * transform.forward;
        if (horizontal.magnitude > 1) horizontal.Normalize();

        horizontal = new Vector3(horizontal.x * speed, 0, horizontal.z * speed);

        horizontalVelocity = horizontal;
    }


    private void ApplyTotalVelocity()
    {
        var totalMove = horizontalVelocity;

        characterController.Move(totalMove * Time.deltaTime);
    }
}

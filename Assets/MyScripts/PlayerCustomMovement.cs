using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PlayerCustomMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpHeight;
    private float gravityScale = 1f;
    private float verticalVelocity;
    private const float gravity = -9.81f;
    private Vector2 size = new Vector2(1, 1);
    private float limitSpeed = -30;
    private bool pressSpace = false;
    private void Start()
    {
        GameInput.Instance.OnJump += GameInput_OnJump;
    }

    private void GameInput_OnJump(object sender, System.EventArgs e)
    {
        pressSpace = true;
    }

    private void Update()
    {

        ApplyVerticalForceAndFloorDetection();
        HandleMovement();
        ApplyLimitSpeed();
        Debug.Log(gravityScale);
    }

    private void ApplyVerticalForceAndFloorDetection()
    {
        verticalVelocity += gravity *gravityScale *Time.deltaTime;
       
        if (IsGrounded() && verticalVelocity < 0)
        {
            verticalVelocity = 0;
            transform.position = new Vector3(transform.position.x, ClosestPoint().y,transform.position.z);
        }
        if (pressSpace && IsGrounded())
        {
            verticalVelocity = (float)Math.Sqrt(-2 * jumpHeight * (gravity * gravityScale));
            pressSpace = false;
        }
        transform.position += new Vector3(0, verticalVelocity, 0) *Time.deltaTime;  
    }
  
    private void ApplyLimitSpeed()
    {
        if (verticalVelocity <= limitSpeed)
        {
            verticalVelocity = limitSpeed;
        }
    }
    private void HandleMovement()
    {
        Vector2 moveDirection = GameInput.Instance.GetInputNormalized();
        transform.position += new Vector3(moveDirection.x,0,0) * moveSpeed * Time.deltaTime;
    }
    private bool IsGrounded()
    {
       
        bool isGrounded = Physics2D.BoxCast(transform.position,size,90f,Vector2.down,verticalVelocity);
        return isGrounded;
    }
    private Vector3 ClosestPoint()
    { 
        bool exitCollision;
        bool isSafe;
        Vector3 closestPoint = transform.position;
        for (float i = 0; i < 100; i+=0.01f)
        {
            isSafe = !Physics2D.BoxCast(transform.position+Vector3.up*0.01f, size, 90f, Vector3.down, verticalVelocity);
            if (!isSafe)
            {
                exitCollision = !Physics2D.BoxCast(transform.position+Vector3.up*i, size, 90f, Vector3.down, verticalVelocity);
                if (exitCollision)
                {
                    closestPoint = transform.position + Vector3.up * i;
                    break;
                }
            }
            
        }
        return closestPoint;
    }
  
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class Player2DMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [Header("Jump&Fall")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float fallGravityScale = 15f;
    [SerializeField] private float jumGravityScale = 5f;
    [Range(0f, 1f)]
    [SerializeField] private float jumpBufferSensivity = 0.8f;
    [Range(0f, 0.3f)]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float limitFallSpeed = -30f;
    [Header("Test")]
    [SerializeField] private bool Test_SpacePosition = false;
    [SerializeField] private GizmoTest gizmoTest;
    [Header("Others")]
    [SerializeField] private Transform jumpTrace;


    private float coyoteTimer;
    private float jumpVelocity = 10;
    private bool jumpBuffer = false;
    private Rigidbody2D rb;
    private Collider2D boxCollider2D;
    private Transform lastTrace = null;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<Collider2D>();
    }
    private void Start()
    {
        GameInput.Instance.OnJump += GameInput_OnJump;
    }

    private void Update()
    {
        FallOptimized();
        CoyoteTime();
        Debug.Log(rb.velocity.x);
        BufferJump();
        HandleMovement();
        Test();
       
    }

    #region Move
    private void HandleMovement()
    {
        float movedHorizantalirection = GameInput.Instance.GetInputNormalized().x;
        rb.velocity = new Vector2(movedHorizantalirection * moveSpeed, rb.velocity.y);
    }
    #endregion
    #region Jump
    private void Jump()
    {
        rb.gravityScale = jumGravityScale;
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        jumpVelocity = (float)Math.Sqrt(-2 * jumpHeight * (Physics2D.gravity.y * rb.gravityScale));
        rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
    }

    private void GameInput_OnJump(object sender, EventArgs e)
    {

        if (Test_SpacePosition)
        {
            if (lastTrace != null)
            {
                Destroy(lastTrace.gameObject);
            }
            lastTrace = Instantiate(jumpTrace, new Vector3(transform.position.x, transform.position.y, transform.position.z), new Quaternion(0, 0, 0, 0));
        }
        
        if (IsGrounded() || coyoteTimer > 0f)
        {
            Jump();
            coyoteTimer = 0f;
        }
        else if (IsJumpBufferable())
        {
            jumpBuffer = true;
        }
        
    }
    #endregion
    #region FallOptimization
    private void FallOptimized()
    {

        if (rb.velocity.y < limitFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, limitFallSpeed);
        }
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = fallGravityScale;
        }
        else
        {
            rb.gravityScale = jumGravityScale;
        }
    }

    #endregion
    #region Coyote&GroundCheck
    private bool IsGrounded()
    {
        float extraHeight = 0.1f;
        bool isGrounded = Physics2D.BoxCast(boxCollider2D.bounds.center - new Vector3(0, boxCollider2D.bounds.extents.y, 0), new Vector2(boxCollider2D.bounds.size.x, extraHeight), 0f, Vector2.down, extraHeight, groundLayer);
        return isGrounded;
    }
    private void CoyoteTime()
    {
        if (IsGrounded())
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
    }
    #endregion
    #region JumpBuffer
    private bool IsJumpBufferable()
    {
        float jumpBufferHeight = jumpBufferSensivity * jumpHeight;
        bool isFalling = rb.velocity.y < -0.1;
        bool jumpBufferCheck = Physics2D.BoxCast(boxCollider2D.bounds.center - new Vector3(0, boxCollider2D.bounds.extents.y, 0), new Vector2(boxCollider2D.bounds.size.x, 0.1f), 0f, Vector2.down, jumpBufferHeight, groundLayer);
        return jumpBufferCheck && isFalling;
    }

    private void BufferJump()
    {
        if (jumpBuffer && rb.velocity.y == 0)
        {
            Jump();
            jumpBuffer = false;

        }
    }
    #endregion
    #region Test
    public enum GizmoTest
    {
        None,
        Test_JumpBuffer,
        Test_GroundCheck,

    }
    private void Test()
    {
        if (gizmoTest == GizmoTest.Test_GroundCheck)
        {

            float extraHeight = 0.1f;
            //extraHeight = jumpBufferSensivity * jumpHeight;


            Color color = Color.green;
            if (coyoteTimer <= 0f)
            {
                color = Color.red;

            }
            Debug.DrawLine(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, 0, 0), boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, -(boxCollider2D.bounds.extents.y + extraHeight), 0), color);
            Debug.DrawLine(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, 0, 0), boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, (boxCollider2D.bounds.extents.y + extraHeight), 0), color);
            Debug.DrawLine(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, -(boxCollider2D.bounds.extents.y + extraHeight), 0), boxCollider2D.bounds.center + new Vector3(-(boxCollider2D.bounds.extents.x), -(boxCollider2D.bounds.extents.y + extraHeight), 0), color);

        }
        else if (gizmoTest == GizmoTest.Test_JumpBuffer)
        {
            float extraHeight = 0.1f;
            extraHeight = jumpBufferSensivity * jumpHeight;


            Color color = Color.green;
            if (!IsJumpBufferable())
            {
                color = Color.red;

            }
            Debug.DrawLine(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, 0, 0), boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, -(boxCollider2D.bounds.extents.y + extraHeight), 0), color);
            Debug.DrawLine(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, 0, 0), boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, (boxCollider2D.bounds.extents.y + extraHeight), 0), color);
            Debug.DrawLine(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, -(boxCollider2D.bounds.extents.y + extraHeight), 0), boxCollider2D.bounds.center + new Vector3(-(boxCollider2D.bounds.extents.x), -(boxCollider2D.bounds.extents.y + extraHeight), 0), color);

        }
    }
    #endregion




}

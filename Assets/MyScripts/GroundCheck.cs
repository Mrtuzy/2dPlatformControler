using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public static GroundCheck Instance { get; private set; }
    public event EventHandler OnGround;
    public event EventHandler OnExitGround;

    private void Awake()
    {
        Instance = this;

    }
    private void Start()
    {
        OnGround?.Invoke(this, EventArgs.Empty);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnGround?.Invoke(this, EventArgs.Empty);
        Debug.Log("a");
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        OnExitGround?.Invoke(this, EventArgs.Empty);
    }
}

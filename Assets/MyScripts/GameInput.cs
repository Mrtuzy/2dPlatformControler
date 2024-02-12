using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; set; }
    private ControlInputAction controlInputAction;
    public event EventHandler OnJump;
    private void Awake()
    {
        Instance = this;
        controlInputAction = new ControlInputAction();
        controlInputAction.Player.Enable();
        controlInputAction.Player.Jump.performed += Jump_performed;
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJump?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetInputNormalized()
    {
        Vector2 ýnput = controlInputAction.Player.Move.ReadValue<Vector2>();
        Vector2 ýnputNormalized = ýnput.normalized;
        return ýnputNormalized;
    }
}

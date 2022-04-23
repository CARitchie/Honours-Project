using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public static InputController Instance;

    public static event Action Jump;
    public static event Action Interact;
    public static event Action Exit;
    public static event Action Pause;
    public static event Action GodMode;
    public static event Action SummonAmmo;

    PlayerInput input;

    private void Awake()
    {
        if (Instance == null)
        {
            // Keep this object loaded
            Instance = this;
            DontDestroyOnLoad(gameObject);
            input = GetComponent<PlayerInput>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static PlayerInput GetInput()
    {
        if (Instance == null) return null;

        return Instance.input;
    }

    // Function to change the current input action map
    public static void SetMap(string map)
    {
        if (Instance == null) return;

        Instance.input.SwitchCurrentActionMap(map);
    }

    public InputAction FindAction(string action)
    {
        return input.actions.FindAction(action);
    }

    void OnJump()
    {
        Jump?.Invoke();
    }

    void OnPause()
    {
        Pause?.Invoke();
    }

    void OnSummonAmmo()
    {
        if (PlayerController.IsPaused()) return;
        SummonAmmo?.Invoke();
    }

    void OnInteract()
    {
        if (PlayerController.IsPaused()) return;
        Interact?.Invoke();
    }

    void OnExitShip()
    {
        if (PlayerController.IsPaused()) return;
        Exit?.Invoke();
    }

    void OnGodMode()
    {
        GodMode?.Invoke();
    }
}

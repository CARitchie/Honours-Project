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

    PlayerInput input;

    private void Awake()
    {
        if (Instance == null)
        {
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

    public static void SetMap(string map)
    {
        if (Instance == null) return;

        Instance.input.SwitchCurrentActionMap(map);
    }

    void OnJump()
    {
        Jump?.Invoke();
    }

    void OnInteract()
    {
        Interact?.Invoke();
    }

    void OnExitShip()
    {
        Exit?.Invoke();
    }
}

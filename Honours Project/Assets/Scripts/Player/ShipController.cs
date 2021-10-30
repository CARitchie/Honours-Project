using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    [SerializeField] float engineStrength;
    [SerializeField] float sensitivity;
    [SerializeField] float maxRotation = 1000;
    [SerializeField] GameObject cam;

    InputAction[] shipControls = new InputAction[8];
    InputAction lookAction;
    PlayerInput input;
    Vector3 angVel;
    bool active = false;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        input = InputController.GetInput();

        if (input != null)
        {
            shipControls[0] = input.actions.FindAction("ShipForward");
            shipControls[1] = input.actions.FindAction("ShipRight");
            shipControls[2] = input.actions.FindAction("ShipBack");
            shipControls[3] = input.actions.FindAction("ShipLeft");
            shipControls[4] = input.actions.FindAction("ShipUp");
            shipControls[5] = input.actions.FindAction("ShipDown");
            shipControls[6] = input.actions.FindAction("ShipRotRight");
            shipControls[7] = input.actions.FindAction("ShipRotLeft");

            lookAction = input.actions.FindAction("ShipLook");
        }

        InputController.Exit += Deactivate;
    }

    private void OnDestroy()
    {
        InputController.Exit -= Deactivate;
    }

    private void FixedUpdate()
    {
        if (!active) return;

        Movement();
        Look();
    }

    public void Activate()
    {
        cam.SetActive(true);
        active = true;
        InputController.SetMap("Ship");
        PlayerController.Instance.Deactivate();
    }

    public void Deactivate()
    {
        active = false;
        cam.SetActive(false);
        InputController.SetMap("Player");

        PlayerController.Instance.SetRotation(transform.eulerAngles);
        PlayerController.Instance.SetPosition(transform.position + transform.up);
        PlayerController.Instance.ForceVelocity(rb.velocity);
        PlayerController.Instance.Activate();
    }

    void Movement()
    {
        Vector3 move = transform.forward * (shipControls[0].ReadValue<float>() - shipControls[2].ReadValue<float>());
        move += transform.right * (shipControls[1].ReadValue<float>() - shipControls[3].ReadValue<float>());
        move += transform.up * (shipControls[4].ReadValue<float>() - shipControls[5].ReadValue<float>());

        move *= engineStrength;
        move *= Time.fixedDeltaTime;

        rb.AddForce(move, ForceMode.VelocityChange);
    }

    void Look()
    {
        float time = Time.fixedDeltaTime;

        // 0.5 and 0.1 are necessary to make motion smoother https://forum.unity.com/threads/mouse-delta-input.646606/
        Vector2 look = lookAction.ReadValue<Vector2>() * 0.5f * 0.1f * time;

        angVel.x -= look.y * sensitivity;
        angVel.y += look.x * sensitivity;

        angVel.z -= (shipControls[6].ReadValue<float>() - shipControls[7].ReadValue<float>())  * sensitivity * 0.6f * time;

        // 0.08 makes rotation fade out rather than instantly stop
        angVel -= angVel.normalized * angVel.sqrMagnitude * 0.08f * time;

        // Added in to prevent Quaternions from becoming infinite and then breaking
        angVel.x = Mathf.Clamp(angVel.x, -maxRotation, maxRotation);
        angVel.y = Mathf.Clamp(angVel.y, -maxRotation, maxRotation);
        angVel.z = Mathf.Clamp(angVel.z, -maxRotation, maxRotation);

        transform.Rotate(angVel * time);

        rb.angularVelocity = Vector3.zero;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PersonController
{
    [Header("Player Settings")]
    [SerializeField] float jumpStrength = 5;
    [SerializeField] Transform dud;

    float verticalLook = 0;

    public static PlayerController Instance;

    InputAction[] movementActions = new InputAction[4];
    InputAction lookAction;
    InputAction sprintAction;
    Transform cam;
    PlayerInput input;

    bool doubleJumped = false;

    protected override void Awake()
    {
        base.Awake();

        Instance = this;
        cam = GetComponentInChildren<Camera>().transform;
    }

    protected override void Start()
    {
        base.Start();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        input = InputController.GetInput();

        if(input != null)
        {
            movementActions[0] = input.actions.FindAction("MoveForward");
            movementActions[1] = input.actions.FindAction("MoveRight");
            movementActions[2] = input.actions.FindAction("MoveBack");
            movementActions[3] = input.actions.FindAction("MoveLeft");

            lookAction = input.actions.FindAction("Look");
            sprintAction = input.actions.FindAction("Sprint");
        }

        InputController.Jump += Jump;    
    }

    private void OnDestroy()
    {
        InputController.Jump -= Jump;
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        Look();
    }

    protected override void Move()
    {
        float forward = movementActions[0].ReadValue<float>() - movementActions[2].ReadValue<float>();
        float sideways = movementActions[1].ReadValue<float>() - movementActions[3].ReadValue<float>();

        Vector3 moveDirection = forward * transform.forward + sideways * transform.right;

        if (moveDirection == Vector3.zero) return;

        if(sprintAction.ReadValue<float>() > 0)
        {
            movementSpeed = sprintSpeed;
        }
        else
        {
            movementSpeed = walkSpeed;
        }

        Vector3 target = rb.position + (moveDirection * movementSpeed * Time.deltaTime);

        rb.MovePosition(target);
    }

    void Look()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();

        if(nearestSource != null)
        {
            verticalLook += -look.y * lookSensitivity;
            verticalLook = Mathf.Clamp(verticalLook, -90, 90);

            transform.localEulerAngles += new Vector3(0, look.x, 0) * lookSensitivity;
            cam.localEulerAngles = new Vector3(verticalLook, 0, 0);
        }
        else
        {
            //transform.parent.localEulerAngles += new Vector3(-look.y, 0, 0) * lookSensitivity;
            //transform.Rotate(transform.right, -look.y * lookSensitivity);

            verticalLook = 0;

            Vector3 camRot = cam.localEulerAngles;
            Vector3 localRot = transform.localEulerAngles;
            bool instant = false;
            float corrector1 = 0;
            float corrector2 = 0;
            if (camRot.x != 0 || localRot.y != 0)
            {
                corrector1 = camRot.x;
                cam.localEulerAngles = new Vector3(0, camRot.y, camRot.z);

                corrector2 = localRot.y;
                transform.localEulerAngles = new Vector3(localRot.x, 0, localRot.z);

                instant = true;
            }

            Quaternion rot = dud.rotation;
            dud.Rotate(Vector3.up, (look.x * lookSensitivity) + corrector2, Space.Self);
            dud.Rotate(Vector3.right, (-look.y * lookSensitivity) + corrector1, Space.Self);
            Quaternion newRot = dud.rotation;
            dud.rotation = rot;

            if (!instant)
            {
                rb.MoveRotation(newRot);
            }
            else
            {
                rb.transform.rotation = newRot;
            }
        }
        
    }

    void Jump()
    {
        if (!grounded && doubleJumped) return;

        float strength = jumpStrength;

        if (!grounded && !doubleJumped)
        {
            doubleJumped = true;
            strength *= 1.2f;
        }

        rb.AddForce(transform.up * strength, ForceMode.VelocityChange);
        
    }

    protected override void CheckGrounded()
    {
        base.CheckGrounded();

        if (grounded) doubleJumped = false;
    }

    public void Activate()
    {
        transform.parent.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        transform.parent.gameObject.SetActive(false);
        transform.localEulerAngles = Vector3.zero;
        cam.localEulerAngles = Vector3.zero;
    }
}

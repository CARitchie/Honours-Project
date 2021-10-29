using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpStrength = 5;
    [SerializeField] float sensitivity;

    float verticalLook = 0;

    InputAction[] movementActions = new InputAction[4];
    InputAction lookAction;
    Rigidbody rb;
    Transform cam;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
        cam = GetComponentInChildren<Camera>().transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        PlayerInput input = InputController.GetInput();

        if(input != null)
        {
            movementActions[0] = input.actions.FindAction("MoveForward");
            movementActions[1] = input.actions.FindAction("MoveRight");
            movementActions[2] = input.actions.FindAction("MoveBack");
            movementActions[3] = input.actions.FindAction("MoveLeft");

            lookAction = input.actions.FindAction("Look");
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
        Movement();

        Look();
    }

    void Movement()
    {
        float forward = movementActions[0].ReadValue<float>() - movementActions[2].ReadValue<float>();
        float sideways = movementActions[1].ReadValue<float>() - movementActions[3].ReadValue<float>();

        Vector3 moveDirection = forward * transform.forward + sideways * transform.right;

        transform.parent.position += moveDirection * speed * Time.deltaTime;
    }

    void Look()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();

        verticalLook += -look.y * sensitivity * Time.deltaTime;
        verticalLook = Mathf.Clamp(verticalLook, -90, 90);

        transform.localEulerAngles += new Vector3(0, look.x, 0) * sensitivity * Time.deltaTime;
        cam.localEulerAngles = new Vector3(verticalLook, 0, 0);
    }

    void Jump()
    {
        rb.AddForce(transform.up * jumpStrength, ForceMode.VelocityChange);
    }
}
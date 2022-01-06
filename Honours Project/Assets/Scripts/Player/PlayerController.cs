using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float jumpStrength = 5;
    [SerializeField] float sensitivity;

    float verticalLook = 0;
    float movementSpeed;

    public static PlayerController Instance;

    InputAction[] movementActions = new InputAction[4];
    InputAction lookAction;
    InputAction sprintAction;
    Rigidbody rb;
    Transform cam;
    PlayerInput input;

    Vector3 lastVelocity;

    private void Awake()
    {
        Instance = this;
        rb = GetComponentInParent<Rigidbody>();
        cam = GetComponentInChildren<Camera>().transform;
    }

    private void Start()
    {
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

        lastVelocity = rb.velocity;
        
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

    void FixedUpdate(){
        VelocityCheck();
    }

    void Movement()
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

        verticalLook += -look.y * sensitivity;
        verticalLook = Mathf.Clamp(verticalLook, -90, 90);

        transform.localEulerAngles += new Vector3(0, look.x, 0) * sensitivity;
        cam.localEulerAngles = new Vector3(verticalLook, 0, 0);
    }

    void Jump()
    {
        rb.AddForce(transform.up * jumpStrength, ForceMode.VelocityChange);
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

    public void ForceVelocity(Vector3 velocity)
    {
        rb.velocity = velocity;
        lastVelocity = velocity;
    }

    public void SetPosition(Vector3 position)
    {
        transform.parent.position = position;
    }

    public void SetRotation(Vector3 rotation)
    {
        transform.parent.eulerAngles = rotation;
    }

    void VelocityCheck(){
        Vector3 currentVelocity = rb.velocity;
        float deltaV = (lastVelocity - currentVelocity).magnitude;
        lastVelocity = currentVelocity;

        if(deltaV > 15) Debug.LogError("Velocity death: " + deltaV + "m/s");
    }
}

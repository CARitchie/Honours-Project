using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PersonController
{
    [Header("Player Settings")]
    [SerializeField] float jumpStrength = 5;
    [SerializeField] Transform dud;
    [SerializeField] Weapon initialWeapon;

    Weapon weapon;

    float verticalLook = 0;

    public static PlayerController Instance;

    InputAction[] movementActions = new InputAction[8];
    InputAction lookAction;
    InputAction sprintAction;
    InputAction weaponAction;
    InputAction weaponSecondaryAction;

    [SerializeField] Transform cam;
    PlayerInput input;

    float fuel;
    float maxFuel = 200;

    bool inSpace = false;
    bool doubleJumped = false;

    protected override void Awake()
    {
        base.Awake();

        Instance = this;
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
            movementActions[4] = input.actions.FindAction("RotRight");
            movementActions[5] = input.actions.FindAction("RotLeft");
            movementActions[6] = input.actions.FindAction("MoveUp");
            movementActions[7] = input.actions.FindAction("MoveDown");

            lookAction = input.actions.FindAction("Look");
            sprintAction = input.actions.FindAction("Sprint");
            weaponAction = input.actions.FindAction("Primary");
            weaponSecondaryAction = input.actions.FindAction("Secondary");

        }

        InputController.Jump += Jump;

        fuel = maxFuel;

        SwapWeapon(initialWeapon);
        initialWeapon = null;
    }

    private void OnDestroy()
    {
        InputController.Jump -= Jump;
    }

    // Update is called once per frame
    void Update()
    {
        inSpace = nearestSource == null;

        Move();

        Look();
    }
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        UseWeapon();

        AddForce(rb.velocity);
        rb.velocity = Vector3.zero;
    }

    public override void Move()
    {
        float forward = movementActions[0].ReadValue<float>() - movementActions[2].ReadValue<float>();
        float sideways = movementActions[1].ReadValue<float>() - movementActions[3].ReadValue<float>();

        Vector3 moveDirection = forward * transform.forward + sideways * transform.right;

        if (!inSpace)
        {
            if (sprintAction.ReadValue<float>() > 0)
            {
                movementSpeed = sprintSpeed;
            }
            else
            {
                movementSpeed = walkSpeed;
            }
        }
        else
        {
            movementSpeed = walkSpeed * 0.8f;
            float up = movementActions[6].ReadValue<float>() - movementActions[7].ReadValue<float>();
            moveDirection += up * transform.up;
        }

        if (moveDirection == Vector3.zero) return;

        Vector3 velocity = moveDirection * movementSpeed * Time.deltaTime;

        if (!inSpace)
        {
            rb.MovePosition(rb.position + velocity);
        }
        else
        {
            if(UseFuel(velocity)) rb.AddForce(velocity, ForceMode.VelocityChange);
        }

    }

    void Look()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();

        if(!inSpace)
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

            float zAngle = (movementActions[4].ReadValue<float>() - movementActions[5].ReadValue<float>()) * Time.deltaTime * 80;

            Quaternion rot = dud.rotation;
            dud.Rotate(Vector3.up, (look.x * lookSensitivity) + corrector2, Space.Self);
            dud.Rotate(Vector3.right, (-look.y * lookSensitivity) + corrector1, Space.Self);
            dud.Rotate(Vector3.forward, -zAngle, Space.Self);
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
        if (inSpace || (!grounded && doubleJumped)) return;

        float strength = jumpStrength;

        if (!grounded && !doubleJumped)
        {
            doubleJumped = true;
            strength *= 1.2f;
        }

        AddForce(transform.up * strength);
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

    void IncreaseFuel(float amount)
    {
        fuel = Mathf.Clamp(fuel + amount, 0, maxFuel);
    }

    bool UseFuel(Vector3 amount)
    {
        if (fuel <= 0) return false;

        float reduction = amount.magnitude;
        fuel = Mathf.Clamp(fuel - reduction, 0, maxFuel);
        Debug.Log(fuel);
        return true;
    }

    void UseWeapon()
    {
        if (weapon == null) return;

        weapon.PrimaryAction(weaponAction.ReadValue<float>());
        weapon.SecondaryAction(weaponSecondaryAction.ReadValue<float>());
    }

    void SwapWeapon(Weapon newWeapon)
    {
        if (weapon != null) weapon.OnUnEquip();

        weapon = newWeapon;

        if (weapon != null) weapon.OnEquip(this);
    }

    public override Transform ProjectileSpawnPoint()
    {
        return cam;
    }

    public override void AddForce(Vector3 force)
    {
        GravityController.AddToPlayerVelocity(force);
    }

    public Transform GetCameraHolder()
    {
        return cam;
    }

    public override void Recoil(float strength)
    {
        AddForce(strength * -cam.forward);
    }
}

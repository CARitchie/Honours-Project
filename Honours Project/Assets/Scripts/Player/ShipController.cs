using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cameraHolder;
    [SerializeField] CameraController cam;
    [SerializeField] GameObject compassIcon;

    [Header("Settings")]
    [SerializeField] float engineStrength;
    [SerializeField] float sensitivity;
    [SerializeField] float rollSensitivity;
    [SerializeField] float maxRotation = 1000;
    [SerializeField] float matchVelocitySpeed = 8;


    InputAction[] shipControls = new InputAction[8];
    InputAction lookAction;
    InputAction matchVeloAction;
    PlayerInput input;
    Vector3 angVel;
    bool active = false;
    Rigidbody rb;

    float originalStrength;
    float originalMatchSpeed;

    public static ShipController Instance;

    private void Awake()
    {
        Instance = this;

        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalStrength = engineStrength;
        originalMatchSpeed = matchVelocitySpeed;
    }

    private void Start()
    {
        input = InputController.GetInput();

        if (input != null)
        {
            // Find and store all of the relevant input actions
            // Done so that they don't need to be found every time they need to be accessed
            shipControls[0] = input.actions.FindAction("ShipForward");
            shipControls[1] = input.actions.FindAction("ShipRight");
            shipControls[2] = input.actions.FindAction("ShipBack");
            shipControls[3] = input.actions.FindAction("ShipLeft");
            shipControls[4] = input.actions.FindAction("ShipUp");
            shipControls[5] = input.actions.FindAction("ShipDown");
            shipControls[6] = input.actions.FindAction("ShipRotRight");
            shipControls[7] = input.actions.FindAction("ShipRotLeft");

            lookAction = input.actions.FindAction("ShipLook");
            matchVeloAction = input.actions.FindAction("ShipMatchVelo");
        }

        InputController.Exit += Deactivate;

        if (!LoadData())
        {
            if (GravityController.FindSource("planet_jungle", out GravitySource source)) SetVelocity(source.GetVelocity());     // Match the ship's velocity to that of Coille if no save was found
        }

        SettingsManager.OnChangesMade += LoadSensitivity;
        LoadSensitivity();

        SaveManager.OnUpgradeChanged += LoadUpgrades;
        LoadUpgrades();
    }

    // Function to load in the ship's data from the save file
    // Returns false if no position could be loaded
    bool LoadData()
    {
        if (!SaveManager.SaveExists()) return false;

        RelativeTransform relTransform = SaveManager.GetShipTransform();

        if (relTransform == null) return false;

        if(relTransform.LoadIntoTransform(transform, out Vector3 velocity))
        {
            SetVelocity(velocity);
        }

        if (SaveManager.GetState() >= 2) compassIcon.SetActive(true);       // Activate the ship's compass icon if it has been flown before

        return true;
    }

    void LoadSensitivity()
    {
        // Load the ship sensitivity from the user settings
        if (PlayerPrefs.HasKey("Ship"))
        {
            int val = PlayerPrefs.GetInt("Ship");
            sensitivity = 8 * ((float)val / 5);
        }
    }

    void LoadUpgrades()
    {
        // Increase the speed of the ship if the thruster upgrade has been unlocked
        if (SaveManager.SelfUpgraded("upgrade_thruster"))
        {
            engineStrength = originalStrength * 3;
            matchVelocitySpeed = originalMatchSpeed * 4;
        }
    }

    private void OnDestroy()
    {
        InputController.Exit -= Deactivate;
        SettingsManager.OnChangesMade -= LoadSensitivity;
    }

    private void FixedUpdate()
    {
        if (!active) return;

        Movement();
        //Look();

        MatchVelocity();

        // Used to ensure that the ship remains still when using physics, and that its the other objects that move around it
        AddForce(rb.velocity);
        rb.velocity = Vector3.zero;
    }

    void MatchVelocity()
    {
        Vector3 target = cam.UpdatePlanetHUD(rb);
        if (matchVeloAction.ReadValue<float>() > 0)         // If the match velocity button is held
        {
            target = Vector3.MoveTowards(rb.velocity, target, Time.fixedDeltaTime * matchVelocitySpeed);        // Move the velocity towards that of the target's
            target -= rb.velocity;
            AddForce(target);
        }
    }

    private void Update()
    {
        if (!active) return;

        Look();

        PlayerController.Instance.KeepLooping();        // Keep looping through the player's necessary functions even though they have been disabled
    }

    // Function to make the player take control of the ship
    public void Activate()
    {
        GravityController.SetPlayer(GetComponent<GravityReceiver>());

        cam.MoveToTransform(cameraHolder);              // Move the camera into position
        active = true;
        InputController.SetMap("Ship");                 // Switch to the ship control scheme
        PlayerController.Instance.Deactivate();         // Deactivate the player
        GlobalLightControl.SwitchToShip(transform);     // Make the sun point at the ship
        Compass.SetActive(false);                       // Disable the compass
        compassIcon.SetActive(true);
        AudioControl.AtmosphereInterpolation(1);
        SaveManager.SetGameState(2);                    // Tell the save file that the ship has been flown

        if (!SaveManager.GetBool("hint_takeOff"))
        {
            HintManager.PlayHint("hint_takeOff");       // Play the basic control hints if not already played
            HintManager.PlayHint("hint_roll");
            StartCoroutine(MatchVelocityHint());
        }
    }

    // Function to exit control of the ship
    public void Deactivate()
    {
        active = false;
        InputController.SetMap("Player");               // Switch back to the player control scheme

        GlobalLightControl.SwitchToPlayer();            // Make the sun point at the player

        // Put the player in a position so that it will look like they were standing in the ship the entire time
        // Also force the player to have the same velocity as the ship to prevent the possibility of the player getting launched due to an incorrect velocity
        PlayerController.Instance.SetRotation(transform.eulerAngles);       
        PlayerController.Instance.SetPosition(cameraHolder.position - (PlayerController.Instance.GetCameraHolder().position - PlayerController.Instance.transform.position));
        PlayerController.Instance.ForceVelocity(rb.velocity);
        PlayerController.Instance.Activate();
        Compass.SetActive(true);

        GravityController.SetPlayer(PlayerController.Instance.GetComponentInParent<GravityReceiver>());

        cam.MoveToTransform(PlayerController.Instance.GetCameraHolder());       // Move the camera back to the player

        angVel = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void Movement()
    {
        Vector3 move = transform.forward * (shipControls[0].ReadValue<float>() - shipControls[2].ReadValue<float>());       // Read input from W and S keys
        move += transform.right * (shipControls[1].ReadValue<float>() - shipControls[3].ReadValue<float>());                // Read input from A and D keys
        move += transform.up * (shipControls[4].ReadValue<float>() - shipControls[5].ReadValue<float>()) * 1.2f;            // Read input from Shift and Control keys

        move *= engineStrength;
        move *= Time.fixedDeltaTime;

        AddForce(move);                                                                                                    // Apply input as a force
    }

    void Look()
    {
        float time = Time.deltaTime;

        // 0.5 and 0.1 are necessary to make motion smoother https://forum.unity.com/threads/mouse-delta-input.646606/
        Vector2 look = lookAction.ReadValue<Vector2>() * 0.5f * 0.1f;           // Read mouse movement input

        angVel.x -= look.y * sensitivity;
        angVel.y += look.x * sensitivity;

        angVel.z -= (shipControls[6].ReadValue<float>() - shipControls[7].ReadValue<float>())  * rollSensitivity * time;        // Read input for Q and E keys, used for rolling

        // 0.08 makes rotation fade out rather than instantly stop
        angVel -= angVel.normalized * angVel.sqrMagnitude * 0.08f * time;

        // Added in to prevent Quaternions from becoming infinite and then breaking
        angVel.x = Mathf.Clamp(angVel.x, -maxRotation, maxRotation);
        angVel.y = Mathf.Clamp(angVel.y, -maxRotation, maxRotation);
        angVel.z = Mathf.Clamp(angVel.z, -maxRotation, maxRotation);

        rb.MoveRotation(rb.rotation * Quaternion.Euler(angVel * time));         // Apply rotation

        rb.angularVelocity = Vector3.zero;                                      // Prevent any unwanted rotation as a result of collision
    }

    public void AddForce(Vector3 force)
    {
        GravityController.AddToPlayerVelocity(force);
    }

    public bool IsActive()
    {
        return active;
    }

    public void SetVelocity(Vector3 velo)
    {
        rb.velocity = velo;
    }

    IEnumerator MatchVelocityHint()
    {
        yield return new WaitForSeconds(15);
        HintManager.PlayHint("hint_match");
    }
}

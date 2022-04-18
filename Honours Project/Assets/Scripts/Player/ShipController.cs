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
            if (GravityController.FindSource("planet_jungle", out GravitySource source)) SetVelocity(source.GetVelocity());
        }

        SettingsManager.OnChangesMade += LoadSensitivity;
        LoadSensitivity();

        SaveManager.OnUpgradeChanged += LoadUpgrades;
        LoadUpgrades();
    }

    bool LoadData()
    {
        if (!SaveManager.SaveExists()) return false;

        RelativeTransform relTransform = SaveManager.GetShipTransform();

        if (relTransform == null) return false;

        if(relTransform.LoadIntoTransform(transform, out Vector3 velocity))
        {
            SetVelocity(velocity);
        }

        if (SaveManager.GetState() >= 2) compassIcon.SetActive(true);

        return true;
    }

    void LoadSensitivity()
    {
        if (PlayerPrefs.HasKey("Ship"))
        {
            int val = PlayerPrefs.GetInt("Ship");
            sensitivity = 8 * ((float)val / 5);
        }
    }

    void LoadUpgrades()
    {
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

        AddForce(rb.velocity);
        rb.velocity = Vector3.zero;
    }

    void MatchVelocity()
    {
        Vector3 target = cam.UpdatePlanetHUD(rb);
        if (matchVeloAction.ReadValue<float>() > 0)
        {
            target = Vector3.MoveTowards(rb.velocity, target, Time.fixedDeltaTime * matchVelocitySpeed);
            target -= rb.velocity;
            AddForce(target);
        }
    }

    private void Update()
    {
        if (!active) return;

        Look();

        PlayerController.Instance.KeepLooping();
    }

    public void Activate()
    {
        GravityController.SetPlayer(GetComponent<GravityReceiver>());

        cam.MoveToTransform(cameraHolder);
        active = true;
        InputController.SetMap("Ship");
        PlayerController.Instance.Deactivate();
        GlobalLightControl.SwitchToShip(transform);
        Compass.SetActive(false);
        compassIcon.SetActive(true);
        AudioControl.AtmosphereInterpolation(1);
        SaveManager.SetGameState(2);

        if (!SaveManager.GetBool("hint_takeOff"))
        {
            HintManager.PlayHint("hint_takeOff");
            HintManager.PlayHint("hint_roll");
            StartCoroutine(MatchVelocityHint());
        }
    }

    public void Deactivate()
    {
        active = false;
        InputController.SetMap("Player");

        GlobalLightControl.SwitchToPlayer();
        PlayerController.Instance.SetRotation(transform.eulerAngles);
        PlayerController.Instance.SetPosition(cameraHolder.position - (PlayerController.Instance.GetCameraHolder().position - PlayerController.Instance.transform.position));
        PlayerController.Instance.ForceVelocity(rb.velocity);
        PlayerController.Instance.Activate();
        Compass.SetActive(true);

        GravityController.SetPlayer(PlayerController.Instance.GetComponentInParent<GravityReceiver>());

        cam.MoveToTransform(PlayerController.Instance.GetCameraHolder());

        angVel = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void Movement()
    {
        Vector3 move = transform.forward * (shipControls[0].ReadValue<float>() - shipControls[2].ReadValue<float>());
        move += transform.right * (shipControls[1].ReadValue<float>() - shipControls[3].ReadValue<float>());
        move += transform.up * (shipControls[4].ReadValue<float>() - shipControls[5].ReadValue<float>()) * 1.2f;

        move *= engineStrength;
        move *= Time.fixedDeltaTime;

        AddForce(move);
    }

    void Look()
    {
        float time = Time.deltaTime;

        // 0.5 and 0.1 are necessary to make motion smoother https://forum.unity.com/threads/mouse-delta-input.646606/
        Vector2 look = lookAction.ReadValue<Vector2>() * 0.5f * 0.1f;

        angVel.x -= look.y * sensitivity;
        angVel.y += look.x * sensitivity;

        angVel.z -= (shipControls[6].ReadValue<float>() - shipControls[7].ReadValue<float>())  * rollSensitivity * time;

        // 0.08 makes rotation fade out rather than instantly stop
        angVel -= angVel.normalized * angVel.sqrMagnitude * 0.08f * time;

        // Added in to prevent Quaternions from becoming infinite and then breaking
        angVel.x = Mathf.Clamp(angVel.x, -maxRotation, maxRotation);
        angVel.y = Mathf.Clamp(angVel.y, -maxRotation, maxRotation);
        angVel.z = Mathf.Clamp(angVel.z, -maxRotation, maxRotation);

        rb.MoveRotation(rb.rotation * Quaternion.Euler(angVel * time));

        rb.angularVelocity = Vector3.zero;
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

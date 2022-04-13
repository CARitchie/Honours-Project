using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : PersonController
{
    [Header("Player Settings")]
    [SerializeField] float jumpStrength = 5;
    [SerializeField] float matchVelocitySpeed = 5;
    [SerializeField] HUD hud;
    [SerializeField] Transform cam;
    [SerializeField] CameraController camController;
    [SerializeField] SummonedAmmo summonedAmmo;
    [SerializeField] bool finalFight;
    Weapon weapon;

    float verticalLook = 0;

    public static PlayerController Instance;

    InputAction[] movementActions = new InputAction[8];
    InputAction lookAction;
    InputAction sprintAction;
    InputAction weaponAction;
    InputAction weaponSecondaryAction;
    InputAction pauseAction;
    InputAction weaponWheelAction;
    InputAction weaponScrollAction;
    InputAction matchVeloAction;

    
    PlayerDetails details;
    WeaponManager weaponManager;
    AudioManager audioManager;

    float fuel;
    float maxFuel = 200;
    float weaponSpeed = 0;
    float evaSpeed;

    bool inSpace = false;
    bool doubleJumped = false;
    bool canSave = true;
    bool walkOnLava = true;
    bool canDoubleJump = true;
    bool canSummon = false;
    float summonCooldown = 0;
    float footstepTimer;
    const float summonDelay = 150;

    bool paused = false;

    bool loadBigGun = false;

    int aimLayerMask = ~((1 << 6) | (1 << 2) | (1 << 11) | (1 << 12) | (1 << 13));

    protected override void Awake()
    {
        base.Awake();

        Instance = this;
        details = GetComponentInParent<PlayerDetails>();
        weaponManager = GetComponentInChildren<WeaponManager>();
        audioManager = GetComponentInParent<AudioManager>();
    }

    protected void Start()
    {
        Application.targetFrameRate = 500;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InputController input = InputController.Instance;

        if(input != null)
        {
            movementActions[0] = input.FindAction("MoveForward");
            movementActions[1] = input.FindAction("MoveRight");
            movementActions[2] = input.FindAction("MoveBack");
            movementActions[3] = input.FindAction("MoveLeft");
            movementActions[4] = input.FindAction("RotRight");
            movementActions[5] = input.FindAction("RotLeft");
            movementActions[6] = input.FindAction("MoveUp");
            movementActions[7] = input.FindAction("MoveDown");

            lookAction = input.FindAction("Look");
            sprintAction = input.FindAction("Sprint");
            weaponAction = input.FindAction("Primary");
            weaponSecondaryAction = input.FindAction("Secondary");
            weaponWheelAction = input.FindAction("WeaponWheel");
            weaponScrollAction = input.FindAction("WeaponScroll");
            matchVeloAction = input.FindAction("MatchVelo");
        }

        InputController.Jump += Jump;
        InputController.Pause += Pause;
        InputController.SummonAmmo += SummonAmmo;

        fuel = maxFuel;
        evaSpeed = walkSpeed * 0.8f;

        if(!finalFight) LoadData();
        LoadWeapon();


        SettingsManager.OnChangesMade += LoadSensitivity;
        LoadSensitivity();

        SaveManager.OnUpgradeChanged += LoadUpgrades;
        LoadUpgrades();
        loadBigGun = true;
    }

    bool LoadData()
    {
        if (!SaveManager.SaveExists()) return false;
        
        Vector3 playerRelativePos = SaveManager.GetRelativePlayerPos();

        if (playerRelativePos == new Vector3(-450000, 0, 0)) return false;

        string key = SaveManager.GetGravitySource();
        if (key == "null" | !GravityController.FindSource(key, out GravitySource source)) return false;

        SetPosition(playerRelativePos + source.transform.position);
        SetAllRotation(SaveManager.save.GetLocalRot(), SaveManager.save.GetParentRot());
        ForceVelocity(source.GetVelocity());

        return true;
    }

    void LoadSensitivity()
    {
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            int val = PlayerPrefs.GetInt("Sensitivity");
            lookSensitivity = 0.2f * ((float)val / 5);
        }
    }

    void LoadUpgrades()
    {
        if (SaveManager.SacrificeMade("sacrifice_lava"))
        {
            walkOnLava = false;
        }

        if (SaveManager.SacrificeMade("sacrifice_speed"))
        {
            walkSpeed = walkSpeed * 0.7f;
            sprintSpeed = sprintSpeed * 0.7f;
        }

        if (SaveManager.SacrificeMade("sacrifice_jump"))
        {
            canDoubleJump = false;
        }

        if (SaveManager.SelfUpgraded("upgrade_teleport"))
        {
            canSummon = true;
        }

        if (SaveManager.SelfUpgraded("upgrade_gun"))
        {
            UnlockWeapon(2);
            if (loadBigGun)
            {
                EquipWeapon(2);
                loadBigGun = false;
            }
        }
    }

    void Pause()
    {
        PauseMenu.TogglePause();

        //Need to force all other ui off
    }

    private void OnDestroy()
    {
        InputController.Jump -= Jump;
        InputController.Pause -= Pause;
        InputController.SummonAmmo -= SummonAmmo;
        SettingsManager.OnChangesMade -= LoadSensitivity;
        SaveManager.OnUpgradeChanged -= LoadUpgrades;
    }

    // Update is called once per frame
    void Update()
    {
        if (paused) return;

        inSpace = nearestSource == null;

        //TODO: Try this in fixedupdate
        Move();

        Look();

        HandleWeaponWheel();

        AmmoSummonCooldown();
    }

    public void KeepLooping()
    {
        details.KeepLooping();
        AmmoSummonCooldown();
    }

    void AmmoSummonCooldown()
    {
        if (canSummon && summonCooldown > 0)
        {
            summonCooldown -= Time.deltaTime;
            hud.SetSummonPercent(1 - (summonCooldown / summonDelay));
            if (summonCooldown <= 0) hud.SetSummonActive(false);
        }
    }
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        UseWeapon();

        CheckPlanetUI();

        AddForce(rb.velocity);
        rb.velocity = Vector3.zero;
    }

    void CheckPlanetUI()
    {
        if (inSpace)
        {
            Vector3 target = camController.UpdatePlanetHUD(rb);
            if(matchVeloAction.ReadValue<float>() > 0)
            {
                target = Vector3.MoveTowards(rb.velocity, target, Time.fixedDeltaTime * matchVelocitySpeed);
                target -= rb.velocity;
                if (details.UseEnergy(target.magnitude)) AddForce(target);
            }
        }
        else
        {
            HUD.SetPlanetTextActive(false);
        }
    }

    public override void Move()
    {
        float gunSpeed = 0;

        float forward = movementActions[0].ReadValue<float>() - movementActions[2].ReadValue<float>();
        float sideways = movementActions[1].ReadValue<float>() - movementActions[3].ReadValue<float>();

        Vector3 moveDirection = forward * transform.forward + sideways * transform.right;

        if (!inSpace)
        {
            if (sprintAction.ReadValue<float>() > 0)
            {
                movementSpeed = sprintSpeed;
                gunSpeed = 1;
            }
            else
            {
                movementSpeed = walkSpeed;
                gunSpeed = 0.5f;
            }
        }
        else
        {
            movementSpeed = evaSpeed;
            float up = movementActions[6].ReadValue<float>() - movementActions[7].ReadValue<float>();
            moveDirection += up * transform.up;
        }

        if (moveDirection == Vector3.zero) { AdjustWeaponSpeed(0); return; }

        AdjustWeaponSpeed(gunSpeed);

        Vector3 velocity = moveDirection * movementSpeed * Time.deltaTime;
        
        if (!inSpace)
        {
            rb.MovePosition(rb.position + velocity);

            if (grounded)
            {
                footstepTimer -= movementSpeed * 0.15f * Time.deltaTime;
                if (footstepTimer <= 0)
                {
                    audioManager.PlaySound("Footstep");
                    footstepTimer = 0.3f;
                }
            }

        }
        else
        {
            if(details.UseEnergy(velocity.magnitude)) rb.AddForce(velocity, ForceMode.VelocityChange);
        }

    }

    public void AdjustWeaponSpeed(float val)
    {
        if (!grounded) val = 0;

        weaponSpeed = Mathf.MoveTowards(weaponSpeed, val, Time.deltaTime * 2);
        SetAnimFloat("WalkSpeed", weaponSpeed);
    }

    void Look()
    {
        Vector2 look = lookAction.ReadValue<Vector2>() * Time.timeScale;
        float yChange = 0;
        float xChange = 0;

        if(!inSpace)
        {
            xChange = -look.y * lookSensitivity;
            verticalLook += xChange;
            verticalLook = Mathf.Clamp(verticalLook, -90, 90);
            

            yChange = look.x * lookSensitivity;

            transform.localEulerAngles += Vector3.up * yChange;
            cam.localEulerAngles = new Vector3(verticalLook, 0, 0);


        }
        else
        {
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

            yChange = (look.x * lookSensitivity) + corrector2;
            xChange = (-look.y * lookSensitivity) + corrector1;
            Vector3 rotation = new Vector3(xChange, yChange, -zAngle);

            Quaternion rot = Quaternion.Euler(rotation);

            if (!instant)
            {
                rb.MoveRotation(rb.rotation * rot);
            }
            else
            {
                // Occurs when going from gravity to space, prevents a flash from a strange rotation
                rb.transform.rotation = rb.transform.rotation * rot;
            }
        }

        weaponManager.Rotate(yChange);
        hud.Shake(yChange, xChange);
    }

    void Jump()
    {
        if (inSpace || (!grounded && (doubleJumped||!canDoubleJump)) || paused) return;

        float strength = jumpStrength;

        if (!grounded && !doubleJumped)
        {
            doubleJumped = true;
            strength *= 1.2f;
            details.UseEnergy(2);
        }

        AddForce(transform.up * strength);
    }

    void SummonAmmo()
    {
        if (!canSummon || summonCooldown > 0) return;

        summonCooldown = summonDelay;
        hud.SetSummonActive(true);
        hud.SetSummonPercent(0);

        if(Physics.Raycast(cam.position, cam.forward,out RaycastHit hit, 7, aimLayerMask))
        {
            float distance = Vector3.Distance(cam.position, hit.point);
            if (distance < 1)
            {
                SpawnAmmo(cam.position + transform.up * 0.25f);
            }
            else
            {
                SpawnAmmo(cam.position + cam.forward * (distance - 1));
            }
        }
        else
        {
            SpawnAmmo(cam.position + cam.forward * 5);
        }
    }

    void SpawnAmmo(Vector3 position)
    {
        SummonedAmmo newAmmo = Instantiate(summonedAmmo.gameObject, nearestSource != null ? nearestSource.transform : null).GetComponent<SummonedAmmo>();
        newAmmo.transform.position = position;
        newAmmo.transform.up = transform.up;
        newAmmo.StartSpawn();
    }

    protected override void CheckGrounded()
    {
        base.CheckGrounded();

        if (grounded) doubleJumped = false;
    }

    public void Activate()
    {
        cam.localEulerAngles = Vector3.zero;
        verticalLook = 0;
        transform.parent.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        transform.parent.gameObject.SetActive(false);
        transform.localEulerAngles = Vector3.zero;
        cam.localEulerAngles = Vector3.zero;

        SetCanSave(true);
    }

    void UseWeapon()
    {
        if (weapon == null) return;

        weapon.PrimaryAction(weaponAction.ReadValue<float>());
        weapon.SecondaryAction(weaponSecondaryAction.ReadValue<float>());

        hud.SetAmmoText(weapon.GetAmmoText());
    }

    public void EquipWeapon(int index)
    {
        if (weaponManager.IsLocked(index)) return;

        SwapWeapon(weaponManager.GetWeapon(index));
    }

    public void UnlockWeapon(int index)
    {
        SaveManager.UnlockWeapon(index);
        weaponManager.UnlockWeapon(index);
        audioManager.PlaySound("equip");

        HUD.ActivateAmmoIndicator();
    }

    void SwapWeapon(Weapon newWeapon)
    {
        if (newWeapon == weapon) return;

        if (weapon != null) weapon.OnUnEquip();

        weapon = newWeapon;

        if (weapon != null) {
            weapon.OnEquip(this);
            hud.SetAmmoInfinite(weapon.IsInfinite());
            hud.SetAmmoText(weapon.GetAmmoText());
        }
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

    public override Vector3 GetAimDirection(Transform fireHole)
    {
        Vector3 newDirection = fireHole.forward;

        Vector3 origin = cam.position + cam.forward * 0.8f;
        if(Physics.Raycast(origin, cam.forward,out RaycastHit hit, 40, aimLayerMask))
        {
            newDirection = (hit.point - fireHole.position).normalized;
        }

        return newDirection;
    }

    void HandleWeaponWheel()
    {
        if(weaponWheelAction.ReadValue<float>() > 0)
        {
            hud.SetWeaponWheelActive(1);
        }
        else
        {
            hud.SetWeaponWheelActive(0);
            float scroll = weaponScrollAction.ReadValue<float>();
            if (scroll != 0) EquipWeapon(weaponManager.Scroll(scroll));
        }
    }

    public static void SetPaused(bool val)
    {
        if (Instance == null) return;
        Instance.paused = val;
    }

    public static bool IsPaused()
    {
        if(Instance == null) return false;
        return Instance.paused;
    }

    public bool Saveable()
    {
        return canSave && details.GetHealth() > 0 && grounded && nearestSource != null && transform.parent.gameObject.activeInHierarchy && nearestSource.Key != "ship_main";
    }

    public Vector3 GetLocalRotation()
    {
        return transform.localEulerAngles;
    }

    public Vector3 GetParentRotation()
    {
        return transform.parent.localEulerAngles;
    }

    public void SetAllRotation(Vector3 localRot, Vector3 parentRot)
    {
        transform.localEulerAngles = localRot;
        transform.parent.localEulerAngles = parentRot;
    }

    public PlayerDetails GetDetails()
    {
        return details;
    }

    public void SetCanSave(bool val)
    {
        canSave = val;
    }

    public static bool GetGrounded()
    {
        if (Instance == null) return false;

        return Instance.grounded;
    }

    public static bool IsPlayerActive()
    {
        if (Instance == null) return false;

        return Instance.transform.parent.gameObject.activeInHierarchy;
    }

    public override bool IsGrounded()
    {
        bool contact = Physics.BoxCast(transform.position, new Vector3(0.3f, 0.05f, 0.3f), -transform.up, out RaycastHit hit, transform.rotation, 1);
        if (contact && !walkOnLava && hit.collider.CompareTag("Lava")) LavaDamage();
        return contact;
    }

    void LavaDamage()
    {
        details.TakeDamage(20 * Time.fixedDeltaTime);
        details.UseEnergy(10 * Time.fixedDeltaTime);
    }

    public void UpdateAmmoText()
    {
        if (weapon == null) return;

        hud.SetAmmoText(weapon.GetAmmoText());
    }

    public bool AddAmmo(float percentOfMax)
    {
        return weaponManager.AddAmmo(percentOfMax);
    }

    public WeaponManager WeaponManager()
    {
        return weaponManager;
    }

    public int GetWeaponIndex()
    {
        return weaponManager.GetWeaponIndex(weapon);
    }

    void LoadWeapon()
    {
        int index = SaveManager.CurrentWeapon();
        weaponManager.ForceLoad();
        EquipWeapon(index);
    }

}

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

    float originalWalk;
    float originalSprint;
    float fuel;
    float maxFuel = 200;
    float weaponSpeed = 0;
    float evaSpeed;
    float summonCooldown = 0;
    float footstepTimer;
    const float summonDelay = 150;

    bool inSpace = false;
    bool doubleJumped = false;
    bool canSave = true;
    bool walkOnLava = true;
    bool canDoubleJump = true;
    bool canSummon = false;
    bool paused = false;
    bool loadBigGun = false;

    int aimLayerMask = ~((1 << 6) | (1 << 2) | (1 << 11) | (1 << 12) | (1 << 13));

    protected override void Awake()
    {
        base.Awake();

        // Store the intended original movement speeds
        originalWalk = walkSpeed;
        originalSprint = sprintSpeed;

        Instance = this;
        details = GetComponentInParent<PlayerDetails>();
        weaponManager = GetComponentInChildren<WeaponManager>();
        audioManager = GetComponentInParent<AudioManager>();
    }

    protected void Start()
    {
        Application.targetFrameRate = 500;

        Cursor.lockState = CursorLockMode.Locked;                       // Lock the position of the cursor
        Cursor.visible = false;                                         // Make the cursor invisible

        InputController input = InputController.Instance;

        if(input != null)
        {
            // Find and store all of the relevant input actions
            // Done so that they don't need to be found every time they need to be accessed
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

        // Subscribe to input events
        InputController.Jump += Jump;
        InputController.Pause += Pause;
        InputController.SummonAmmo += SummonAmmo;

        fuel = maxFuel;
        evaSpeed = walkSpeed * 0.8f;

        if (!finalFight) {                                                                  // If not in the final fight of the game
            LoadData();                                                                     // Load data from the save file
            if (!SaveManager.GetBool("hint_initial")) StartCoroutine(InitialHints());       // Display basic movement hints
        }
        LoadWeapon();                                                                       // Switch to the weapon that was last used in the save file


        SettingsManager.OnChangesMade += LoadSensitivity;
        LoadSensitivity();

        SaveManager.OnUpgradeChanged += LoadUpgrades;
        LoadUpgrades();
        loadBigGun = true;
    }

    // Function to load transform data from the save file
    bool LoadData()
    {
        if (!SaveManager.SaveExists()) return false;
        
        Vector3 playerRelativePos = SaveManager.GetRelativePlayerPos();

        if (playerRelativePos == new Vector3(-450000, 0, 0)) return false;                                  // Return if no relative position was found

        string key = SaveManager.GetGravitySource();
        if (key == "null" | !GravityController.FindSource(key, out GravitySource source)) return false;     // Return if no nearest gravity source could be found

        SetPosition(playerRelativePos + source.transform.position);                                         // Move to the relative position specified in the save file
        SetAllRotation(SaveManager.save.GetLocalRot(), SaveManager.save.GetParentRot());                    // Apply the rotations found in the save file
        ForceVelocity(source.GetVelocity());                                                                // Match the velocity to that of the nearest gravity source

        return true;
    }

    // Function to load the look sensitivity from the settings
    void LoadSensitivity()
    {
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            int val = PlayerPrefs.GetInt("Sensitivity");
            lookSensitivity = 0.2f * ((float)val / 5);
        }
    }

    // Function to load in all of the sacrifices and upgrades
    void LoadUpgrades()
    {
        if (SaveManager.SacrificeMade("sacrifice_lava"))
        {
            walkOnLava = false;         // This means that the player will now take damage when walking on lava
        }

        if (SaveManager.SacrificeMade("sacrifice_speed"))
        {
            // Reduce the player's movement speed
            walkSpeed = originalWalk * 0.7f;
            sprintSpeed = originalSprint * 0.7f;
        }

        if (SaveManager.SacrificeMade("sacrifice_jump"))
        {
            canDoubleJump = false;      // Prevent the player from double jumping
        }

        if (SaveManager.SelfUpgraded("upgrade_teleport"))
        {
            canSummon = true;                       // Allow the player to summon ammunition
            HintManager.PlayHint("hint_summon");    // Tell the player which key summons ammo
        }

        if (SaveManager.SelfUpgraded("upgrade_gun"))
        {
            UnlockWeapon(2);                        // Unlock the big gun
            if (loadBigGun)                         // If it was just unlocked for the first time
            {
                EquipWeapon(2);                     // Equip the big gun
                loadBigGun = false;
            }
        }
    }

    // Function called when the player pressed the pause button
    void Pause()
    {
        PauseMenu.TogglePause();
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

        inSpace = nearestSource == null;        // Determine whether the player is in space

        Move();

        Look();

        HandleWeaponWheel();

        AmmoSummonCooldown();
    }

    // Function used to keep essential functions still looping when the player is piloting their spaceship
    public void KeepLooping()
    {
        details.KeepLooping();
        AmmoSummonCooldown();
    }

    void AmmoSummonCooldown()
    {
        if (canSummon && summonCooldown > 0)                                // If the player can summon ammo, but has done so too recently
        {
            summonCooldown -= Time.deltaTime;                               // Reduce the time until they can summon ammo again
            hud.SetSummonPercent(1 - (summonCooldown / summonDelay));       // Update this progress in the HUD
            if (summonCooldown <= 0) hud.SetSummonActive(false);            // If ammo can be summoned again, disable the cooldown icon in the HUD
        }
    }
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        UseWeapon();

        CheckPlanetUI();

        // Used to ensure that the player remains still when using physics, and that its the other objects that move around them
        AddForce(rb.velocity);
        rb.velocity = Vector3.zero;
    }

    void CheckPlanetUI()
    {
        if (inSpace)
        {
            Vector3 target = camController.UpdatePlanetHUD(rb);
            if(matchVeloAction.ReadValue<float>() > 0)                                                                  // If the player is holding the match velocity button
            {
                target = Vector3.MoveTowards(rb.velocity, target, Time.fixedDeltaTime * matchVelocitySpeed);            // Move the current velocity towards the target velocity
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

        float forward = movementActions[0].ReadValue<float>() - movementActions[2].ReadValue<float>();      // Read the input from the W and S keys
        float sideways = movementActions[1].ReadValue<float>() - movementActions[3].ReadValue<float>();     // Read the input from the A and D keys

        Vector3 moveDirection = forward * transform.forward + sideways * transform.right;                   // Work out the direction that the player wants to move

        if (!inSpace)
        {
            if (sprintAction.ReadValue<float>() > 0)                                                        // If the sprint key is held
            {
                movementSpeed = sprintSpeed;                                                                // Change to the sprint speed
                gunSpeed = 1;
            }
            else
            {
                movementSpeed = walkSpeed;                                                                  // Otherwise, use the walk speed
                gunSpeed = 0.5f;
            }
        }
        else
        {
            movementSpeed = evaSpeed;                                                                       // If in space, use the EVA speed
            float up = movementActions[6].ReadValue<float>() - movementActions[7].ReadValue<float>();       // Read the input from the Shift and Control keys
            moveDirection += up * transform.up;
        }

        if (moveDirection == Vector3.zero) { AdjustWeaponSpeed(0); return; }                                // If not moving, reduce the weapon shake

        AdjustWeaponSpeed(gunSpeed);

        Vector3 velocity = moveDirection * movementSpeed * Time.deltaTime;                                  // Work out the appropriate velocity
        
        if (!inSpace)                                                                                       // If not in space
        {
            rb.MovePosition(rb.position + velocity);                                                        // Move towards the desired position

            if (grounded)                                                                                   // If on the ground, play footstep sound effects
            {
                footstepTimer -= movementSpeed * 0.15f * Time.deltaTime;
                if (footstepTimer <= 0)
                {
                    audioManager.PlaySound("Footstep");
                    footstepTimer = 0.3f;
                }
            }

        }
        else                                                                                                // If in space, add the velocity as a change in velocity force
        {
            if(details.UseEnergy(velocity.magnitude)) rb.AddForce(velocity, ForceMode.VelocityChange);
        }

    }

    // Function to alter the weapon shake speed
    public void AdjustWeaponSpeed(float val)
    {
        if (!grounded) val = 0;

        weaponSpeed = Mathf.MoveTowards(weaponSpeed, val, Time.deltaTime * 2);
        SetAnimFloat("WalkSpeed", weaponSpeed);
    }

    void Look()
    {
        Vector2 look = lookAction.ReadValue<Vector2>() * Time.timeScale;        // Read the mouse input
        float yChange = 0;
        float xChange = 0;

        if(!inSpace)                                                            // If not in space
        {
            xChange = -look.y * lookSensitivity;
            verticalLook += xChange;
            verticalLook = Mathf.Clamp(verticalLook, -90, 90);
            

            yChange = look.x * lookSensitivity;

            transform.localEulerAngles += Vector3.up * yChange;                 // Use the mouse's horizontal input to rotate the controller on its y axis
            cam.localEulerAngles = Vector3.right * verticalLook;                // Use the mouse's vertical input to rotate the camera on its x axis


        }
        else                                                                    // If in space
        {
            verticalLook = 0;

            Vector3 camRot = cam.localEulerAngles;
            Vector3 localRot = transform.localEulerAngles;
            bool instant = false;
            float corrector1 = 0;
            float corrector2 = 0;
            if (camRot.x != 0 || localRot.y != 0)                                       // If the camera or controller have been rotated
            {
                corrector1 = camRot.x;
                cam.localEulerAngles = new Vector3(0, camRot.y, camRot.z);              // Remove any rotation on the camera's x axis

                corrector2 = localRot.y;
                transform.localEulerAngles = new Vector3(localRot.x, 0, localRot.z);    // Remove any rotation on the controller's y axis

                instant = true;
            }

            float zAngle = (movementActions[4].ReadValue<float>() - movementActions[5].ReadValue<float>()) * Time.deltaTime * 80;       // Read the input from the Q and E keys, responsible for rolling

            yChange = (look.x * lookSensitivity) + corrector2;                          // Set yChange to the mouse's horiontal input plus any rotation that was on the controller
            xChange = (-look.y * lookSensitivity) + corrector1;                         // Set xChange to the mouse's vertical input plus any rotation that was on the camera
            Vector3 rotation = new Vector3(xChange, yChange, -zAngle);

            Quaternion rot = Quaternion.Euler(rotation);                                // Find the rotation as a quaternion

            if (!instant)
            {
                rb.MoveRotation(rb.rotation * rot);                                     // Move towards the desired rotation
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
        if (inSpace || (!grounded && (doubleJumped||!canDoubleJump)) || paused) return;     // Return if the player cannot jump

        float strength = jumpStrength;

        if (!grounded && !doubleJumped)         // If the player is double jumping
        {
            doubleJumped = true;
            strength *= 1.2f;
            details.UseEnergy(2);
            audioManager.PlaySound("jump");
        }

        AddForce(transform.up * strength);      // Add a jump force
    }

    void SummonAmmo()
    {
        if (!canSummon || summonCooldown > 0) return;

        summonCooldown = summonDelay;
        hud.SetSummonActive(true);
        hud.SetSummonPercent(0);

        if(Physics.Raycast(cam.position, cam.forward,out RaycastHit hit, 7, aimLayerMask))      // Attempt to find a point to spawn the ammo at
        {
            float distance = Vector3.Distance(cam.position, hit.point);
            if (distance < 1)                                                                   // If the point is too close
            {
                SpawnAmmo(cam.position + transform.up * 0.25f);                                 // Spawn the ammo above the camera
            }
            else
            {
                SpawnAmmo(cam.position + cam.forward * (distance - 1));                         // Spawn the ammo one metre closer to the player from the point to avoid potential clipping
            }
        }
        else
        {
            SpawnAmmo(cam.position + cam.forward * 5);                                          // Spawn the ammo five metres away from the player if no object was hit
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

        if (grounded) doubleJumped = false;     // Reset double jumping when contact is made with the ground
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

    // Function to use a weapon
    void UseWeapon()
    {
        if (weapon == null || paused) return;                                   // Return if no weapon is equipped or the game is paused

        weapon.PrimaryAction(weaponAction.ReadValue<float>());                  // Read the left click input and send that to the weapon
        weapon.SecondaryAction(weaponSecondaryAction.ReadValue<float>());       // Read the right click input and send that to the weapon

        hud.SetAmmoText(weapon.GetAmmoText());                                  // Update the ammo display text in the HUD
    }

    // Function to equip a weapon
    public void EquipWeapon(int index)
    {
        if (weaponManager.IsLocked(index)) return;      // Return if the weapon hasn't been unlocked

        SwapWeapon(weaponManager.GetWeapon(index));
    }

    // Function to unlock a weapon
    public void UnlockWeapon(int index)
    {
        SaveManager.UnlockWeapon(index);                    // Tell the save file that the weapon has been unlocked
        weaponManager.UnlockWeapon(index);                  // Unlock the weapon
        audioManager.PlaySound("equip");

        HUD.ActivateAmmoIndicator();
        HintManager.PlayHint("hint_fire");                  // Tell the user how to use the weapon

        Debug.Log("WEAPON COUNT: " + weaponManager.NumberUnlocked());

        if(weaponManager.NumberUnlocked() == 2)             // If this is the second weapon to be unlocked
        {
            HintManager.PlayHint("hint_wheel");             // Tell the player how to change weapons
        }
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

    // Function to add a force to the player
    // Doesn't actually add any forces, just tells the gravity controller what velocity the player should be at
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

    // Function to change the aim direction of a ranged weapon
    public override Vector3 GetAimDirection(Transform fireHole)
    {
        Vector3 newDirection = fireHole.forward;

        Vector3 origin = cam.position + cam.forward * 0.8f;
        if(Physics.Raycast(origin, cam.forward,out RaycastHit hit, 40, aimLayerMask))       // Find out if there are any objects that the cursor is in front of
        {
            newDirection = (hit.point - fireHole.position).normalized;                      // Aim the gun at the found collision point
        }

        return newDirection;
    }

    // Function to display the weapon wheel
    void HandleWeaponWheel()
    {
        if(weaponWheelAction.ReadValue<float>() > 0)                        // If Tab is held down
        {
            hud.SetWeaponWheelActive(1);                                    // Display the weapon wheel
        }
        else
        {
            hud.SetWeaponWheelActive(0);                                    // Otherwise, hide it
            float scroll = weaponScrollAction.ReadValue<float>();           // Read the mouse scroll wheel input
            if (scroll != 0) EquipWeapon(weaponManager.Scroll(scroll));     // Change the equipped weapon if being scrolled
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

    // Determine whether the game can be saved
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

    // Poorly named function, used to tell the player that they are in a combat area and thus cannot save
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
        if (contact && !walkOnLava && hit.collider.CompareTag("Lava")) LavaDamage();                // Determine whether the player is on lava, take damage if this is the case
        return contact;
    }

    // Function to handle taking damage from lava
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
        bool success = weaponManager.AddAmmo(percentOfMax);
        if (success)
        {
            audioManager.PlaySound("increaseAmmo");
        }

        return success;
    }

    public WeaponManager WeaponManager()
    {
        return weaponManager;
    }

    public int GetWeaponIndex()
    {
        return weaponManager.GetWeaponIndex(weapon);
    }

    // Function to load in the currently equipped weapon from the save file
    void LoadWeapon()
    {
        int index = SaveManager.CurrentWeapon();
        weaponManager.ForceLoad();
        EquipWeapon(index);
    }

    public void PlaySound(string sound)
    {
        audioManager.PlaySound(sound);
    }

    // Function to show the intial control hints once the player has touched the ground
    IEnumerator InitialHints()
    {
        while (!grounded)
        {
            yield return new WaitForEndOfFrame();
        }

        HintManager.PlayHint("hint_initial");
        HintManager.PlayHint("hint_sprint");
        HintManager.PlayHint("hint_jump");
        HintManager.PlayHint("hint_pause");
    }

    public bool IsDead()
    {
        return details.GetHealth() <= 0;
    }
}

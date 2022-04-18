using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] float maxOffset;
    [SerializeField] float snappiness;
    [SerializeField] float returnSpeed;

    [Header("References")]
    [SerializeField] Camera cam;
    [SerializeField] HealthBar healthBar;
    [SerializeField] HealthBar energyBar;
    [SerializeField] HealthBar shieldBar;
    [SerializeField] HealthBar ammoSummonBar;
    [SerializeField] TextMeshProUGUI powerCells;
    [SerializeField] WeaponWheel weaponWheel;
    [SerializeField] TextMeshProUGUI interactText;
    [SerializeField] TextMeshProUGUI planetText;
    [SerializeField] TextMeshProUGUI velocityText;
    [SerializeField] GameObject planetTextHolder;
    [SerializeField] DamageIndicatorController damageIndicator;
    [SerializeField] SaveSymbol saveSymbol;
    [SerializeField] GameObject ammoIndicator;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] Image ammoInfinite;
    [SerializeField] RectTransform objectiveMarker;
    [SerializeField] TextMeshProUGUI objectiveText;
    [SerializeField] Transform[] objectiveTargets;

    public static HUD Instance;

    Vector3 currentPos;
    Vector3 targetPos;

    bool damageIndicatorsActive = true;
    bool infinite = false;

    int objectiveTarget;

    private void Awake()
    {
        weaponWheel.gameObject.SetActive(true);
        Instance = this;
    }

    private void Start()
    {
        SaveManager.OnUpgradeChanged += LoadSacrifice;
        LoadSacrifice();
    }

    private void OnDestroy()
    {
        SaveManager.OnUpgradeChanged -= LoadSacrifice;
    }

    void LoadSacrifice()
    {
        if (SaveManager.SacrificeMade("sacrifice_indicators"))
        {
            damageIndicatorsActive = false;
        }
    }

    public void SetHealthPercent(float percent)
    {
        healthBar.SetPercent(percent);
    }

    public void SetEnergyPercent(float percent)
    {
        energyBar.SetPercent(percent);
    }

    public void SetShieldPercent(float percent)
    {
        shieldBar.SetPercent(percent);
    }

    public void SetSummonPercent(float percent)
    {
        ammoSummonBar.SetPercent(percent);
    }

    public void SetSummonActive(bool active)
    {
        ammoSummonBar.gameObject.SetActive(active);
    }

    public void SetWeaponWheelActive(float val)
    {
        weaponWheel.Activate(val);
    }

    public static void ActivateAmmoIndicator()
    {
        if (Instance == null) return;
        Instance.ammoIndicator.SetActive(true);
    }

    public void SetAmmoText(string text)
    {
        if (infinite) return;
        ammoText.text = text;
    }

    public void SetAmmoInfinite(bool infinite)
    {
        ammoText.enabled = !infinite;
        ammoInfinite.enabled = infinite;

        this.infinite = infinite;
    }

    public void Shake(float yChange, float xChange)
    {
        targetPos.x -= yChange;
        targetPos.y += xChange;
    }

    private void LateUpdate()
    {
        targetPos = Vector3.Lerp(targetPos, Vector3.zero, returnSpeed * Time.deltaTime);
        currentPos = Vector3.Lerp(currentPos, targetPos, snappiness * Time.deltaTime);
        transform.localPosition = currentPos;

        UpdateObjectiveMarker();
    }

    public static void SetInteractText(string text)
    {
        if (Instance == null) return;

        Instance.interactText.gameObject.SetActive(true);
        Instance.interactText.text = "[F] " + text;
    }

    public static void ClearInteractText()
    {
        if (Instance == null) return;

        Instance.interactText.gameObject.SetActive(false);
    }

    public void SetNumberOfPowerCells(int value)
    {
        powerCells.text = value.ToString();
    }

    public static void SetPlanetDetails(string name, float relativeVel, float distance)
    {
        if (Instance == null) return;

        SetPlanetTextActive(true);

        Instance.planetText.text = name;


        string subText = DistanceText(distance);

        subText += '\n';
        subText += relativeVel.ToString("F0") + "m/s";

        Instance.velocityText.text = subText;
    }

    public static string DistanceText(float distance)
    {
        if (distance > 1000) return (distance / 1000).ToString("F1") + "km";
        else return distance.ToString("F0") + "m";
    }

    public static void SetPlanetTextActive(bool val)
    {
        if (Instance == null) return;

        Instance.planetText.enabled = val;
        Instance.velocityText.enabled = val;
    }

    public static void SetPlanetTextHolderActive(bool val)
    {
        if (Instance == null) return;

        Instance.planetTextHolder.SetActive(val);
    }

    public static void AddDamageIndicator(Transform target)
    {
        if (Instance == null || !Instance.damageIndicatorsActive) return;

        Instance.damageIndicator.AddIndicator(target);
    }

    public static void SetActive(bool val)
    {
        if (Instance == null) return;

        Instance.gameObject.SetActive(val);
    }

    public static void SpinSaveIcon(bool instant)
    {
        if (Instance == null) return;

        Instance.saveSymbol.SpinSaveIcon(instant);
    }

    public static void StopSaving()
    {
        if (Instance == null) return;

        Instance.saveSymbol.StopSaving();
    }

    public static void SetReducedMaxHealth()
    {
        if (Instance == null) return;

        Instance.healthBar.ReduceMax();
    }

    public static void SetReducedMaxEnergy()
    {
        if (Instance == null) return;

        Instance.energyBar.ReduceMax();
    }

    public static void DisableObjectiveMarker(int index)
    {
        if (Instance == null) return;

        if(index == Instance.objectiveTarget || index == -1) {
            Instance.objectiveMarker.gameObject.SetActive(false);
        }
    }

    public static void ChangeObjectiveTarget(int index)
    {
        if (Instance == null) return;
        if (index < Instance.objectiveTargets.Length) {
            Instance.objectiveMarker.gameObject.SetActive(true);
            Instance.objectiveTarget = index;
        }
    }

    void UpdateObjectiveMarker()
    {
        if (objectiveMarker == null || !objectiveMarker.gameObject.activeInHierarchy) return;

        Vector3 screenPos = cam.WorldToScreenPoint(objectiveTargets[objectiveTarget].position);
        float width = Screen.currentResolution.width;
        float height = Screen.currentResolution.height;


        if (Vector3.Dot(objectiveTargets[objectiveTarget].position - cam.transform.position, cam.transform.forward) < 0)
        {
            screenPos *= -1;
        }
        screenPos.x = Mathf.Clamp(screenPos.x, 0.1f * width, 0.9f * width);
        screenPos.y = Mathf.Clamp(screenPos.y, 0.1f * height, 0.9f * height);
        screenPos.z = 0;

        float distance = Vector3.Distance(objectiveTargets[objectiveTarget].position, cam.transform.position);
        objectiveText.text = DistanceText(distance);

        objectiveMarker.transform.position = screenPos;
    }
}

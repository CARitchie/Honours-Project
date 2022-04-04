using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] float maxOffset;
    [SerializeField] float snappiness;
    [SerializeField] float returnSpeed;

    [Header("References")]
    [SerializeField] HealthBar healthBar;
    [SerializeField] HealthBar energyBar;
    [SerializeField] TextMeshProUGUI powerCells;
    [SerializeField] WeaponWheel weaponWheel;
    [SerializeField] TextMeshProUGUI interactText;
    [SerializeField] TextMeshProUGUI planetText;
    [SerializeField] TextMeshProUGUI velocityText;
    [SerializeField] GameObject planetTextHolder;
    [SerializeField] DamageIndicatorController damageIndicator;
    [SerializeField] SaveSymbol saveSymbol;

    public static HUD Instance;

    Vector3 currentPos;
    Vector3 targetPos;

    private void Awake()
    {
        weaponWheel.gameObject.SetActive(true);
        Instance = this;
    }

    public void SetHealthPercent(float percent)
    {
        healthBar.SetPercent(percent);
    }

    public void SetEnergyPercent(float percent)
    {
        energyBar.SetPercent(percent);
    }

    public void SetWeaponWheelActive(float val)
    {
        weaponWheel.Activate(val);
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

        string subText = "";
        if (distance > 1000) subText += (distance / 1000).ToString("F1") + "km";
        else subText += distance.ToString("F0") + "m";

        subText += '\n';
        subText += relativeVel.ToString("F0") + "m/s";

        Instance.velocityText.text = subText;
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
        if (Instance == null) return;

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
}

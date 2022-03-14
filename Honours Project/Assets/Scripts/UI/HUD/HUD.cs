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
    [SerializeField] WeaponWheel weaponWheel;
    [SerializeField] TextMeshProUGUI interactText;

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
}

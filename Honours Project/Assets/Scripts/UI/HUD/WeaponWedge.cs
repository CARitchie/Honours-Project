using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWedge : MonoBehaviour
{
    [SerializeField] Color defaultColour;
    [SerializeField] Color highlightColour;
    [SerializeField] GameObject weaponImage;
    [SerializeField] GameObject padlockImage;
    [SerializeField] int index;

    Image background;
    AudioManager audioManager;

    private void Start()
    {
        WeaponManager.OnWeaponUnlock += Unlock;
    }

    private void OnDestroy()
    {
        WeaponManager.OnWeaponUnlock -= Unlock;
    }

    private void Awake()
    {
        background = GetComponentInChildren<Image>();
        audioManager = GetComponentInParent<AudioManager>();
    }


    public void Select()
    {
        StopAllCoroutines();
        StartCoroutine(FadeToColour(highlightColour));
        audioManager.PlaySound("WeaponHover");
    }

    public void Deselect()
    {
        StopAllCoroutines();
        StartCoroutine(FadeToColour(defaultColour));
    }

    IEnumerator FadeToColour(Color colour)
    {
        bool equal = false;
        while (!equal)
        {
            float speed = Time.unscaledDeltaTime;

            float alpha = background.color.a;
            float r = Mathf.MoveTowards(background.color.r, colour.r, speed);
            float g = Mathf.MoveTowards(background.color.g, colour.g, speed);
            float b = Mathf.MoveTowards(background.color.b, colour.b, speed);

            Color newColour = new Color(r, g, b, alpha);
            background.color = newColour;

            equal = (r == colour.r) && (g == colour.g) && (b == colour.b);

            yield return new WaitForEndOfFrame();
        }
    }

    public void Equipped()
    {
        PlayerController.Instance.EquipWeapon(index);
    }

    public void Unlock(int index)
    {
        if (index != this.index) return;
        weaponImage.SetActive(true);
        padlockImage.SetActive(false);
    }
}

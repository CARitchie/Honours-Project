using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWedge : MonoBehaviour
{
    [SerializeField] Color defaultColour;
    [SerializeField] Color highlightColour;
    [SerializeField] int index;

    Image background;
    AudioManager audio;

    private void Awake()
    {
        background = GetComponentInChildren<Image>();
        audio = GetComponentInParent<AudioManager>();
    }


    public void Select()
    {
        StopAllCoroutines();
        StartCoroutine(FadeToColour(highlightColour));
        audio.PlaySound("WeaponHover");
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
}

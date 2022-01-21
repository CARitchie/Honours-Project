using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassItem : MonoBehaviour
{
    [SerializeField] RectTransform itemIcon;

    RectTransform icon;

    private void Start()
    {
        Compass.AddItem(this);
    }

    public Image CreateNewIcon(Transform parent)
    {
        if (icon != null) Destroy(icon.gameObject);

        icon = Instantiate(itemIcon, parent);

        icon.localPosition = Vector3.zero;

        return icon.GetComponent<Image>();
    }

    public void SetPosition(float x)
    {
        Vector3 pos = icon.localPosition;
        pos.x = x;
        icon.localPosition = pos;
    }

    public void DestroyIcon()
    {
        if (icon != null) Destroy(icon.gameObject);
    }

    private void OnDestroy()
    {
        Compass.RemoveItem(this);
    }
}

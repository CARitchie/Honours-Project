using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassItem : MonoBehaviour
{
    [SerializeField] bool activeOnStart = true;
    [SerializeField] bool alwaysActive = false;
    [SerializeField] RectTransform itemIcon;

    RectTransform icon;
    [HideInInspector] public bool active = true;

    private void Start()
    {
        if(activeOnStart) Compass.AddItem(this);
    }

    public Image CreateNewIcon(Transform parent)
    {
        if (icon != null) Destroy(icon.gameObject);

        icon = Instantiate(itemIcon, parent);           // Create a copy of the icon

        icon.localPosition = Vector3.zero;

        return icon.GetComponent<Image>();
    }

    public Image GetIcon()
    {
        Image image = null;
        if (icon!= null)
        {
            image = icon.GetComponent<Image>();
        }
        return image;
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

    // Determine whether the specified transform is a parent of this transform
    public bool SameParent(Transform parent)
    {
        Transform temp = transform.parent;
        while(temp != null)
        {
            if (temp == parent) return true;
            temp = temp.parent;
        }

        return false;
    }

    public void SetIconActive(bool val)
    {
        if (!val && alwaysActive) return;

        if(icon != null)
        {
            icon.gameObject.SetActive(val);
        }

        active = val;
    }
}

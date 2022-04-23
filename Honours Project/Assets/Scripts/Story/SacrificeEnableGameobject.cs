using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class used to enable or disable gameobjects depending upon the state of a sacrifice
public class SacrificeEnableGameobject : MonoBehaviour
{
    [SerializeField] string sacrificeKey;
    [SerializeField] GameObject[] defaultObjects;
    [SerializeField] GameObject[] sacrificedObjects;

    // Start is called before the first frame update
    void Start()
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
        if (SaveManager.SacrificeMade(sacrificeKey))
        {
            SetSacrificedObjects(true);     // Enable sacrifice objects
            SetDefaultObjects(false);       // Disable default objects
        }
        else
        {
            SetSacrificedObjects(false);    // Disable sacrifice objects
            SetDefaultObjects(true);        // Enable default objects
        }
    }

    void SetSacrificedObjects(bool active)
    {
        if (sacrificedObjects == null || sacrificedObjects.Length < 1) return;
        for (int i = 0; i < sacrificedObjects.Length; i++)
        {
            sacrificedObjects[i].SetActive(active);
        }
    }

    void SetDefaultObjects(bool active)
    {
        if (defaultObjects == null || defaultObjects.Length < 1) return;
        for (int i = 0; i < defaultObjects.Length; i++)
        {
            defaultObjects[i].SetActive(active);
        }
    }
}

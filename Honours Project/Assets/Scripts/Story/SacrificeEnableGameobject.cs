using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            SetSacrificedObjects(true);
            SetDefaultObjects(false);
        }
        else
        {
            SetSacrificedObjects(false);
            SetDefaultObjects(true);
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

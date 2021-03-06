using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class used to enable or disable gameobjects depending upon the state of an upgrade
public class UpgradeEnableGameobject : MonoBehaviour
{
    [SerializeField] string upgradeKey;
    [SerializeField] bool sacrificed;
    [SerializeField] GameObject[] defaultObjects;
    [SerializeField] GameObject[] upgradedObjects;

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
        bool upgraded = sacrificed ? SaveManager.SacrificeMade(upgradeKey) : SaveManager.SelfUpgraded(upgradeKey);  // Determine whether the upgrade objects should be enabled when the upgrade is chosen for self or chosen for colony ship

        if (upgraded)
        {
            SetUpgradedObjects(true);
            SetDefaultObjects(false);
        }
        else
        {
            SetUpgradedObjects(false);
            SetDefaultObjects(true);
        }
    }

    void SetUpgradedObjects(bool active)
    {
        if (upgradedObjects == null || upgradedObjects.Length < 1) return;
        for (int i = 0; i < upgradedObjects.Length; i++)
        {
            upgradedObjects[i].SetActive(active);
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

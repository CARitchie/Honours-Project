using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeItem : MonoBehaviour, Interact
{
    [SerializeField] string upgradeKey;

    private void Start()
    {
        if ((int)SaveManager.GetUpgradeState(upgradeKey) > 0)       // Destroy the gameobject if it has already been collected
        {
            Destroy(gameObject);
        }
    }

    public void OnEnter()
    {
        HUD.SetInteractText("Take");
    }

    public void OnExit()
    {
        HUD.ClearInteractText();
    }

    public void OnSelect()
    {
        SaveManager.SetUpgradeState(upgradeKey, SaveFile.UpgradeState.Available);       // Make the upgrade available in the upgrade/sacrifice menu
        PlayerController.Instance.PlaySound("collectUpgrade");
        HintManager.PlayHint("hint_upgrade", true);                                     // Tell the user that a new upgrade is available
        Useful.DestroyGameObject(gameObject);                                           // Destroy the gameobject
    }
}

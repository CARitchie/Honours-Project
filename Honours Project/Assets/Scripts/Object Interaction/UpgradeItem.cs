using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeItem : MonoBehaviour, Interact
{
    [SerializeField] string upgradeKey;

    private void Start()
    {
        if ((int)SaveManager.GetUpgradeState(upgradeKey) > 0)
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
        SaveManager.SetUpgradeState(upgradeKey, SaveFile.UpgradeState.Available);
        PlayerController.Instance.PlaySound("collectUpgrade");
        Useful.DestroyGameObject(gameObject);
    }
}

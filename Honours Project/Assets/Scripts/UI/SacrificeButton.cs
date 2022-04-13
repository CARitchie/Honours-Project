using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SacrificeButton : MonoBehaviour
{
    [SerializeField] string key;
    [SerializeField] bool upgrade;
    [SerializeField] Image padlock;
    [SerializeField] Image background;
    [SerializeField] string sacrificeText;
    [SerializeField] string upgradeText;
    [SerializeField] GameObject viewButton;
    [SerializeField] GameObject titleText;
    [SerializeField] Sprite unlockedImage;

    SacrificeMenu sacrificeMenu;
    SaveFile.UpgradeState currentState = SaveFile.UpgradeState.NonExistent;

    public string SacrificeText { get { return sacrificeText.Replace("\\n", "\n"); } }
    public string UpgradeText { get { return upgradeText.Replace("\\n", "\n"); } }

    private void Awake()
    {
        sacrificeMenu = GetComponentInParent<SacrificeMenu>();
    }

    private void OnEnable()
    {
        UpdateState();
    }

    public void UpdateState()
    {
        currentState = SaveManager.GetUpgradeState(key);
        switch (currentState)
        {
            case SaveFile.UpgradeState.NonExistent:
                SetNotAvailable();
                break;
            case SaveFile.UpgradeState.NotAvailable:
                SetNotAvailable();
                break;
            case SaveFile.UpgradeState.Available:
                SetAvailable();
                break;
            case SaveFile.UpgradeState.PlayerUnlocked:
                SetPlayerUnlocked();
                break;
            case SaveFile.UpgradeState.Sacrificed:
                SetSacrificed();
                break;
            case SaveFile.UpgradeState.Lost:
                SetNotAvailable();
                background.color = Color.black;
                break;
        }
    }

    void SetNotAvailable()
    {
        padlock.gameObject.SetActive(true);
        background.color = Color.gray;

        if (!upgrade)
        {
            viewButton.SetActive(false);
            titleText.SetActive(false);
        }
    }

    void SetAvailable()
    {
        UnlockBasics();
        background.color = sacrificeMenu.AvailableColour;
    }

    void SetPlayerUnlocked()
    {
        UnlockBasics();
        background.color = sacrificeMenu.UnlockedColour;
    }

    void SetSacrificed()
    {
        UnlockBasics();
        background.color = sacrificeMenu.SacrificedColour;
    }

    void UnlockBasics()
    {
        if (!upgrade)
        {
            viewButton.SetActive(true);
            titleText.SetActive(true);
            padlock.gameObject.SetActive(false);
        }
        else
        {
            padlock.gameObject.SetActive(true);
            padlock.sprite = unlockedImage;
        }
    }

    public void OpenSubMenu()
    {
        sacrificeMenu.OpenSpecificSacrifice(this, upgrade);
    }

    public void UpgradeThis()
    {
        ChangeState(SaveFile.UpgradeState.PlayerUnlocked);
    }

    public void SacrificeThis()
    {
        ChangeState(SaveFile.UpgradeState.Sacrificed);
    }

    public void SetNewState(int numberOfFoundPods, int index)
    {
        if (currentState == SaveFile.UpgradeState.NonExistent) UpdateState();
        if(index < numberOfFoundPods && (int)currentState < 5)
        {         
            if(index +2 < numberOfFoundPods)
            {
                // Lost
                ChangeState(SaveFile.UpgradeState.Lost);
            }
            else
            {
                // Available
                ChangeState(SaveFile.UpgradeState.Available);
            }
        }
    }

    void ChangeState(SaveFile.UpgradeState state)
    {
        SaveManager.SetUpgradeState(key, state);
        UpdateState();
    }

    public bool OptionsAvailable()
    {
        if (currentState == SaveFile.UpgradeState.NonExistent) UpdateState();
        return (int)currentState >= 0 && (int)currentState < 5; 
    }
}

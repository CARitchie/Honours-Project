using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SacrificeMenu : MonoBehaviour
{
    [SerializeField] Color playerUnlockedColour;
    [SerializeField] Color sacrificedColour;
    [SerializeField] Color availableColour;
    [SerializeField] GameObject mainSacrificeMenu;
    [SerializeField] GameObject subMenu;
    [SerializeField] GameObject sacrificeMenu;
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] RectTransform indicator;
    [SerializeField] TextMeshProUGUI sacrificeText;
    [SerializeField] TextMeshProUGUI upgradeText;
    [SerializeField] TextMeshProUGUI sacrificeButton;
    [SerializeField] TextMeshProUGUI upgradeButton;
    [SerializeField] GameObject[] subMenuButtons;
    [SerializeField] SacrificeButton[] sacrificeBtns;

    public Color UnlockedColour { get { return playerUnlockedColour; } }
    public Color SacrificedColour { get { return sacrificedColour; } }
    public Color AvailableColour { get { return availableColour; } }

    SacrificeButton currentSacrifice;

    public void OpenSpecificSacrifice(SacrificeButton btn, bool upgrade)
    {
        currentSacrifice = btn;
        if (upgrade)
        {
            sacrificeButton.text = "Upgrade Ship";
            upgradeButton.text = "Upgrade Self";
        }
        else
        {
            sacrificeButton.text = "Sacrifice";
            upgradeButton.text = "Keep";
        }

        sacrificeText.text = currentSacrifice.SacrificeText;
        upgradeText.text = currentSacrifice.UpgradeText;
        subMenu.SetActive(true);
        mainSacrificeMenu.SetActive(false);

        if (currentSacrifice.OptionsAvailable())
        {
            for(int i = 0; i < subMenuButtons.Length; i++)
            {
                subMenuButtons[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < subMenuButtons.Length; i++)
            {
                subMenuButtons[i].SetActive(false);
            }
        }
    }

    public void ChooseUpgrade()
    {
        if (currentSacrifice == null) return;
        currentSacrifice.UpgradeThis();
        CloseSubMenu();
    }

    public void ChooseSacrifice()
    {
        if (currentSacrifice == null) return;
        currentSacrifice.SacrificeThis();
        CloseSubMenu();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        subMenu.SetActive(false);
        mainSacrificeMenu.SetActive(true);
        CheckSacrificeBtns();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        DialogueManager.PlayDialogue("audio_depart");
    }

    public void CloseSubMenu()
    {
        subMenu.SetActive(false);
        mainSacrificeMenu.SetActive(true);
    }

    public void Close()
    {
        PauseMenu.Instance.Resume();
    }

    void CheckSacrificeBtns()
    {
        int foundPods = SaveManager.NumberOfFoundPods();
        for(int i = 0; i < sacrificeBtns.Length; i++)
        {
            sacrificeBtns[i].SetNewState(foundPods, i);
        }
    }

    public void OpenSacrifices()
    {
        sacrificeMenu.SetActive(true);
        upgradeMenu.SetActive(false);
        Vector3 pos = indicator.localPosition;
        pos.x = -325;
        indicator.localPosition = pos;
    }

    public void OpenUpgrades()
    {
        sacrificeMenu.SetActive(false);
        upgradeMenu.SetActive(true);
        Vector3 pos = indicator.localPosition;
        pos.x = 325;
        indicator.localPosition = pos;
    }
}

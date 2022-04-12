using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalButton : MonoBehaviour, Interact
{
    bool pressed = false;

    public void OnEnter()
    {
        HUD.SetInteractText("Start Engines. There is no turning back");
    }

    public void OnExit()
    {
        HUD.ClearInteractText();
    }

    public void OnSelect()
    {
        PlayerController.SetPaused(true);
        OnExit();
        // TODO: Need to temporarily save some player data
        SceneManager.FadeToScene("StoryEnding");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalButton : MonoBehaviour, Interact
{
    bool pressed = false;

    public void OnEnter()
    {
        HUD.SetInteractText("Start Engines. There is no turning back");     // Warn the player that this is the ending
    }

    public void OnExit()
    {
        HUD.ClearInteractText();
    }

    public void OnSelect()
    {
        PlayerController.SetPaused(true);           // Prevent the player from performing any actions
        OnExit();
        SceneManager.FadeToScene("StoryEnding");    // Start the end of the game
    }
}

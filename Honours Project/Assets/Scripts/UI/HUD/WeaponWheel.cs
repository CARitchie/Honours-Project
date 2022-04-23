using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class WeaponWheel : MonoBehaviour
{
    [SerializeField] float slowness;
    [SerializeField] float activeTime;
    [SerializeField] RectTransform arrow;
    [SerializeField] WeaponWedge[] wedges;

    bool displaying = false;
    float active = 0;
    MaskableGraphic[] graphics;
    InputAction lookAction;
    int lastIndex = 0;
    Vector2 lookDirection = Vector2.up;
    float lastVal = 0;
    bool goingUp = false;
    bool swapped = true;

    private void Awake()
    {
        graphics = GetComponentsInChildren<MaskableGraphic>(true);
        SetGraphicsAlpha(0);
    }

    private void Start()
    {
        InputController input = InputController.Instance;
        if(input != null) lookAction = input.FindAction("Look");
    }

    public void Activate(float val)
    {
        if (val < 0) return; // Change to be an immediate cancel

        float multiplier = val == 1 ? 1 : -1;
        active += Time.unscaledDeltaTime * multiplier;
        active = Mathf.Clamp(active, 0, activeTime);

        if (active >= lastVal && active != 0) goingUp = true;
        else goingUp = false;
        lastVal = active;

        Time.timeScale = slowness + (1 - (active / activeTime)) * (1 - slowness);       // Gradually slow down / speed up time
        PostProcessControl.SetDepth(0,10,active/activeTime);                            // Gradually increase / decrease the depth of field strength

        if(active > 0)
        {
            // Opened
            if (!displaying) {
                displaying = true;
            }

            Display();

        }
        else if (displaying) // Closed
        {
            displaying = false;
            SetGraphicsAlpha(0);
            
        }

        if (swapped && val > 0 && goingUp) swapped = false;
        if (!goingUp && !swapped) SwapWeapons();                // Swap to the selected weapon if the wheel is fading out and not already swapped
    }

    void SwapWeapons()
    {
        if (swapped) return;
        swapped = true;
        if (lastIndex < wedges.Length) wedges[lastIndex].Equipped();
    }

    void Display()
    {
        float percent = active / activeTime;
        if (!goingUp)
        {
            // Fade out between 1 and 0.5
            if (percent < 0.5f) percent = 0;
            else percent = (percent - 0.5f) * 2;
        }
        else
        {
            // Fade in between 0 and 0.5
            if (percent > 0.5f) percent = 1;
            else percent *= 2;
        }
        SetGraphicsAlpha(percent);

        if (!goingUp) return;

        lookDirection += lookAction.ReadValue<Vector2>();   // Read the mouse input

        lookDirection = lookDirection.normalized * 50;      // Restrict the magnitude of the lookdirection, makes rotating the arrow easier for the player

        arrow.up = lookDirection;

        int newIndex = GetIndex();
        if(newIndex != lastIndex)                                               // If a new wedge is selected
        {
            if (lastIndex < wedges.Length) wedges[lastIndex].Deselect();        // Deselect the old one
            if (newIndex < wedges.Length) wedges[newIndex].Select();            // Select the new one
            lastIndex = newIndex;
        }
    }

    void SetGraphicsAlpha(float alpha)
    {
        foreach(MaskableGraphic graphic in graphics)
        {
            Color colour = graphic.color;
            colour.a = alpha;
            graphic.color = colour;
        }
    }

    int GetIndex()
    {
        // Work out the index based on the arrow's rotation
        return (int)Mathf.Floor((arrow.localEulerAngles.z / 45 + 0.5f) % 8);
    }
}

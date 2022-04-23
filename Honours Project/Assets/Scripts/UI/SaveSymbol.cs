using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSymbol : MonoBehaviour
{
    [SerializeField] Image image;

    bool saving;

    // Start is called before the first frame update
    void Start()
    {
        Color col = image.color;
        col.a = 0;
        image.color = col;
    }

    public void SpinSaveIcon(bool instant)
    {
        StopAllCoroutines();
        saving = !instant;
        StartCoroutine(SpinningIcon());
    }

    IEnumerator SpinningIcon()
    {
        Color col = image.color;
        col.a = 1;
        image.color = col;

        image.rectTransform.localEulerAngles = Vector3.zero;

        // Spin the symbol while trying to save
        while (saving)
        {
            Vector3 rot = image.rectTransform.localEulerAngles;
            rot.z -= Time.unscaledDeltaTime * 30;
            image.rectTransform.localEulerAngles = rot;
            yield return new WaitForEndOfFrame();
        }

        float progress = 1.2f;
        while (progress > 0)
        {

            // Spin the save symbol
            Vector3 rot = image.rectTransform.localEulerAngles;
            rot.z -= Time.unscaledDeltaTime * 60;
            image.rectTransform.localEulerAngles = rot;

            if (progress < 1)
            {
                // Fade the symbol out after 0.2 seconds
                Color colour = image.color;
                colour.a = progress;
                image.color = colour;
            }

            progress -= Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }

        col = image.color;
        col.a = 0;
        image.color = col;
    }

    public void StopSaving()
    {
        saving = false;
    }
}

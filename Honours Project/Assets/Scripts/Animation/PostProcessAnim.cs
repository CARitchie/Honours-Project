using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[ExecuteInEditMode]
public class PostProcessAnim : MonoBehaviour
{
    [SerializeField] PostProcessVolume volume;
    [SerializeField] float lensIntensity;
    

    LensDistortion lens;

    // Update is called once per frame
    void Update()
    {
        CheckNull();                                // Ensure that the lens distortion has been found
        lens.intensity.value = lensIntensity;       // Change the intensity of the lens distortion
    }

    void CheckNull()
    {
        if (lens == null) lens = volume.profile.GetSetting<LensDistortion>();
    }
}

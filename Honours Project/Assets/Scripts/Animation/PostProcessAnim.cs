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
        CheckNull();
        lens.intensity.value = lensIntensity;
    }

    void CheckNull()
    {
        if (lens == null) lens = volume.profile.GetSetting<LensDistortion>();
    }
}

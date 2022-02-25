using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{
    static AudioControl Instance;

    [SerializeField] AudioMixer gameAudio;

    private void Awake()
    {
        Instance = this;
    }

    public static void AtmosphereInterpolation(float val)
    {
        if (Instance == null) return;
        Instance.gameAudio.SetFloat("Frequency", val * (1 - 0.05f) + 0.05f);
    }

}

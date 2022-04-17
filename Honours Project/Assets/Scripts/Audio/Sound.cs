using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Taken from submission for 3rd year Games Programming Module
[CreateAssetMenu(menuName = "My Assets/Audio/Sound")]
public class Sound : PlayableSound
{
    [Header("Settings")]
    [SerializeField] AudioClip clip;
    [SerializeField] AudioMixerGroup group;
    [SerializeField] [Range(0f, 1f)] float volume = 1;
    [SerializeField] [Range(.1f, 3f)] float pitch = 1;
    [SerializeField] [Range(0f, 1f)] float spatialBlend;
    [SerializeField] [Range(0f, 1f)] float doppler = 1;

    AudioSource audioSource;

    // Method to create a source for the specific sound to exist in
    // and apply the required settings
    public override void Initialise(GameObject holder)
    {
        if (clip == null) return;

        // Create the audio source
        audioSource = holder.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.spatialBlend = spatialBlend;
        audioSource.outputAudioMixerGroup = group;
        audioSource.dopplerLevel = doppler;
        //audioSource.rolloffMode = AudioRolloffMode.Linear;
        //audioSource.maxDistance = 25;
    }

    public override void Play()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    public override void Stop()
    {
        if(audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public override float GetLength()
    {
        if (audioSource != null)
        {
            return audioSource.clip.length;
        }

        return -1;
    }

    public override AudioSource GetSource()
    {
        return audioSource;
    }

    public override AudioClip GetClip()
    {
        return clip;
    }
}

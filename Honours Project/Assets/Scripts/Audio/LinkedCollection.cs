using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/Audio/Linked Collection")]
public class LinkedCollection : SoundCollection
{
    List<AudioSource> sources;
    int sourceIndex = 0;

    public override void Initialise(GameObject holder)
    {
        base.Initialise(holder);
        sources = new List<AudioSource>();
        foreach(PlayableSound sound in sounds)
        {
            AudioSource source = sound.GetSource();
            if (source != null) sources.Add(source);                // Add all audio source components into a list
        }
    }

    // Function to play a random audio clip within the collection
    public override void Play()
    {
        sources[sourceIndex].Stop();
        sources[sourceIndex].time = 0;
        sources[sourceIndex].Play();
        sourceIndex++;
        if (sourceIndex >= sources.Count) sourceIndex = 0;

        int index = Random.Range(0, sounds.Length);             // Choose the next random clip
        if(index < sounds.Length)
        {
            AudioClip clip = sounds[index].GetClip();           // Retrieve the clip
            if(clip != null)
            {
                sources[sourceIndex].clip = clip;               // Set the clip of the next audio source to the chosen clip
            }
        }
    }
}

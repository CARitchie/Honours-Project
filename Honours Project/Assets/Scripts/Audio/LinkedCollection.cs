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
            if (source != null) sources.Add(source);
        }
    }

    public override void Play()
    {
        int index = Random.Range(0, sounds.Length);
        if(index < sounds.Length)
        {
            AudioClip clip = sounds[index].GetClip();
            if(clip != null)
            {
                sources[sourceIndex].clip = clip;
                sources[sourceIndex].Play();
                sourceIndex++;
                if (sourceIndex >= sources.Count) sourceIndex = 0;
            }
        }
    }
}

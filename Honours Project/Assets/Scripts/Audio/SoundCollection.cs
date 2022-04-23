using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Taken from submission for 3rd year Games Programming Module
[CreateAssetMenu(menuName = "My Assets/Audio/Collection")]
public class SoundCollection : PlayableSound
{
    [SerializeField] protected PlayableSound[] sounds;

    // Method to initialise all the sounds contained within this collection
    public override void Initialise(GameObject holder)
    {
        PlayableSound[] finalSounds = new PlayableSound[sounds.Length];
        for (int i = 0; i < sounds.Length; i++)
        {
            PlayableSound newSound = Instantiate(sounds[i]);
            newSound.Initialise(holder);

            finalSounds[i] = newSound;
        }

        sounds = finalSounds;
    }

    // Method that selects a random sound in
    // the collection and plays it
    public override void Play()
    {
        int index = Random.Range(0, sounds.Length);
        if (index < sounds.Length)
        {
            sounds[index].Play();
        }
    }

    // Function to return a random clip from within the collection
    public override AudioClip GetClip()
    {
        int index = Random.Range(0, sounds.Length);
        if (index < sounds.Length)
        {
            return sounds[index].GetClip();
        }
        return null;
    }
}

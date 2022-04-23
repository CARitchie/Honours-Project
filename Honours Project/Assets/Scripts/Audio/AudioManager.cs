using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Taken and modified from submission for 3rd year Games Programming Module
public class AudioManager : MonoBehaviour
{
    [SerializeField] protected PlayableSound[] sounds;
    Dictionary<string, PlayableSound> dictionary = new Dictionary<string, PlayableSound>();

    protected virtual void Awake()
    {
        GameObject holder = new GameObject("AudioHolder");                          // Create a child gameobject to contain all of the audio sources
        holder.transform.parent = transform;
        holder.transform.localPosition = Vector3.zero;

        foreach (PlayableSound sound in sounds)
        {
            PlayableSound newSound = Instantiate(sound);                            // Create a copy of all of the sound objects
            newSound.Initialise(holder);                                            // Initialise the new sound objects

            dictionary.Add(newSound.GetTitle(), newSound);                          // Add the sounds to the dictionary
        }
    }

    // Function to find a sound and play it
    public void PlaySound(string key)
    {
        // Make sure that the gameObject is active and that the sound exists
        if (gameObject.activeInHierarchy && dictionary.ContainsKey(key))
        {
            dictionary[key].Play();
        }
    }

    // Function to stop all sounds
    public void StopAll()
    {
        foreach(KeyValuePair<string, PlayableSound> pair in dictionary)
        {
            pair.Value.Stop();
        }
    }
}

// Class to store sound objects as files within the project
public class PlayableSound : ScriptableObject
{
    [SerializeField] string title;

    public virtual void Play() { }
    public virtual void Stop() { }
    public virtual void Initialise(GameObject holder) { }
    public virtual float GetLength() { return -1; }
    public virtual AudioSource GetSource() { return null; }
    public virtual AudioClip GetClip() { return null; }

    public string GetTitle()
    {
        return title;
    }


}

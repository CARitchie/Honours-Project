using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedAudio : MonoBehaviour
{
    [SerializeField] float maxDelay;
    AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Play the sound after a random amount of time
        source.PlayDelayed(Random.Range(0, maxDelay));
    }
}

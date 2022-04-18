using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    AudioManager audioManager;

    static DialogueManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioManager = GetComponent<AudioManager>();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public static void PlayDialogue(string key)
    {
        if (Instance == null || SaveManager.GetBool(key)) return;

        SaveManager.SetBool(key, true);
        Instance.audioManager.StopAll();
        Instance.audioManager.PlaySound(key);
    }
}

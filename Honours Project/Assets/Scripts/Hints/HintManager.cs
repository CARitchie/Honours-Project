using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Hint[] hints;

    Dictionary<string, Hint> dictionary = new Dictionary<string, Hint>();
    static HintManager Instance;

    Queue<string> queue = new Queue<string>();

    bool running = false;

    private void Awake()
    {
        Instance = this;

        foreach(Hint hint in hints)
        {
            dictionary.Add(hint.Key, hint);
        }
    }

    // Function to play a hint
    public static void PlayHint(string key, bool replayable = false)
    {
        if (Instance == null || !Instance.HintPlayable(key ,replayable)) return;        // Return if the hint cannot be played

        SaveManager.SetBool(key, true);                                                 // Tell the savemanager that the hint has been played
        Instance.StartHint(key);

    }

    // Function to determine whether a hint is playable
    bool HintPlayable(string key, bool replayable)
    {
        if (!replayable && SaveManager.GetBool(key)) return false;
        if (dictionary == null) return false;
        if (!dictionary.ContainsKey(key)) return false;

        return true;
    }

    // Function to add a hint to the queue, and start the play hint coroutine if not already active
    void StartHint(string key)
    {
        queue.Enqueue(key);

        if (!running)
        {
            StartCoroutine(ShowHint());
        }
    }

    IEnumerator ShowHint()
    {
        running = true;

        Color col1 = text.color;
        col1.a = 0;
        Color col2 = col1;
        col2.a = 1;

        text.color = col1;
        while(queue.Count > 0)                                      // While there are still hints to be played
        {
            string key = queue.Dequeue();                           // Retrieve the next hint key from the queue

            text.text = dictionary[key].Text;

            text.enabled = true;

            float percent = 0;
            while (percent < 1)
            {
                text.color = Color.Lerp(col1, col2, percent);       // Fade the hint text colour in
                percent += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(3 + dictionary[key].Time);

            while (percent > 0)
            {
                text.color = Color.Lerp(col1, col2, percent);       // Fade the hint text colour out
                percent -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        text.enabled = false;

        running = false;
    }
}

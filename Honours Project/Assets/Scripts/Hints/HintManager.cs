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

    public static void PlayHint(string key)
    {
        if (Instance == null || !Instance.HintPlayable(key)) return;

        SaveManager.SetBool(key, true);
        Instance.StartHint(key);

    }

    bool HintPlayable(string key)
    {
        if (SaveManager.GetBool(key)) return false;
        if (dictionary == null) return false;
        if (!dictionary.ContainsKey(key)) return false;

        return true;
    }

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
        while(queue.Count > 0)
        {
            string key = queue.Dequeue();

            text.text = dictionary[key].Text;

            text.enabled = true;

            float percent = 0;
            while (percent < 1)
            {
                text.color = Color.Lerp(col1, col2, percent);
                percent += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(3 + dictionary[key].Time);

            while (percent > 0)
            {
                text.color = Color.Lerp(col1, col2, percent);
                percent -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        text.enabled = false;

        running = false;
    }
}

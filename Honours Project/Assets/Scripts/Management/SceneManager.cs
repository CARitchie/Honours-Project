using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image fadeScreen;
    [SerializeField] float fadeOutTime;
    [SerializeField] float fadeInTime;

    public static SceneManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void LoadScene(string sceneName)
    {
        if (Instance == null) return;

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public static void FadeToScene(string sceneName)
    {
        if (Instance == null) return;

        Instance.StartCoroutine(Instance.FadeOut(sceneName));
    }

    IEnumerator FadeOut(string sceneName)
    {
        float timer = fadeOutTime;
        while(fadeScreen.color.a < 1)
        {
            timer -= Time.unscaledDeltaTime;

            Color colour = fadeScreen.color;
            colour.a = Mathf.Lerp(1, 0, timer / fadeOutTime);
            fadeScreen.color = colour;

            yield return new WaitForEndOfFrame();
        }

        LoadScene(sceneName);
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fadeScreen.color.a != 0) StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float timer = fadeInTime;
        while (fadeScreen.color.a != 0)
        {
            timer -= Time.unscaledDeltaTime;

            Color colour = fadeScreen.color;
            colour.a = Mathf.Lerp(0, 1, timer / fadeInTime);
            fadeScreen.color = colour;

            yield return new WaitForEndOfFrame();
        }
    }

    public static void SetToBlack()
    {
        if (Instance == null) return;
        Color colour = Instance.fadeScreen.color;
        colour.a = 1;
        Instance.fadeScreen.color = colour;
    }
}

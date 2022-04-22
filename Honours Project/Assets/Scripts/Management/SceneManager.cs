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
    public static bool reload = false;

    private void Awake()
    {
        if(Instance == null)
        {
            // Keep this object active at all times
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

        if (reload) SaveManager.LoadGame();     // Load the game again if necessary, happens when the player dies

        Instance.StopAllCoroutines();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

        reload = false;
    }

    public static void FadeToScene(string sceneName)
    {
        if (Instance == null) return;

        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.FadeOut(sceneName));
    }

    IEnumerator FadeOut(string sceneName)
    {
        float timer = fadeOutTime;
        while(fadeScreen.color.a < 1)
        {
            timer -= Time.unscaledDeltaTime;

            // Increase the opacity of a black image, giving the appearance of fading out
            Color colour = fadeScreen.color;
            colour.a = Mathf.Lerp(1, 0, timer / fadeOutTime);
            fadeScreen.color = colour;

            yield return new WaitForEndOfFrame();
        }

        LoadScene(sceneName);
    }

    // Function called when a new scene has been loaded
    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fadeScreen.color.a != 0 && mode != LoadSceneMode.Additive) StartCoroutine(FadeIn());    // If the screen is black, fade in
    }

    IEnumerator FadeIn()
    {
        float timer = fadeInTime;
        while (fadeScreen.color.a != 0)
        {
            timer -= Time.unscaledDeltaTime;

            // Reduce the opacity of a black image, giving the appearance of fading in
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

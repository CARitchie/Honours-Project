using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class EndFight : MonoBehaviour
{
    [SerializeField] bool faster;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] CombatArea combat;
    [SerializeField] Animator colonyDoor;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject cinematicCamera;
    [SerializeField] PlayableDirector director;
    [SerializeField] GameObject[] otherPlanets;
    [SerializeField] MeshRenderer sun;
    float timer;
    int state = 0;
    bool countDown = true;

    // Start is called before the first frame update
    void Start()
    {
        cinematicCamera.SetActive(false);

        // Work out what the fight timer should be
        float health = Mathf.Clamp(EndingManager.health, 10, 70);
        float percent = 1 - ((health - 10) / 60);

        timer = percent * 4 * 60 + 60;
        if(faster) timer = 10;
        colonyDoor.SetBool("Open", false);

        DialogueManager.PlayDialogue("audio_fight");
    }

    private void Update()
    {
        if (!countDown) return;

        // Reduce the time
        timer -= Time.deltaTime;
        timerText.text = GetTime(timer);

        if(timer <= 0)
        {
            if(state == 0)
            {
                StartSecondSegment();
            }
            else if(state == 1)
            {
                OnOutOfTime();
            }
        }
    }

    // Start the section of the final fight where the player has to return to the colony ship
    void StartSecondSegment()
    {
        state = 1;
        if (faster) timer = 10;
        else timer = 60;                    // Give the player one minute to return
        combat.ForceOff();                  // Stop spawning enemy waves
        colonyDoor.SetBool("Open", true);   // Open the colony ship door

        DialogueManager.PlayDialogue("audio_oneMinute");
    }

    // Function to convert a time value into a timer string
    // Taken from https://answers.unity.com/questions/25614/how-do-i-format-a-string-into-daysminuteshoursseco.html
    public string GetTime(float value)
    {
        value = value * 1000;
        int d = (int)value;
        int minutes = d / (60 * 1000);
        int seconds = (d % (60 * 1000)) / 1000;

        return string.Format("{0:00}:{1:00}", minutes, seconds);

    }

    // Function to handle what happens when the player dies
    public void OnPlayerDeath()
    {
        DisablePlayer();
        if(state == 0)
        {
            // Blow up ship
            SetTime(80);
        }
        else
        {
            DialogueManager.PlayDialogue("audio_soLong");
            SetTime(50);
        }
    }

    // Function that occurs when the player reaches the colony ship
    public void OnPlayerReturn()
    {
        if (state == 0) return;
        DisablePlayer();
        SetTime(0);
    }

    // Function that occurs when the player fails to reach the colony ship in time
    public void OnOutOfTime()
    {
        DisablePlayer();
        SetTime(25);
        DialogueManager.PlayDialogue("audio_soLong");
    }

    // Function to prevent the player from making any more action
    void DisablePlayer()
    {
        countDown = false;
        combat.gameObject.SetActive(false);
        PlayerController.SetPaused(true);
        PlayerController.Instance.transform.parent.gameObject.SetActive(false);
        canvas.SetActive(false);
        colonyDoor.SetBool("Open", false);
        cinematicCamera.SetActive(true);

        GravityController.Disable();
        foreach(GameObject planet in otherPlanets)
        {
            planet.SetActive(false);
        }
        sun.enabled = false;

    }

    public void SetTime(float time)
    {
        director.Stop();
        director.time = time;
        director.Play();
    }

    // Function that occurs when the game is over
    public void GameEnd()
    {
        director.Stop();
        SceneManager.FadeToScene("MainMenu");
    }
}

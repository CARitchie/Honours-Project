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
    float timer;
    int state = 0;
    bool countDown = true;

    // Start is called before the first frame update
    void Start()
    {
        cinematicCamera.SetActive(false);

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

    void StartSecondSegment()
    {
        state = 1;
        if (faster) timer = 10;
        else timer = 60;
        combat.ForceOff();
        colonyDoor.SetBool("Open", true);

        DialogueManager.PlayDialogue("audio_oneMinute");
    }

    public string GetTime(float value)
    {
        value = value * 1000;
        int d = (int)value;
        int minutes = d / (60 * 1000);
        int seconds = (d % (60 * 1000)) / 1000;

        return string.Format("{0:00}:{1:00}", minutes, seconds);

    }

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

    public void OnPlayerReturn()
    {
        if (state == 0) return;
        DisablePlayer();
        SetTime(0);
    }

    public void OnOutOfTime()
    {
        DisablePlayer();
        SetTime(25);
        DialogueManager.PlayDialogue("audio_soLong");
    }

    void DisablePlayer()
    {
        countDown = false;
        combat.gameObject.SetActive(false);
        PlayerController.SetPaused(true);
        PlayerController.Instance.transform.parent.gameObject.SetActive(false);
        canvas.SetActive(false);
        colonyDoor.SetBool("Open", false);
        cinematicCamera.SetActive(true);
    }

    public void SetTime(float time)
    {
        director.Stop();
        director.time = time;
        director.Play();
    }

    public void GameEnd()
    {
        director.Stop();
        SceneManager.FadeToScene("MainMenu");
    }
}

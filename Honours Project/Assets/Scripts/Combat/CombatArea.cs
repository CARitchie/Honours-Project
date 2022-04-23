using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CombatArea : MonoBehaviour
{
    [SerializeField] EnemyWave[] waves;
    [SerializeField] GravitySource gravitySource;
    [SerializeField] string areaKey;
    [SerializeField] UnityEvent completed;
    [SerializeField] bool dontSave = false;

    int index;
    bool complete = false;

    private void Start()
    {
        if (SaveManager.IsCombatAreaComplete(areaKey))      // Find out if this area has been completed
        {
            complete = true;
        }
        else
        {
            SpawnInitialWave();
        }
        
    }

    // Function to spawn a wave that should be active even if the area hasn't yet been activated
    void SpawnInitialWave()
    {
        if (waves == null || waves.Length <= 0 || !waves[0].IsInitialWave()) return;

        SpawnNextWave();
    }

    void SpawnNextWave()
    {
        if (waves == null || index >= waves.Length) return;

        //waves[index] = Instantiate(waves[index], transform).GetComponent<EnemyWave>();
        //waves[index].transform.localPosition = Vector3.zero;
        //waves[index].transform.localEulerAngles = Vector3.zero;
        waves[index].SpawnWave(this);
        index++;
    }

    IEnumerator SpawnAllWaves()
    {
        if (waves != null)
        {
            while (index < waves.Length && gameObject.activeSelf)
            {
                if (waves[index].TimeToSpawn())
                {
                    SpawnNextWave();
                }

                yield return new WaitForEndOfFrame();
            }
        }

    }

    public void OnPlayerEnter()
    {
        if (!complete)
        {
            PlayerController.Instance.SetCanSave(false);        // Prevent the player from saving
            StopAllCoroutines();
            StartCoroutine(SpawnAllWaves());                    // Start to spawn all of the waves
        }
    }

    public void CheckComplete()
    {
        if (complete) return;

        for(int i = 0; i < waves.Length; i++)
        {
            if (waves[i] != null && !waves[i].IsComplete()) return;     // Return if any of the waves are incomplete
        }

        complete = true;
        OnComplete();

    }

    public void OnComplete()
    {
        PlayerController.Instance.SetCanSave(true);                     // Allow the player to save
        if (dontSave || PlayerController.Instance.IsDead()) return;     // If the player is dead, or this area shouldn't autosave, return
        SaveManager.CompleteCombatArea(areaKey);
        GameManager.Autosave();
        completed?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        if(other.attachedRigidbody.GetComponent<PlayerDetails>() != null)
        {
            OnPlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        if (other.attachedRigidbody.GetComponent<PlayerDetails>() != null)
        {
            PlayerController.Instance.SetCanSave(true);
        }
    }

    public Vector3 GetVelocity()
    {
        return gravitySource.GetVelocity();
    }

    public GravitySource GetSource()
    {
        return gravitySource;
    }

    // Function to prevent any more waves from spawning
    public void ForceOff()
    {
        complete = true;
        StopAllCoroutines();
    }

    public void PlayDialogue(string key)
    {
        DialogueManager.PlayDialogue(key);
    }
}

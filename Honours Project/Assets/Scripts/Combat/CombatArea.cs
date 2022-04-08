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

    int index;
    bool complete = false;

    private void Start()
    {
        if (SaveManager.IsCombatAreaComplete(areaKey))
        {
            complete = true;
        }
        else
        {
            SpawnInitialWave();
        }
        
    }

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
            PlayerController.Instance.SetCanSave(false);
            StopAllCoroutines();
            StartCoroutine(SpawnAllWaves());
        }
    }

    public void CheckComplete()
    {
        if (complete) return;

        for(int i = 0; i < waves.Length; i++)
        {
            if (waves[i] != null && !waves[i].IsComplete()) return;
        }

        complete = true;
        OnComplete();

    }

    public void OnComplete()
    {
        Debug.Log("Yay");
        PlayerController.Instance.SetCanSave(true);
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    [SerializeField] bool initialWave;
    [SerializeField] float spawnDelay;
    [SerializeField] EnemySpawnPoint[] enemies;

    bool spawned = false;
    float timer = 0;
    CombatArea area;
    bool complete = false;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public bool IsInitialWave()
    {
        return initialWave;
    }

    public bool TimeToSpawn()
    {
        timer += Time.deltaTime;
        return timer >= spawnDelay;
    }

    public void SpawnWave(CombatArea area)
    {
        if (spawned) return;
        spawned = true;
        Debug.Log("Spawning Wave");
        this.area = area;
        gameObject.SetActive(true);

        if (initialWave)
        {
            StartCoroutine(WaitAFrame());
        }
        else
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SetWave(this);
                enemies[i].Spawn();
            }
        }

    }

    IEnumerator WaitAFrame()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].SetWave(this);
            enemies[i].Spawn();
        }
    }

    public bool IsComplete()
    {
        if (complete) return true;

        for(int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null && enemies[i].IsAlive()) return false;
        }
        complete = true;
        return true;
    }

    public void CheckComplete()
    {
        if (area != null) area.CheckComplete();
    }

    public Vector3 GetVelocity()
    {
        return area.GetVelocity();
    }

    public GravitySource GetSource()
    {
        return area.GetSource();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    [SerializeField] bool initialWave;
    [SerializeField] float spawnDelay;
    [SerializeField] EnemySpawnPoint[] enemies;

    CombatArea area;

    bool spawned = false;
    float timer = 0;
    bool complete = false;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public bool IsInitialWave()
    {
        return initialWave;
    }

    // Function to determine whether enough time has passed for this wave to spawn
    public bool TimeToSpawn()
    {
        timer += Time.deltaTime;
        return timer >= spawnDelay;
    }

    // Function to activate the wave
    public void SpawnWave(CombatArea area)
    {
        if (spawned) return;
        spawned = true;
        Debug.Log("Spawning Wave");
        this.area = area;
        gameObject.SetActive(true);

        if (initialWave)                            // If this is happening at the start of the game
        {
            StartCoroutine(WaitAFrame());           // Wait a couple of frames to ensure that spawned enemies will correctly match the nearest gravity source's velocity       
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

    // Function to determine whether this wave has been completed
    public bool IsComplete()
    {
        if (complete) return true;

        for(int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null && enemies[i].IsAlive()) return false;       // Return false if any of the enemies are still alive
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField] float spawnDelay;
    [SerializeField] EnemyDetails enemy;
    [SerializeField] bool inScene = false;
    EnemyWave wave;

    public void Spawn()
    {
        if (spawnDelay > 0) StartCoroutine(DelayedSpawn());
        else InstantSpawn();
    }

    IEnumerator DelayedSpawn()
    {
        float timer = 0;
        while(timer < spawnDelay)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        InstantSpawn();
    }

    public void InstantSpawn()
    {
        if (!inScene)
        {
            enemy = Instantiate(enemy, transform).GetComponent<EnemyDetails>();
            enemy.GetComponentInChildren<EnemyController>()?.SetNearestSource(wave.GetSource());
            enemy.gameObject.SetActive(false);
            enemy.transform.localPosition = Vector3.zero;
            enemy.transform.localEulerAngles = Vector3.zero;
        }
        enemy.SetWave(wave);
        enemy.gameObject.SetActive(true);
        enemy.GetComponentInChildren<Rigidbody>().AddForce(wave.GetVelocity(), ForceMode.VelocityChange);
    }

    public void SetWave(EnemyWave wave)
    {
        this.wave = wave;
    }

    public bool IsAlive()
    {
        return enemy != null && enemy.IsAlive();
    }

    public GravitySource GetSource()
    {
        return wave.GetSource();
    }
}

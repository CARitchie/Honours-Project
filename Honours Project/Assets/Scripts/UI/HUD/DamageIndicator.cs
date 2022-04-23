using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : PoolObject
{
    [SerializeField] Image image;
    Transform target;
    float timer;

    protected override void Start()
    {
    }

    public override void OnExitQueue()
    {
        base.OnExitQueue();

        ResetTimer();
    }

    public void SetTarget(Transform target)
    {
        gameObject.SetActive(true);
        this.target = target;
    }

    public void RotateTo(Transform player)
    {
        if (target == null) return;

        // Work out the angle between the player and the damage causer
        Vector3 dir = target.position - player.position;
        Vector3 right = Vector3.Cross(dir, player.up).normalized;
        dir = Vector3.Cross(-right, player.up);

        float multi = Vector3.Dot(player.right, dir) > 0 ? 1 : -1;

        // Rotate the damage indicator so that it points towards the damage causer
        transform.localEulerAngles = Vector3.forward * (Vector3.Dot(player.forward, dir) * 90 - 90) * multi;
    }

    public bool EqualTargets(Transform otherTarget)
    {
        if(otherTarget == target)
        {
            ResetTimer();
            return true;
        }
        return false;
    }

    // Function to fade the damage indicator out
    // Return true if completely invisible
    public bool Fade()
    {
        timer -= Time.deltaTime;
        Color colour = image.color;
        colour.a -= Time.deltaTime / 3;
        image.color = colour;

        if(timer <= 0)
        {
            gameObject.SetActive(false);
            return true;
        }
        
        return false;
    }

    // If damage is caused by this damage causer again, reset opacity and timer to full
    void ResetTimer()
    {
        timer = 3;
        Color colour = image.color;
        colour.a = 1;
        image.color = colour;
    }
}

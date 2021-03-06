using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : PoolObject
{
    [SerializeField] GameObject hitMarker;
    [SerializeField] protected float damage = 10;
    [SerializeField] float despawnTime = 10;
    [SerializeField] protected UnityEvent OnHit;
    Rigidbody rb;
    Vector3 lastPos;
    protected Transform body;
    TrailRenderer trail;
    float timer;
    Transform originator;
    protected float damageMultiplier = 1;

    int layerMask = ~((1 << 6) | (1 << 2) | (1 << 11) | (1 << 12) | (1 << 13));

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        hitMarker.SetActive(false);
    }

    public override void OnExitQueue()
    {
        rb.velocity = Vector3.zero;
        timer = despawnTime;
        base.OnExitQueue();
    }

    // Function to handle when fired from a gun
    public void Fire(Vector3 velocity, Transform body, Transform originator, float multiplier)
    {
        rb.velocity = velocity;             // Set the velocity
        transform.forward = velocity;       // Face forward
        damageMultiplier = multiplier;

        this.body = body;
        this.originator = originator;
        Vector3 offset = Vector3.zero;
        if (body != null) offset = body.position;
        lastPos = transform.position - offset;
        if(trail != null) trail.Clear();
    }

    private void FixedUpdate()
    {
        if (DetectCollision()) return;

        // If too much time passes, despawn
        timer -= Time.fixedDeltaTime;
        if (timer <= 0) {
            Despawn();
        }
    }

    protected virtual void Despawn()
    {
        gameObject.SetActive(false);
        timer = despawnTime;
    }

    // Function to handle when a collision is made
    public virtual void HitSuccess(RaycastHit hit, Vector3 direction)
    {
        PlaceHitMarker(hit, direction);

        OnHit?.Invoke();

        if (hit.transform.TryGetComponent(out Damageable damageable))
        {
            damageable.OnShot(damage * damageMultiplier, originator);       // Add damage to the hit object
        }

        gameObject.SetActive(false);
    }

    protected void PlaceHitMarker(RaycastHit hit, Vector3 direction)
    {
        if(hitMarker == null)
        {
            Debug.LogWarning("No hitmarker present");
            return;
        }
        hitMarker.transform.position = hit.point;
        hitMarker.transform.parent = hit.collider != null ? hit.collider.transform : null;
        //hitMarker.transform.parent = body;
        hitMarker.transform.up = -direction;
        hitMarker.SetActive(true);
    }

    // Function to determine whether the projectile collided with any objects
    bool DetectCollision()
    {
        Vector3 offset = Vector3.zero;
        if (body != null) offset = body.position;

        lastPos += offset;
        if(Physics.Raycast(lastPos, transform.position - lastPos, out RaycastHit hit, Vector3.Distance(lastPos, transform.position), layerMask))    // Find any collisions
        {
            HitSuccess(hit, transform.position - lastPos);
            return true;
        }

        lastPos = transform.position - offset;
        return false;
    }
}

public interface Damageable
{
    void OnShot(float damage, Transform origin);
    void OnMelee(float damage, Transform origin);
    void OnExplosion(float damage);
}

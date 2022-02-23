using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : PoolObject
{
    [SerializeField] GameObject hitMarker;
    [SerializeField] float damage = 10;
    [SerializeField] float despawnTime = 10;
    Rigidbody rb;
    Vector3 lastPos;
    Transform body;
    TrailRenderer trail;
    float timer;

    int layerMask = ~((1 << 6) | (1 << 2) | (1 << 11) | (1 << 12));

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
    }

    public override void OnExitQueue()
    {
        rb.velocity = Vector3.zero;
        timer = despawnTime;
        base.OnExitQueue();
    }

    public void Fire(Vector3 velocity, Transform body)
    {
        rb.velocity = velocity;

        this.body = body;
        Vector3 offset = Vector3.zero;
        if (body != null) offset = body.position;
        lastPos = transform.position - offset;
        if(trail != null) trail.Clear();
    }

    private void FixedUpdate()
    {
        if (DetectCollision()) return;

        timer -= Time.fixedDeltaTime;
        if (timer <= 0) {
            gameObject.SetActive(false);
            timer = despawnTime;
        }
    }

    public void HitSuccess(RaycastHit hit)
    {
        GameObject marker = Instantiate(hitMarker);
        marker.transform.position = hit.point;
        marker.transform.parent = hit.collider.transform;

        if(hit.transform.TryGetComponent(out Damageable damageable))
        {
            damageable.OnShot(damage);
        }

        gameObject.SetActive(false);
    }

    bool DetectCollision()
    {
        Vector3 offset = Vector3.zero;
        if (body != null) offset = body.position;

        lastPos += offset;
        if(Physics.Raycast(lastPos, transform.position - lastPos, out RaycastHit hit, Vector3.Distance(lastPos, transform.position), layerMask))
        {
            HitSuccess(hit);
            return true;
        }

        lastPos = transform.position - offset;
        return false;
    }
}

public interface Damageable
{
    void OnShot(float damage);
    void OnMelee(float damage);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryoPod : MonoBehaviour, Interact
{
    [SerializeField] GravitySource source;
    [SerializeField] MeshCollider meshCollider;

    Rigidbody rb;
    GravityReceiver gravity;

    bool grabbed = false;
    Vector3 offset;
    float mass;
    bool active = true;

    public void OnEnter()
    {
        if (!grabbed)
        {
            HUD.SetInteractText("Pick Up");
        }
        else
        {
            HUD.SetInteractText("Drop");
        }
        
    }

    public void OnExit()
    {
        HUD.ClearInteractText();
    }

    public void OnSelect()
    {
        if (!active) return;

        if (!grabbed)
        {
            HUD.SetInteractText("Drop");
            AttachToTransform(Camera.main.transform);
            meshCollider.enabled = false;

            ObjectInteractor.podGrabbed = true;
        }
        else
        {
            HUD.SetInteractText("Pick Up");
            Detach();
            ObjectInteractor.podGrabbed = false;
        }

        grabbed = !grabbed;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gravity = GetComponent<GravityReceiver>();
        mass = rb.mass;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = source.GetVelocity();
    }

    public void AttachToTransform(Transform parent)
    {
        transform.parent = parent;
        offset = transform.localPosition;
        gravity.enabled = false;
        Destroy(rb);
    }

    public void Detach()
    {
        Vector3 velocity = transform.parent.GetComponentInParent<Rigidbody>().velocity;
        transform.parent = null;

        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.mass = mass;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.velocity = velocity;

        gravity.SetRigidBody(rb);
        gravity.enabled = true;
        meshCollider.enabled = true;
        StopAllCoroutines();
    }

    public void StartAlign()
    {
        StartCoroutine(Align());
    }

    IEnumerator Align()
    {
        float percent = 0;
        Quaternion start = transform.localRotation;
        Vector3 pos = transform.localPosition;
        while (percent < 1)
        {
            percent += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(pos, Vector3.zero, percent);
            transform.localRotation = Quaternion.Slerp(start, Quaternion.identity, percent);
            yield return new WaitForEndOfFrame();
        }

        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    public void Disable()
    {
        active = false;
    }

}

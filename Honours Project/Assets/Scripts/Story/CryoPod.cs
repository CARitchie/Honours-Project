using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryoPod : MonoBehaviour, Interact
{
    [SerializeField] string key;
    [SerializeField] GravitySource source;
    [SerializeField] MeshCollider meshCollider;

    public static List<CryoPod> allPods = new List<CryoPod>();

    Rigidbody rb;
    GravityReceiver gravity;

    bool grabbed = false;
    Vector3 offset;
    float mass;
    bool active = true;

    NearObject nearObject;

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
            SaveManager.SetPodState(key, 1);
            HUD.SetInteractText("Drop");
            AttachToTransform(Camera.main.transform);
            meshCollider.enabled = false;
            

            ObjectInteractor.podGrabbed = true;
        }
        else
        {
            HUD.SetInteractText("Pick Up");
            Detach();
            nearObject.Enable();
            ObjectInteractor.podGrabbed = false;
        }

        grabbed = !grabbed;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gravity = GetComponent<GravityReceiver>();
        nearObject = GetComponentInChildren<NearObject>();
        mass = rb.mass;
    }

    // Start is called before the first frame update
    void Start()
    {
        bool destroy = false;
        SaveFile.Pod podData = SaveManager.GetPod(key);
        if (podData == null)
        {
            rb.velocity = source.GetVelocity();

        }
        else
        {
            int state = podData.GetState();
            if(state >= 4)
            {
                destroy = true;
                Destroy(gameObject);
            }
            else if(podData.transform.LoadIntoTransform(transform, out Vector3 velocity))
            {
                rb.velocity = velocity;
            }
        }

        if(!destroy) allPods.Add(this);
    }

    private void OnDestroy()
    {
        if (allPods.Contains(this)) allPods.Remove(this);
    }

    public void AttachToTransform(Transform parent)
    {
        transform.parent = parent;
        offset = transform.localPosition;
        gravity.enabled = false;
        nearObject.Disable();
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

    public void PutIntoLift()
    {
        SaveManager.SetPodState(key, 4);
    }

    public string Key { get { return key; } }

    public GravityReceiver Receiver { get { return gravity; } }

}

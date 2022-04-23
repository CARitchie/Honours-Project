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
            SaveManager.SetPodState(key, 1);                // Tell the save file that this pod has been found
            HUD.SetInteractText("Drop");
            AttachToTransform(Camera.main.transform);       // Attach the pod to the camera
            meshCollider.enabled = false;                   // Disabel the pod's collider
            if (SaveManager.NumberOfFoundPods() < 1)        // If no pods have previously been found
            {
                HUD.ChangeObjectiveTarget(0);                   // Dispay an objective marker
                DialogueManager.PlayDialogue("audio_found");    // Play dialogue explaining what to do
            }
            ObjectInteractor.podGrabbed = true;
        }
        else
        {
            HUD.SetInteractText("Pick Up");
            Detach();
            nearObject.Enable();
            //HUD.DisableObjectiveMarker(0);
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
            if(state >= 4)                      // If the pod has been recovered
            {
                destroy = true;
                Destroy(gameObject);            // Destroy this gameobject
            }
            else if(podData.transform.LoadIntoTransform(transform, out Vector3 velocity))       // Otherwise, load the saved position and apply it
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

    // Function to make this pod move with the desired transform
    public void AttachToTransform(Transform parent)
    {
        transform.parent = parent;
        offset = transform.localPosition;
        gravity.enabled = false;
        nearObject.Disable();
        Destroy(rb);        // Strange results occurred without deleting the rigidbody
    }

    // Function to make the pod behave like an ordinary physics object again
    public void Detach()
    {
        Vector3 velocity = transform.parent.GetComponentInParent<Rigidbody>().velocity;
        transform.parent = null;

        // Recreate the rigidbody
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

    // Function to gradually change local position and rotation to 0
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
        SaveManager.SetPodState(key, 4);        // Tell the save file that the pod has been recovered
    }

    public string Key { get { return key; } }

    public GravityReceiver Receiver { get { return gravity; } }

}

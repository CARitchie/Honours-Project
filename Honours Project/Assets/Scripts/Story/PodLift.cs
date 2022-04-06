using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodLift : MonoBehaviour
{
    [SerializeField] Transform podHolder;
    Animator anim;

    CryoPod lastPod;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.TryGetComponent(out CryoPod pod))
        {
            pod.Disable();
            pod.AttachToTransform(podHolder);
            pod.StartAlign();
            pod.PutIntoLift();

            GameManager.Autosave();

            lastPod = pod;

            StartCoroutine(StartDecent());
        }
    }

    IEnumerator StartDecent()
    {
        yield return new WaitForSeconds(1);
        anim.SetTrigger("Activate");
    }

    public void DestroyPod()
    {
        if (lastPod != null)
        {
            Destroy(lastPod.gameObject);
            lastPod = null;
        }
    }
}

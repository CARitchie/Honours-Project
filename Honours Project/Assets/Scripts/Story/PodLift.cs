using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodLift : MonoBehaviour
{
    [SerializeField] Transform podHolder;
    Animator anim;

    CryoPod lastPod;
    bool inUse = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (inUse) return;
        if (other.attachedRigidbody == null) return;
        if (other.attachedRigidbody.TryGetComponent(out CryoPod pod))
        {
            inUse = true;
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
        yield return new WaitForSeconds(3);
        inUse = false;
        if (SaveManager.NumberOfFoundPods() <= 1)
        {
            HUD.ChangeObjectiveTarget(1);
            DialogueManager.PlayDialogue("audio_delivered");
        }
    }

    public void DestroyPod()
    {
        if (lastPod != null)
        {
            Useful.DestroyGameObject(lastPod.gameObject);
            lastPod = null;
        }
    }
}

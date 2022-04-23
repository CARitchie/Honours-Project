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
        if (other.attachedRigidbody.TryGetComponent(out CryoPod pod))       // If a cryo pods triggerd the collider
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

    // Function to lower the lift
    IEnumerator StartDecent()
    {
        yield return new WaitForSeconds(1);         // Provide enough time for the cryo pod to align properly
        anim.SetTrigger("Activate");                // Start the lift descend animation
        yield return new WaitForSeconds(3);         // Wait for the lift to return
        inUse = false;
        if (SaveManager.NumberOfFoundPods() <= 1)   // If this was the first pod delivered
        {
            HUD.ChangeObjectiveTarget(1);           // Add an objective marker to the science lab
            DialogueManager.PlayDialogue("audio_delivered");
        }
        else
        {
            HintManager.PlayHint("hint_newSacrifice", true);        // Tell the player that a new sacrifice is available
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

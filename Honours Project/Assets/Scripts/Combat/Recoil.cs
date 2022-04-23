using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Taken from https://youtu.be/geieixA4Mqc?t=522
public class Recoil : MonoBehaviour
{
    Vector3 currentRot;
    Vector3 targetRot;

    [SerializeField] float snappiness;
    [SerializeField] float returnSpeed;


    // Update is called once per frame
    void LateUpdate()
    {
        targetRot = Vector3.Lerp(targetRot, Vector3.zero, returnSpeed * Time.deltaTime);    // Find a rotation from the target back to 0
        currentRot = Vector3.Slerp(currentRot, targetRot, snappiness * Time.deltaTime);     // Rotate towards the target rotation
        transform.localRotation = Quaternion.Euler(currentRot);                             // Set the rotation
    }

    public void RecoilFire(Vector3 dir)
    {
        targetRot += new Vector3(dir.x, Random.Range(-dir.y, dir.y), Random.Range(-dir.z, dir.z));      // Increase the target rotation by a random amount
    }
}

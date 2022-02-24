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
        targetRot = Vector3.Lerp(targetRot, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRot = Vector3.Slerp(currentRot, targetRot, snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRot);
    }

    public void RecoilFire(Vector3 dir)
    {
        targetRot += new Vector3(dir.x, Random.Range(-dir.y, dir.y), Random.Range(-dir.z, dir.z));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotSpeed;


    public void MoveToTransform(Transform transform)
    {
        StopAllCoroutines();
        this.transform.parent = transform;
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        float percent = 0;
        Quaternion start = transform.localRotation;
        while (transform.localPosition.sqrMagnitude > 0.01f || transform.localEulerAngles.sqrMagnitude > 0.01f)
        {
            yield return new WaitForEndOfFrame();

            percent += rotSpeed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, moveSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(start, Quaternion.identity, percent);
        }

        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }
}

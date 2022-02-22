using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float rotSpeed;

    AtmosphereRenderer atmosphere;
    CloudRenderer cloud;

    private void Start()
    {
        atmosphere = GetComponent<AtmosphereRenderer>();
        cloud = GetComponent<CloudRenderer>();

        InputController.Cloud += ToggleClouds;
        InputController.Atmosphere += ToggleAtmospheres;
    }

    private void OnDestroy()
    {
        InputController.Cloud -= ToggleClouds;
        InputController.Atmosphere -= ToggleAtmospheres;
    }

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
        Vector3 pos = transform.localPosition;
        while (percent < 1)
        {
            percent += rotSpeed * Time.deltaTime;
            transform.localPosition = Vector3.Lerp(pos, Vector3.zero, percent);
            transform.localRotation = Quaternion.Slerp(start, Quaternion.identity, percent);
            yield return new WaitForEndOfFrame();
        }

        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    public void ToggleClouds()
    {
        cloud.enabled = !cloud.enabled;
    }

    public void ToggleAtmospheres()
    {
        atmosphere.enabled = !atmosphere.enabled;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{

    [SerializeField] float maxX;
    [SerializeField] float maxY;

    [SerializeField] float speed;

    Vector3 target;

    private void FixedUpdate()
    {
        // Change the target position every 0.02 seconds
        target.Set(Random.Range(-maxX, maxX), Random.Range(-maxY, maxY), 0);
    }

    private void Update()
    {
        // Move the camera towards the target position every frame
        transform.localPosition = Vector3.Lerp(transform.localPosition, target, speed * Time.deltaTime);
    }
}

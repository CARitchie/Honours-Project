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
        target.Set(Random.Range(-maxX, maxX), Random.Range(-maxY, maxY), 0);
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, target, speed * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    [SerializeField] RectTransform image1;

    [SerializeField] Transform player;
    [SerializeField] Transform planet;

    Vector3 targetPos = new Vector3(200, 0, 0);

    float size = 800;

    private void Start()
    {
        size = image1.sizeDelta.x;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTarget();
        image1.localPosition = targetPos;
    }

    void UpdateTarget()
    {
        Vector3 dir1 = (player.position - planet.position).normalized;
        Vector3 cross1 = Vector3.Cross(dir1, planet.up).normalized;
        Vector3 cross2 = Vector3.Cross(cross1, dir1);

        float dot = Vector3.Dot(cross2, player.forward);
        float dot2 = Vector3.Dot(cross2, player.right);

        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        angle += 90;

        float x = (angle / 360) * size;

        if (float.IsNaN(x)) return;

        if (dot2 < 0)
        {
            // East
            targetPos.x = (size/2) - x;
        }
        else
        {
            // West
            targetPos.x = x;
        }
        
    }
}

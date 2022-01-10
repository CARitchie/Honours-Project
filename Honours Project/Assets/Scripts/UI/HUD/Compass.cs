using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [SerializeField] RectTransform image1;
    [SerializeField] float fadeSpeed;

    PersonController player;
    GravitySource planet;
    Transform playerT;

    Image[] images;

    Vector3 targetPos = new Vector3(200, 0, 0);

    static bool active = true;

    float size = 800;

    private void Start()
    {
        size = image1.sizeDelta.x;

        player = PlayerController.Instance;
        playerT = player.transform;

        images = image1.GetComponentsInChildren<Image>();

        SetAlpha(0);
    }

    // Update is called once per frame
    void Update()
    {
        planet = player.GetNearestSource();

        if (planet == null || !active)
        {
            FadeOut();
            return;
        }

        FadeIn();

        UpdateTarget();
        image1.localPosition = targetPos;
    }

    void UpdateTarget()
    {
        Vector3 dir1 = (playerT.position - planet.transform.position).normalized;
        Vector3 cross1 = Vector3.Cross(dir1, planet.transform.up).normalized;
        Vector3 cross2 = Vector3.Cross(cross1, dir1);

        float dot = Vector3.Dot(cross2, playerT.forward);
        float dot2 = Vector3.Dot(cross2, playerT.right);

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

    void FadeIn()
    {
        float alpha = images[0].color.a;
        if(alpha < 1)
        {
            alpha = Mathf.Clamp01(alpha + Time.deltaTime * fadeSpeed);
            SetAlpha(alpha);
        }
    }

    void FadeOut()
    {
        float alpha = images[0].color.a;
        if (alpha > 0)
        {
            alpha = Mathf.Clamp01(alpha - Time.deltaTime * fadeSpeed * 2);
            SetAlpha(alpha);
        }
    }

    void SetAlpha(float percent)
    {
        Color colour = images[0].color;
        colour.a = percent;
        foreach(Image image in images)
        {
            image.color = colour;
        }
    }

    public static void SetActive(bool value)
    {
        active = value;
    }
}

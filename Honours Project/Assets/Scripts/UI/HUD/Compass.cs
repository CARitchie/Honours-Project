using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [SerializeField] RectTransform compassImage;
    [SerializeField] RectTransform shipImage;
    [SerializeField] float fadeSpeed;
    [SerializeField] Transform ship;

    PersonController player;
    GravitySource planet;
    Transform playerT;

    Image[] images;

    Vector3 targetPos = new Vector3(200, 0, 0);

    static bool active = true;

    float size = 800;

    private void Start()
    {
        size = compassImage.sizeDelta.x;

        player = PlayerController.Instance;
        playerT = player.transform;

        images = GetComponentsInChildren<Image>();

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

        FindShip();
        UpdateTarget();
        compassImage.localPosition = targetPos;
    }

    void UpdateTarget()
    {
        Vector3 north = planet.GetNorthDirection(playerT);
        float dot = Vector3.Dot(north, playerT.forward);
        float dot2 = Vector3.Dot(north, playerT.right);

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

    void FindShip()
    {
        Vector3 directToShip = (playerT.position - ship.position).normalized;
        Vector3 shipPlayerRight = Vector3.Cross(directToShip, playerT.up).normalized;

        Vector3 toShip = Vector3.Cross(shipPlayerRight, playerT.up);
        float shipAngle = Vector3.Dot(toShip, playerT.forward);

        float rightAngle = Vector3.Dot(toShip, playerT.right);

        float angle = Mathf.Acos(shipAngle) * Mathf.Rad2Deg;

        float x = (angle / 360) * size;  

        if (float.IsNaN(x)) return;

        Vector3 shipPos = shipImage.localPosition;

        if (rightAngle < 0)
        {
            // East
            shipPos.x = -x;
        }
        else
        {
            // West
            shipPos.x = x;
        }

        shipImage.localPosition = shipPos;
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

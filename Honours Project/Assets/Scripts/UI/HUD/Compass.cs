using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [SerializeField] RectTransform compassImage;
    [SerializeField] TextMeshProUGUI distanceText;
    [SerializeField] float fadeSpeed;

    public static Compass Instance;

    PersonController player;
    GravitySource planet;
    Transform playerT;

    List<Image> images = new List<Image>();

    List<CompassItem> items = new List<CompassItem>();

    Vector3 targetPos = new Vector3(200, 0, 0);

    // This could cause issues
    static bool active = true;

    float size = 800;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        size = compassImage.sizeDelta.x;

        player = PlayerController.Instance;
        playerT = player.transform;

        Image[] tempImages = GetComponentsInChildren<Image>();
        for(int i = 0; i < tempImages.Length; i++)
        {
            images.Add(tempImages[i]);
        }


        SetAlpha(0);
    }

    // Update is called once per frame
    void Update()
    {
        GravitySource tempPlanet = player.GetNearestSource();

        if (tempPlanet == null || !active)
        {
            FadeOut();
            return;
        }

        if(tempPlanet != planet)
        {
            CheckSamePlanet();
            planet = tempPlanet;
        }

        FadeIn();

        float minDist = 1000;
        int minIndex = -1;

        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].active)
            {
                float distance = FindItem(items[i]);

                if (distance < 0) distance *= -1;
                if(distance < 30 && distance < minDist)
                {
                    minDist = distance;
                    minIndex = i;
                }
            }
        }

        if(minIndex != -1)
        {
            if (!distanceText.enabled)
            {
                distanceText.enabled = true;
                Color colour = distanceText.color;
                colour.a = 1;
                distanceText.color = colour;
            }

            int distance = (int)Vector3.Distance(player.transform.position, items[minIndex].transform.position);
            distanceText.text = distance.ToString() + "m";
            
        }
        else
        {
            if (distanceText.enabled)
            {
                distanceText.enabled = false;
            }
        }


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

    float FindItem(CompassItem item)
    {
        Vector3 directToItem = (playerT.position - item.transform.position).normalized;
        Vector3 itemPlayerRight = Vector3.Cross(directToItem, playerT.up).normalized;

        Vector3 toItem = Vector3.Cross(itemPlayerRight, playerT.up);
        float itemAngle = Vector3.Dot(toItem, playerT.forward);

        float rightAngle = Vector3.Dot(toItem, playerT.right);

        float angle = Mathf.Acos(itemAngle) * Mathf.Rad2Deg;

        float x = (angle / 360) * size;

        if (float.IsNaN(x)) return 1000;

        if (rightAngle < 0)
        {
            // East
            item.SetPosition(-x);
        }
        else
        {
            // West
            item.SetPosition(x);
        }

        return x;
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
        foreach(Image image in images)
        {
            Color colour = image.color;
            colour.a = percent;
            image.color = colour;
        }
    }

    public static void SetActive(bool value)
    {
        active = value;
    }

    void CheckSamePlanet()
    {
        Transform playerPlanet = player.GetNearestSource().transform;

        foreach(CompassItem item in items)
        {
            if (!item.SameParent(playerPlanet))
            {
                item.SetIconActive(false);
            }
            else
            {
                item.SetIconActive(true);
            }
            
        }
    }

    public static void AddItem(CompassItem item)
    {
        if (Instance == null) return;

        Instance.items.Add(item);

        Image newImage = item.CreateNewIcon(Instance.transform);
        Instance.images.Add(newImage);

        if (!active || Instance.planet == null){
            Color colour = newImage.color;
            colour.a = 0;
            newImage.color = colour;
        }

        if (!item.SameParent(Instance.planet == null ? null : Instance.planet.transform)) item.SetIconActive(false);
    }

    public static void RemoveItem(CompassItem item)
    {
        if (Instance == null) return;

        Instance.images.Remove(item.GetIcon());
        Instance.items.Remove(item);
        item.DestroyIcon();
    }
}

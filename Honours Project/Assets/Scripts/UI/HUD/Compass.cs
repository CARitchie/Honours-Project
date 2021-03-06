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
    [SerializeField] Color pulseColour;
    
    public static Compass Instance;

    // This could cause issues
    static bool active = true;

    PersonController player;
    GravitySource planet;
    Transform playerT;

    List<Graphic> images = new List<Graphic>();
    Graphic[] pulseImages;

    List<CompassItem> items = new List<CompassItem>();
    List<Transform> nearObjects = new List<Transform>();

    Vector3 targetPos = new Vector3(200, 0, 0);
    AudioManager audioManager;

    float size = 800;
    float pulseTime;
    float pulsePercent = 0;
    float pulseDelay;

    bool pulseActive = true;
    bool sacrificedCompass = false;


    private void Awake()
    {
        Instance = this;
        pulseImages = GetComponentsInChildren<Graphic>(true);
        audioManager = GetComponentInParent<AudioManager>();
    }

    private void Start()
    {
        size = compassImage.sizeDelta.x;

        player = PlayerController.Instance;
        playerT = player.transform;

        for (int i = 0; i < pulseImages.Length; i++)
        {
            images.Add(pulseImages[i]);         // Add pulse images to the list of images that will fade in/out
        }

        pulseTime = pulseDelay;

        SetAlpha(0);

        SaveManager.OnUpgradeChanged += LoadSacrifices;
        LoadSacrifices();
    }

    private void OnDestroy()
    {
        SaveManager.OnUpgradeChanged -= LoadSacrifices;
    }

    void LoadSacrifices()
    {
        // Disable pulsing if the sacrifice was made
        if (SaveManager.SacrificeMade("sacrifice_pulse"))
        {
            pulseActive = false;

            // Restore pulse images to their original colour
            foreach (Graphic image in pulseImages)
            {
                Color target = Color.white;
                target.a = image.color.a;
                image.color = target;
            }
        }

        // Disable the functionality of the compass if the sacrifice was made
        if (SaveManager.SacrificeMade("sacrifice_compass"))
        {
            sacrificedCompass = true;
            // If the pulse was also sacrificed, completely disable the gameobject
            if (!pulseActive) gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GravitySource tempPlanet = player.GetNearestSource();

        if (tempPlanet == null || !active)      // Fade out the compass if it is inactive or if the player is in space
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

        if (sacrificedCompass) return;

        float minDist = 1000;
        int minIndex = -1;

        // Find the distance of the compass item closest to the middle of the compass
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
                // Enable the distance text
                distanceText.enabled = true;
                Color colour = distanceText.color;
                colour.a = 1;
                distanceText.color = colour;
            }

            // Display the distance to the compass item closest to the middle of the compass
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

        Pulse();
    }

    // Function to find the rotation for the compass direction indicators
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

    // Function to find the angle between the player and the item
    // Sets the item's compass image to the correct position
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

    // Function to fade all of the images in
    void FadeIn()
    {
        float alpha = images[0].color.a;
        if(alpha < 1)
        {
            alpha = Mathf.Clamp01(alpha + Time.deltaTime * fadeSpeed);
            SetAlpha(alpha);
        }
    }

    // Function to fade out all of the images
    void FadeOut()
    {
        float alpha = images[0].color.a;
        if (alpha > 0)
        {
            alpha = Mathf.Clamp01(alpha - Time.deltaTime * fadeSpeed * 2);
            SetAlpha(alpha);
        }
    }

    void Pulse()
    {
        if (!pulseActive) return;

        if(nearObjects.Count > 0)
        {
            float distance = NearestDistance();                         // Find the closest object
            pulseDelay = Mathf.Clamp01((distance - 2) / 500) * 5;       // Work out the pulse rate
            if (pulseTime > pulseDelay) pulseTime = pulseDelay;
        }
        else
        {
            pulseDelay = -1;
        }

        if(pulseTime < 0)
        {
            if(pulseTime != -450 && pulseDelay >= 0)            // Make the pulse images change from white to orange
            {
                pulsePercent += Time.deltaTime * 3;

                if (pulsePercent >= 1) pulseTime = -450;
            }
            else if (pulsePercent > 0)                          // Make the pulse images change from orange to white
            {
                pulsePercent -= Time.deltaTime * 3;

                if (pulsePercent <= 0) pulseTime = pulseDelay;
            }

            // Change the colour of all of the pulse images
            foreach (Graphic image in pulseImages)
            {
                Color target = Color.Lerp(Color.white, pulseColour, pulsePercent);
                target.a = image.color.a;
                image.color = target;
            }
        }
        else if(pulseDelay >= 0)
        {
            pulseTime -= Time.deltaTime;            // Count down until the next pulse
            if(pulseTime <= 0)                      // If it is time to pulse
            {
                audioManager.PlaySound("pulse");    // Play the pulse sound effect
            }
        }
    }

    // Function to find the shortest distance to a nearby object
    float NearestDistance()
    {
        float dist = 10000;
        foreach(Transform transform in nearObjects)
        {
            float newDist = (playerT.position - transform.position).sqrMagnitude;
            if (newDist < dist) dist = newDist;
        }

        return dist;
    }

    void SetAlpha(float percent)
    {
        foreach(Graphic image in images)
        {
            Color colour = image.color;
            colour.a = percent;
            image.color = colour;
        }
    }

    public static void SetActive(bool value)
    {
        active = value;
        if (!active && Instance != null)
        {
            Instance.nearObjects.Clear();
        }
    }

    // Disable compass items if they are on a planet that the player is not on
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

    // Function to add a new item to the compass
    public static void AddItem(CompassItem item)
    {
        if (Instance == null || Instance.sacrificedCompass) return;

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

    public static void AddNearObject(Transform nearObject)
    {
        if (Instance == null || !Instance.pulseActive) return;

        if (!Instance.nearObjects.Contains(nearObject)) Instance.nearObjects.Add(nearObject);
    }

    public static void RemoveNearObject(Transform nearObject)
    {
        if (Instance == null) return;

        if (Instance.nearObjects.Contains(nearObject)) Instance.nearObjects.Remove(nearObject);
    }
}

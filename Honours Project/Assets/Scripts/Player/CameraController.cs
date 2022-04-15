using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float rotSpeed;

    AtmosphereRenderer atmosphere;
    CloudRenderer cloud;

    int layerMask = 1 << 8 | 1 << 14;

    private void Awake()
    {
        atmosphere = GetComponent<AtmosphereRenderer>();
        cloud = GetComponent<CloudRenderer>();
    }

    private void Start()
    {
        SettingsManager.OnChangesMade += LoadAtmosphereSetting;
        LoadAtmosphereSetting();
    }

    private void OnDestroy()
    {
        SettingsManager.OnChangesMade -= LoadAtmosphereSetting;
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

    public void LoadAtmosphereSetting()
    {
        if (PlayerPrefs.HasKey("Atmosphere"))
        {
            bool active = PlayerPrefs.GetInt("Atmosphere") == 1;
            atmosphere.enabled = active;
            cloud.enabled = active;
        }
    }

    public Vector3 UpdatePlanetHUD(Rigidbody rb)
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 50000, layerMask))
        {
            Vector3 objectVelocity = hitInfo.rigidbody.velocity;

            float relativeVel = Vector3.Distance(rb.velocity, objectVelocity);

            string planetName = hitInfo.rigidbody.name;
            Vector3 planetPos = hitInfo.rigidbody.position;

            if (hitInfo.collider.name == "Arturius") {
                planetName = "Arturius";
                planetPos = hitInfo.collider.transform.position;
            }

            float distance = Vector3.Distance(rb.position, planetPos);

            if (Vector3.Dot(rb.velocity - objectVelocity, rb.position - planetPos) > 0) relativeVel *= -1;

            HUD.SetPlanetDetails(planetName, relativeVel, distance);

            return objectVelocity;
        }
        else
        {
            HUD.SetPlanetTextActive(false);
        }

        return rb.velocity;
    }
}

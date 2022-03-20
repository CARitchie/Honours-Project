using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLightControl : MonoBehaviour
{
    [SerializeField] Transform directionalLight;
    [SerializeField] float angle;
    [SerializeField] LitObject[] litObjects;

    static GlobalLightControl Instance;

    Transform[] player = new Transform[2];
    int index = 0;

    Vector3 playerDir;

    void Awake()
    {
        Instance = this;

        for(int i = 0; i < litObjects.Length; i++)
        {
            litObjects[i].Initialise();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        player[0] = PlayerController.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        playerDir = (player[index].position - transform.position).normalized;

        LookAtPlayer();

        UpdateObjects();
    }

    void LookAtPlayer(){
        directionalLight.forward = playerDir;
    }

    public static void SwitchToPlayer(){
        if(Instance == null) return;

        Instance.index = 0;
    }
        
    public static void SwitchToShip(Transform ship){
        if(Instance == null) return;

        Instance.player[1] = ship;
        Instance.index = 1;
    }

    void UpdateObjects()
    {
        for(int i = 0; i < litObjects.Length; i++)
        {
            UpdateObject(i);
        }
    }

    void UpdateObject(int index)
    {
        float dot = Vector3.Dot(playerDir, (litObjects[index].renderer.transform.position - transform.position).normalized);

        if (dot > angle) litObjects[index].RestoreLayer();
        else litObjects[index].SetLayer(14);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.TryGetComponent(out PersonDetails details))
        {
            details.TakeDamage(100000000);
        }else if(other.attachedRigidbody.TryGetComponent(out ShipController ship))
        {
            if (ship.IsActive())
            {
                SceneManager.FadeToScene("Space");
            }
        }
    }

    [System.Serializable]
    public struct LitObject
    {
        public GameObject renderer;
        int originalLayer;

        public void Initialise()
        {
            originalLayer = renderer.layer;
        }

        public void SetLayer(int newLayer)
        {
            if (newLayer == renderer.layer) return;
            renderer.layer = newLayer;
        }

        public void RestoreLayer()
        {
            if (renderer.layer == originalLayer) return;
            renderer.layer = originalLayer;
        }
    }
}

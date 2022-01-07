using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLightControl : MonoBehaviour
{
    [SerializeField] Transform directionalLight;

    static GlobalLightControl Instance;

    Transform[] player = new Transform[2];
    int index = 0;

    void Awake()
    {
        Instance = this;
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
        LookAtPlayer();
    }

    void LookAtPlayer(){
        directionalLight.forward = player[index].position - directionalLight.position;
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLightControl : MonoBehaviour
{
    [SerializeField] Transform directionalLight;

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        LookAtPlayer();
    }

    void LookAtPlayer(){
        directionalLight.forward = player.position - directionalLight.position;
    }
}

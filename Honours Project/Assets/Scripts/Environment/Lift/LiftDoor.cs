using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftDoor : MonoBehaviour
{
    [SerializeField] Lift lift;
    [SerializeField] int index;
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        lift.FloorSelected += CloseDoor;
    }

    private void OnDestroy()
    {
        if(lift != null) lift.FloorSelected -= CloseDoor;
    }

    public void CloseDoor()
    {
        anim.SetBool("Open", false);
    }

    public void OpenDoor()
    {
        anim.SetBool("Open", true);
    }

    public void OnClose()
    {
        lift.StartMove(index);
    }
}

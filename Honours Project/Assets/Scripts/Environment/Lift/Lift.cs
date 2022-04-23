using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lift : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int currentIndex;
    [SerializeField] LiftFloor[] floors;

    List<Rigidbody> bodies = new List<Rigidbody>();

    int targetIndex;

    bool moving = false;

    public delegate void LiftAction();
    public event LiftAction FloorSelected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;

        if (other.attachedRigidbody != null)
        {
            if (!bodies.Contains(other.attachedRigidbody)) bodies.Add(other.attachedRigidbody);     // Add any rigidbodies that enter the lift to a list
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody == null) return;

        if (other.attachedRigidbody != null)
        {
            if (bodies.Contains(other.attachedRigidbody)) bodies.Remove(other.attachedRigidbody);   // Remove any rigidbodies that exit the lift from a list
        }
    }

    private void Start()
    {
        transform.localPosition = floors[currentIndex].Position;
    }

    // Function to determine which floor should be moved to
    public void MoveToTarget(bool up)
    {
        if (up) targetIndex = currentIndex - 1;
        else targetIndex = currentIndex + 1;

        if(targetIndex >= floors.Length)
        {
            targetIndex = floors.Length - 1;
            return;
        }else if(targetIndex < 0)
        {
            targetIndex = 0;
            return;
        }

        if (!moving)
        {
            moving = true;
            FloorSelected?.Invoke();
            if (floors[currentIndex].Door == null) StartCoroutine(Move());
        }
    }

    public void StartMove(int index)
    {
        if(index == currentIndex) StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        moving = true;
        while(transform.localPosition != floors[targetIndex].Position)
        {
            Vector3 initial = transform.position;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, floors[targetIndex].Position, Time.deltaTime * speed);   // Move towards the target
            Vector3 offset = transform.position - initial;

            for (int i = 0;i < bodies.Count; i++)
            {
                if (bodies[i] != null)
                {
                    bodies[i].transform.position += offset;         // Change the position of the rigidbodies by the distance that the lift moved this frame
                }
            }

            yield return new WaitForEndOfFrame();
        }
        currentIndex = targetIndex;
        moving = false;
        floors[currentIndex].OpenDoor();
    }

    public void Summon(int index)
    {
        if (index == currentIndex && index == targetIndex) return;

        targetIndex = index;

        if (!moving)
        {
            moving = true;
            FloorSelected?.Invoke();
            if (floors[currentIndex].Door == null) StartCoroutine(Move());
        }
    }
}

[System.Serializable]
public class LiftFloor
{
    [SerializeField] Vector3 position;
    [SerializeField] LiftDoor door;

    public Vector3 Position { get { return position; } }
    public LiftDoor Door { get { return door; } }

    public void OpenDoor()
    {
        if (door != null) door.OpenDoor();
    }
}

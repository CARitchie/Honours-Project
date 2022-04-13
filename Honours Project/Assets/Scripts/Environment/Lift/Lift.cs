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
        if(other.attachedRigidbody != null)
        {
            if (!bodies.Contains(other.attachedRigidbody)) bodies.Add(other.attachedRigidbody);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            if (bodies.Contains(other.attachedRigidbody)) bodies.Remove(other.attachedRigidbody);
        }
    }

    private void Start()
    {
        transform.localPosition = floors[currentIndex].Position;
    }

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
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, floors[targetIndex].Position, Time.deltaTime * speed);
            Vector3 offset = transform.position - initial;

            for (int i = 0;i < bodies.Count; i++)
            {
                if (bodies[i] != null)
                {
                    bodies[i].transform.position += offset;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidGroup : MonoBehaviour
{
    [SerializeField] GameObject boidObject;
    [SerializeField] int numberOfBoids;
    [SerializeField] float boidSpeed;
    [SerializeField] float separationSpeed;
    [SerializeField] float alignmentSpeed;
    [SerializeField] float cohesionSpeed;
    [SerializeField] float avoidanceSpeed;
    [SerializeField] bool visualise;
    [SerializeField] Transform planet;
    [SerializeField] float planetRadius;
    [SerializeField] float atmosphereRadius;

    GameObject[] boids;
    float pr;
    float ar;

    void Awake(){
        pr = planetRadius * planetRadius;
        ar = atmosphereRadius * atmosphereRadius;

        boids = new GameObject[numberOfBoids];
        for(int i = 0 ; i < boids.Length; i++){
            boids[i] = Instantiate(boidObject,transform);
            boids[i].transform.localPosition = Random.insideUnitSphere * 5;
            boids[i].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject boid in boids){
            boid.transform.position += boid.transform.forward * boidSpeed * Time.deltaTime;

            List<GameObject> localBoids = CloseBoids(boid);

            if(localBoids != null && localBoids.Count > 0){
                Vector3 closest = ClosestPosition(localBoids, boid) - boid.transform.position;
                Vector3 heading = AverageHeading(localBoids);
                Vector3 targetDirection = AveragePosition(localBoids) - boid.transform.position;

                Vector3 newDirection = boid.transform.forward;
            
                if(closest.sqrMagnitude < 3) {
                    newDirection = Vector3.RotateTowards(newDirection, -closest, separationSpeed * Time.deltaTime, 0.0f);
                    if(visualise)Debug.DrawLine(boid.transform.position,boid.transform.position + newDirection, Color.red);
                }
                
                newDirection = Vector3.RotateTowards(newDirection, heading, alignmentSpeed * Time.deltaTime, 0.0f);
                newDirection = Vector3.RotateTowards(newDirection, targetDirection, cohesionSpeed * Time.deltaTime, 0.0f);
                
                if(visualise){
                    Debug.DrawLine(boid.transform.position,boid.transform.position + heading, Color.blue);
                    Debug.DrawLine(boid.transform.position,boid.transform.position + targetDirection, Color.green);
                }

                float height = (planet.position - boid.transform.position).sqrMagnitude;
                if(height > ar){
                    newDirection = Vector3.RotateTowards(newDirection, planet.position - boid.transform.position, avoidanceSpeed * Time.deltaTime, 0.0f);
                }else if(height < pr){
                    newDirection = Vector3.RotateTowards(newDirection, -(planet.position - boid.transform.position), avoidanceSpeed * Time.deltaTime, 0.0f);
                }

                boid.transform.rotation = Quaternion.LookRotation(newDirection);
            } 

        }
    }

    List<GameObject> CloseBoids(GameObject targetBoid){
        List<GameObject> closeBoids = new List<GameObject>();
        foreach(GameObject boid in boids){
            if(boid != targetBoid){
                if((boid.transform.position - targetBoid.transform.position).sqrMagnitude < 100){
                    closeBoids.Add(boid);
                }
            }
        }
        return closeBoids;
    }

    Vector3 AverageHeading(List<GameObject> targetBoids){
        Vector3 totalHeading = Vector3.zero;
        foreach(GameObject boid in targetBoids){
            totalHeading += boid.transform.forward;
        }

        return totalHeading / targetBoids.Count;
    }

    Vector3 AveragePosition(List<GameObject> targetBoids){
        Vector3 totalPosition = Vector3.zero;
        foreach(GameObject boid in targetBoids){
            totalPosition += boid.transform.position;
        }

        return totalPosition / targetBoids.Count;
    }

    Vector3 ClosestPosition(List<GameObject> localBoids, GameObject currentBoid){
        float minDist = (localBoids[0].transform.position - currentBoid.transform.position).sqrMagnitude;
        GameObject closestBoid = localBoids[0];
        for(int i = 1; i < localBoids.Count;i++){
            float dist = (localBoids[i].transform.position - currentBoid.transform.position).sqrMagnitude;
            if(dist < minDist){
                minDist = dist;
                closestBoid = localBoids[i];
            }
        }

        return closestBoid.transform.position;
    }
}

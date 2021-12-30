using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMesh : MonoBehaviour
{
    [SerializeField][Range(0,100000)] int numberOfPoints = 30;
    [SerializeField] float planetRadius;
    [SerializeField] bool drawRays;
    [SerializeField] Transform closeObject;
    Vector3 closePoint;

    Vector3[] meshPoints;

    public void GenerateMesh(){
        meshPoints = GeneratePoints(numberOfPoints);
    }

    void OnValidate(){
        GenerateMesh();
        closePoint = ClosestPoint(closeObject.position);
        Debug.Log("CLOSE POINT: " +  closePoint);
    }

    // https://stackoverflow.com/questions/9600801/evenly-distributing-n-points-on-a-sphere
    Vector3[] GeneratePoints(int numberOfPoints){
        Vector3[] points = new Vector3[numberOfPoints];
        float phi = Mathf.PI * (3.0f - Mathf.Sqrt(5.0f));  // golden angle in radians

        for(int i = 0 ; i < numberOfPoints; i++){
            float y = 1 - (i / (float)(numberOfPoints - 1)) * 2;  // y goes from 1 to -1
            float radius = Mathf.Sqrt(1 - y * y);  // radius at y

            float theta = phi * i;  // golden angle increment

            float x = Mathf.Cos(theta) * radius;
            float z = Mathf.Sin(theta) * radius;

            Vector3 initialPoint = new Vector3(x,y,z);

            points[i] = FindPointOnSurface(initialPoint);    
        }
        return points;
    }

    Vector3 FindPointOnSurface(Vector3 direction){
        Vector3 origin = transform.position + direction * 200;

        Vector3 point = Vector3.zero;

        if(Physics.Raycast(origin, -direction, out RaycastHit hit, 1000,1<<8)){
            point = hit.point;
        }else{
            point = direction * planetRadius + transform.position;
            Debug.Log("Not Found");
        }

        if(drawRays) Debug.DrawRay(transform.position, point - transform.position,Color.yellow,10);

        return point;
    }

    Vector3 ClosestPoint(Vector3 position){
        float distance = 10000;
        Vector3 point = Vector3.zero;
        for(int i = 0 ; i < meshPoints.Length;i++){
            float gap = (position - meshPoints[i]).sqrMagnitude;
            if(gap < distance){
                distance = gap;
                point = meshPoints[i];
                if(gap < 2) return point;
            }
        }

        if(distance != 10000){
            return point;
        }else{
            return Vector3.zero;
        }
        
    }

    void OnDrawGizmosSelected(){
        if(meshPoints != null && meshPoints.Length > 0){
            foreach(Vector3 point in meshPoints){
                if(point != closePoint){
                    Gizmos.color = new Color(1,0,0,0.5f);
                }else{
                    Gizmos.color = Color.yellow;
                }
                
                Gizmos.DrawSphere(point, 1);
            }
        }else{
            GenerateMesh();
        }
    }
}

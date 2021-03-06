using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    [SerializeField] float height;
    [SerializeField] Transform planet;
    [SerializeField] Transform targetObject;
    [SerializeField] bool visualise;
    [SerializeField] int maxIterations = 1000;

    List<Node> openList = new List<Node>();
    List<Node> closedList = new List<Node>();
    Vector3 target;
    bool inViewGoodEnough = false;
    int inViewCount = 0;

    float straightCost = 10;
    float diagonalCost = 14;

    Node currentNode = null;
    List<Vector3> completed = new List<Vector3>();

    Vector3 originalPos;

    int layerMask = (1 << 0 | 1 << 8 | 1 << 9 | 1 << 10);

    void Start(){

        // If the process is to be visualised
        if(visualise && targetObject != null){
            StartCoroutine(VisualiseProcess(targetObject.position));
        }
    }

    // Function to return a stack of positions that represent a path
    public Stack<Vector3> FindPath(Vector3 target, Transform planet, bool inViewGoodEnough){

        if (planet == null) return null;                                                    // If there is no planet, then a path cannot be found

        this.target = target;
        this.planet = planet;
        this.inViewGoodEnough = inViewGoodEnough;

        openList = new List<Node>();
        closedList = new List<Node>();
        originalPos = planet.position;

        Vector3 start = transform.position;
        Vector3 dir = transform.forward;

        if (inViewGoodEnough)
        {
            Vector3[] newTarget = FindPosition(target, transform.forward, 30);              // Instead of using the target's actual position, reduce its height to match the rest of the nodes
            if (newTarget != null) this.target = newTarget[0];
        }

        Vector3[] originData = FindPosition(start, dir);                                    // Convert the starting position into node data
        if(originData == null) return null;
        openList.Add(new Node(null, originData, 0, CalculateHCost(originData[0])));         // Create an origin node
        
        int counter = 3000;
        bool found = false;

        while(!found){
            currentNode = FindSmallestCost();                                               // The current node should be the node in the open list with the smallest cost
            if(currentNode == null) return null;

            /*
            if(inViewGoodEnough && TargetInView(currentNode.Position))
            {
                inViewCount++;
                if(inViewCount >= 3)
                {
                    inViewCount = 0;
                    counter = 0;
                }
            }
            */

            if(AddToClosed(currentNode) || counter == 0){                                   // If the current node is close enough to the target, or too many loops have occured
                found = true;                                                               // Stop looping
            }else{
                FindAdjacent(currentNode);                                                  // Otherwise, find all of the nodes adjacent to the current node
            }
            counter--;
        }

        Stack<Vector3> path = new Stack<Vector3>();

        while(currentNode.Parent != null){                                                  // Go back through all of the current node's parents to create the full path
            path.Push(currentNode.Position - planet.position);
            currentNode = currentNode.Parent;
        }

        return path;
    }

    // Function to determine whether the player is within view from a specific position
    bool TargetInView(Vector3 current)
    {
        Vector3 point1 = current;
        Vector3 point2 = target;

        RaycastHit[] hits = Physics.RaycastAll(point1, point2 - point1, Vector3.Distance(point1, point2), layerMask);

        if (hits == null || hits.Length < 1) return false;                                      // Return false if there are no collisions

        if (hits.Length == 1 && hits[0].transform.CompareTag("Player")) return true;            // Return true if only the player is collided with

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.layer != 10) return false;                          // Return false if anything not on the character layer is collided with
        }

        return true;
    }

    // Function that also finds a path
    // Done so with delays so that the process can be visualised in the editor
    IEnumerator VisualiseProcess(Vector3 target){
        this.target = target;
        openList = new List<Node>();
        closedList = new List<Node>();
        originalPos = planet.position;

        Vector3 start = transform.position;
        Vector3 dir = transform.forward;

        Vector3[] originData = FindPosition(start, dir);
        if(originData == null) yield return null;
        openList.Add(new Node(null, originData, 0, CalculateHCost(originData[0])));
        
        int counter = maxIterations;
        bool found = false;

        while(!found){

            currentNode = FindSmallestCost();
            if(currentNode == null) yield return null;
            // Add code here to account for first visible node
            if(AddToClosed(currentNode) || counter == 0){
                found = true;
            }else{
                FindAdjacent(currentNode);
            }
            counter--;
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }

        while(currentNode.Parent != null){
            completed.Add(currentNode.Position - planet.position);
            currentNode = currentNode.Parent;

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }
    }

    // Function to find all of the nodes adjacent to the specified node
    void FindAdjacent(Node node){
        CreateNode(node, node.Position + node.Forward, straightCost);
        CreateNode(node, node.Position + node.Forward + node.Right, diagonalCost);
        CreateNode(node, node.Position + node.Right, straightCost);
        CreateNode(node, node.Position + node.Right - node.Forward, diagonalCost);
        CreateNode(node, node.Position - node.Forward, straightCost);
        CreateNode(node, node.Position - node.Forward - node.Right, diagonalCost);
        CreateNode(node, node.Position - node.Right, straightCost);
        CreateNode(node, node.Position - node.Right + node.Forward, diagonalCost);    
    }

    // Function to find the node in the open list with the smallest cost
    Node FindSmallestCost(){
        if(openList == null || openList.Count < 1) return null;

        Node min = openList[0];
        for(int i = 1 ; i < openList.Count;i++){
            if(openList[i].FCost < min.FCost) min = openList[i];
        }

        return min;
    }

    // Function to create a node
    void CreateNode(Node parent, Vector3 position, float cost){
        if(Physics.CheckSphere(position,0.5f,1 << 9)) return;                                           // Cancel creation if there is an obstacle in the desired position

        Vector3[] positionData = FindPosition(position, parent.Forward);
        if(positionData == null) return;

        cost = (positionData[0] - parent.Position).sqrMagnitude;

        if(positionData != null){
            Node newNode = new Node(parent, positionData, cost, CalculateHCost(positionData[0]));

            if(!InClosedList(newNode)){
                if(!InOpenList(newNode)){
                    openList.Add(newNode);                                                              // If the new node doesn't already exist, add it to the open list
                }
            }
        }else{
            Debug.LogError("Node position data not found");
        }
    }

    // Function to determine whether a node exists within the closed list
    bool InClosedList(Node node){
        foreach(Node closedNode in closedList){
            if(node.Equals(closedNode)) return true;
        }
        return false;
    }

    // Function to determine whether a node exists within the open list
    bool InOpenList(Node node){
        foreach(Node openNode in openList){
            if(node.Equals(openNode)){
                if(node.GCost < openNode.GCost){                // If the node is already in the open list but has a smaller cost
                    openNode.SetParent(node.Parent);            // Replace the existing node's details with the faster details
                    openNode.SetGCost(node.GCost);
                    openNode.SetFCost(node.FCost);
                }
                return true;
            } 
        }
        return false;
    }

    bool AddToClosed(Node node){
        openList.Remove(node);
        closedList.Add(node);
        return (target - node.Position).sqrMagnitude < 4;
    }

    // Function to convert position data into a node
    public Vector3[] FindPosition(Vector3 position, Vector3 oldForward, float distance = 3){
        if(!Physics.Raycast(position, planet.position - position, out RaycastHit hit, distance, 1 << 8)){
            return null;
        }

        Vector3 direction = (hit.point - planet.position).normalized;       // Get the normalized direction from the planet

        Vector3 newPosition = hit.point + direction * height;               // Create the new position
        Vector3 right = Vector3.Cross(direction, oldForward);
        Vector3 forward = Vector3.Cross(right, direction);
        

        return new Vector3[] {newPosition,forward,right};
    }

    float CalculateHCost(Vector3 position){
        return (position - target).sqrMagnitude;
    }

    void OnDrawGizmosSelected(){
        if(planet == null) return;
        Vector3 offset = planet.position - originalPos;

        // Make all of the nodes visible in the editor
        foreach(Node node in openList){
            if(node == currentNode){
                Gizmos.color = new Color(1,0,0,0.5f);
                
            }else{
                Gizmos.color = new Color(0,0,1,0.5f);
            }
            Gizmos.DrawSphere(node.Position + offset,0.5f);
        }

        foreach(Node node in closedList){
            if(node == currentNode){
                Gizmos.color = new Color(1,0,0,0.5f);
                
            }else{
                Gizmos.color = new Color(0,1,0,0.5f);
            }
            Gizmos.DrawSphere(node.Position + offset,0.5f);
        }

        // Draw a red line indicating the final path
        Gizmos.color = Color.red;
        for(int i = 0 ; i < completed.Count - 1 ; i++){
            Gizmos.DrawLine(completed[i] + planet.position,completed[i+1] + planet.position);
        }

        Gizmos.DrawSphere(target, 0.5f);
    
    }
}

class Node{
    Vector3 position;
    Vector3 forward;
    Vector3 right;
    Node parent;

    float gCost = 0;
    float fCost = 0;

    public Node(Node parent, Vector3[] positionData, float cost, float hCost){
        this.position = positionData[0];
        this.forward = positionData[1];
        this.right = positionData[2];

        this.parent = parent;

        if(parent != null){
            gCost = parent.gCost + cost;
        }
        
        fCost = gCost + hCost;
    }

    public Vector3 Position {get{return position;}}
    public Vector3 Forward {get{return forward;}}
    public Vector3 Right {get{return right;}}

    public Node Parent {get{return parent;}}

    public float GCost{get{return gCost;}}
    public float FCost{get{return fCost;}}

    public bool Equals(Node node){
        return (position - node.position).sqrMagnitude < 0.09f;
    }

    public void SetParent(Node node){
        this.parent = node;
    }

    public void SetGCost(float cost){
        gCost = cost;
    }

    public void SetFCost(float cost){
        fCost = cost;
    }
}

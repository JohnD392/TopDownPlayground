using UnityEngine;
using System.Collections.Generic;

public class SmartFleeMovement : MonoBehaviour {
    public List<string> tagsToFleeFrom;
    public bool runBool;

    Stack<Vector3> path;
    Vector3? nextPathPosition;

    float moveSpeed = 3f;

    private float arrivalDistance = 1f;

    bool isMoving = false;

    public void Start() {
        path = null;
        Pathfind();
    }

    public void Update() {
        // Pathfinding
        if (runBool) {
            runBool = false;
            Pathfind();
        }

        if(nextPathPosition != null) {
            transform.position += ((Vector3)nextPathPosition - transform.position).normalized * moveSpeed * Time.deltaTime;
            if(Vector3.Distance(transform.position, (Vector3)nextPathPosition) < arrivalDistance) {
                if (path.Count > 0) {
                    nextPathPosition = path.Pop();
                } else {
                    nextPathPosition = null;
                }
            }
        }
    }

    private void Pathfind() {
        AStarGeneral asg = new AStarGeneral();
        path = asg.Run(transform.position, new Vector3(-20f, 0f, 20f));
        Debug.Log("Path length: " + path.Count);
        nextPathPosition = path.Pop();
    }
}

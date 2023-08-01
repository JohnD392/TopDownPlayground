using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AStarGeneral {
    List<AStarNode> openList;
    List<AStarNode> closedList;
    
    public Vector3 endPosition;
    Vector3 startPosition;

    public float stepDistance;

    int iteration = 0;
    int maxIterations = 1000;

    AStarNode solution = null;

    float finalDistance = 1f;
    public bool isRunning = false;

    public bool fireBool = false;

    Vector3[] possibleDirections = new Vector3[] {
        new Vector3(1f,0f,-1f),
        new Vector3(1f,0f,0f),
        new Vector3(1f,0f,1f),
        new Vector3(0f,0f,-1f),
        new Vector3(0f,0f,1f),
        new Vector3(-1f,0f,-1f),
        new Vector3(-1f,0f,0f),
        new Vector3(-1f,0f,1f)
    };
    
    public Stack<Vector3> Run(Vector3 startPosition, Vector3 endPosition) {
        AStarInitialization(startPosition, endPosition);
        return AStar();
    }

    public Stack<Vector3> Path() {
        if(solution == null) {
            return null;
        } else {
            return StraightPath(solution);
        }
    }

    // Distance between current node and start node
    float G(Vector3 currentPosition) {
        return Vector3.Distance(currentPosition, startPosition);
    }

    //H is the heuristic
    float H(Vector3 currentPosition) {
        return Vector3.Distance(currentPosition, endPosition);
    }

    // Cost of the node
    float F(Vector3 currentPosition) {
        return H(currentPosition) + G(currentPosition);
    }

    void AStarInitialization(Vector3 startPosition, Vector3 endPosition) {
        //initialize the search
        this.startPosition = startPosition;
        this.endPosition = endPosition;
        iteration = 0;
        openList = new List<AStarNode>();
        closedList = new List<AStarNode>();
        openList.Add(new AStarNode(null, startPosition));
    }

    Stack<Vector3> AStar() {
        //start the search
        AStarNode s = AStarLoop();
        //Draw the solution path
        if(s==null) return null;
        DrawFullPath(s);
        Stack<Vector3> path = StraightPath(s);
        return path;
    }

    // takes an AStarNode and returns a stack of Vector3s that can be pathed to in a straighter manner
    Stack<Vector3> StraightPath(AStarNode root) {
        Stack<Vector3> path = new Stack<Vector3>();
        path.Push(root.vec);
        int layerMask = 1 << 7;
        if(root == null || root.parent == null) return path;
        AStarNode currentNode = root;
        AStarNode lastNode = root;
        AStarNode nextNode = root.parent;
        while(nextNode!=null) {
            RaycastHit hit;
            if(Physics.Raycast(currentNode.vec, nextNode.vec - currentNode.vec, out hit, (nextNode.vec - currentNode.vec).magnitude, layerMask)) {
                //There is a collider between the current node and the next node
                Debug.DrawLine(currentNode.vec, lastNode.vec, Color.white, 3f);
                path.Push(lastNode.vec);
                currentNode = lastNode;
            }
            lastNode = nextNode;
            nextNode = lastNode.parent;
            if(nextNode == null) {
                Debug.DrawLine(currentNode.vec, lastNode.vec, Color.white, 3f);
                path.Push(lastNode.vec);
                return path;
            }
        }
        return path;
    }

    void DrawFullPath(AStarNode root) {
        Debug.Log(root.vec);
        while(root.parent!=null) {
            Debug.Log("Parent!");
            Debug.DrawLine(root.vec, root.parent.vec, Color.blue, 3f);
            root = root.parent;
        }
    }

    AStarNode AStarLoop() {
        iteration++;
        Debug.Log("Iteration: " + iteration);
        Debug.Log("Current open list count: " + openList.Count);
        if(iteration > maxIterations) {
            Debug.Log("Max iterations. Aborting pathfind.");
            return null;
        }

        //Take from the open list the node node_current with the lowest F
        float lowestF = 100000f;
        AStarNode currentNode = null;
        foreach(AStarNode node in openList) {
            float f = F(node.vec);
            if(f < lowestF) {
                currentNode = node;
                lowestF = f;
            }
        }
        if(currentNode == null) {
            Debug.Log("No Solution Found.");
            return null;
        } else {
            openList.Remove(currentNode);
        }
        //if node_current is node_goal we have found the solution. return.
        if(Vector3.Distance(currentNode.vec, endPosition) < finalDistance) {
            Debug.Log("Solution found.");
            return currentNode;
        }
        // Generate each state node_successor that come after node_current
        int layerMask = 1 << 7;
        foreach(Vector3 direction in possibleDirections) {
            RaycastHit hit;
            if(Physics.Raycast(currentNode.vec, direction, out hit, direction.magnitude, layerMask)) {
                //Collides with terrain. Ignore this path
                Debug.DrawLine(currentNode.vec, currentNode.vec + direction, Color.red, 3f);
                continue;
            }
            AStarNode child = new AStarNode(currentNode, currentNode.vec + direction);
            if(NodeListContains(openList, child)) {
                // open list already contains child. we can optimize here for shorter paths i think
            } else if(NodeListContains(closedList, child)) {
                //more optimization
            } else {
                openList.Add(child);
            }
        }
        closedList.Add(currentNode);
        return AStarLoop();
    }

    bool NodeListContains(List<AStarNode> l, AStarNode node) {
        foreach(AStarNode listNode in l) {
            if(Vector3.Distance(listNode.vec, node.vec) < .1f) return true;
        }
        return false;
    }

    public class AStarNode {
        public AStarNode parent;
        public Vector3 vec;

        public AStarNode(AStarNode parent, Vector3 vec) {
            this.parent = parent;
            this.vec = vec;
        }
    }
}

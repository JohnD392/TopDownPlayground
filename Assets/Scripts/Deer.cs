using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Deer : MonoBehaviour {
    AStarGeneral aStar;
    Vision vision;
    GameObject terrainGeneratorObj;

    public bool test = false;
    bool isFleeing = false;
    Vector3? currentDestination = null;
    public float runSpeed = 10f;

    void Start() {
        aStar = GetComponent<AStarGeneral>();
        vision = GetComponentInChildren<Vision>();
        terrainGeneratorObj = GameObject.Find("Terrain Generator");
    }

    void FixedUpdate() {
        if(aStar.isRunning) {
            //The pathing algorithm is running. let it complete
            return;
        }
        if(isFleeing) {
            // let the initial pathing complete
            return;
        }
        if(vision.seesPlayer) {
            //RunAwayFrom(vision.playersInRange[0].transform.position);
        }
    }

    IEnumerator RunCoroutine(Stack<Vector3> path) {
        isFleeing = true;
        while(path.Count > 0) {
            currentDestination = path.Pop();
            Debug.Log("Current destination: " + currentDestination);
            Debug.DrawLine(transform.position, (Vector3)currentDestination, Color.blue, 3f);
            while(Vector3.Distance(transform.position, (Vector3)currentDestination) > .2f) {
                Vector3 dir = ((Vector3)currentDestination - transform.position).normalized;
                GetComponent<Rigidbody>().velocity = dir * runSpeed;
                yield return new WaitForSeconds(.05f);
            }
        }
        isFleeing = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        yield break;
    }

    //void RunAwayFrom(Vector3 enemyPosition) {
    //    float runDistance = 12f;
    //    Vector3 oppositeDirection = -(enemyPosition - transform.position).normalized;
    //    oppositeDirection.y = 0f;

    //    Vector3 targetPosition = oppositeDirection * runDistance;
    //    targetPosition.y = 0f;

    //    if(!terrainGeneratorObj.GetComponent<TerrainGenerator>().DoesGenerate((int)targetPosition.x, (int)targetPosition.z)){
    //        aStar.endPosition = targetPosition;
    //        Action<Stack<Vector3>> runAction = RunCallback;
    //        aStar.Run(runAction);
    //    } else {
    //        Debug.DrawLine(transform.position, targetPosition, Color.red, 3f);
    //    }
    //}

    // This function is passed to the astargeneral class to be run on pathing completion
    void RunCallback(Stack<Vector3> path) {
        StartCoroutine(RunCoroutine(path));
    }
}

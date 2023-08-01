using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class FleeMovement : MonoBehaviour {
    public List<string> tagsToFleeFrom;
    List<GameObject> objectsInRadius;

    public void Start() {
        objectsInRadius = new List<GameObject>();
    }

    public void Update() {
        Flee();
    }

    private void OrientTransform() {
        Vector3 dir = VectorAwayFromTargets();
        transform.forward = dir;
        transform.LookAt(transform.position + VectorAwayFromTargets(), Vector3.up);
    }
    
    private void Flee() {
        if(objectsInRadius.Count > 0) OrientTransform();
        GetComponent<Rigidbody>().velocity = VectorAwayFromTargets();
    }

    private Vector3 VectorAwayFromTargets() {
        Vector3 vecToStuff = Vector3.zero;
        foreach (GameObject fleeFrom in objectsInRadius) {
            Vector3 dir = fleeFrom.transform.position - transform.position;
            dir.y = 0f;
            vecToStuff += dir;
        }
        //invert the average 
        Debug.DrawLine(transform.position, transform.position + vecToStuff * 4f);
        return -vecToStuff.normalized;
    }

    // on other collisions


    private void OnTriggerEnter(Collider other) {
        if (tagsToFleeFrom.Contains(other.gameObject.tag)) {
            objectsInRadius.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        CleanupObjects();
        if(objectsInRadius.Contains(other.gameObject)) objectsInRadius.Remove(other.gameObject);
    }

    private void CleanupObjects() {
        List<GameObject> removeList = new List<GameObject>();
        objectsInRadius.RemoveAll(isNull);
    }

    private bool isNull(GameObject obj) {
        return obj == null;
    }
}

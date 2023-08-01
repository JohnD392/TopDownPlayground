using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public Vector3 cameraOffset;
    public Transform target;

    void Update() {
        transform.position = target.position + cameraOffset;
        Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
        transform.localRotation = rotation;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Focuser : MonoBehaviour {
    List<GameObject> focusableObjects;
    GameObject focusedObject;
    int focusIndex = 0;
    Gamepad gamepad;

    void Start() {
        focusableObjects = new List<GameObject>();
        gamepad = Gamepad.current;
    }

    void Update() {
        if(gamepad != null) if(gamepad.buttonSouth.wasPressedThisFrame) Cycle();
    }

    void OnTriggerEnter(Collider other) {
        Focusable focusable = other.GetComponent<Focusable>();
        if(focusable == null) {
            return;
        } else {
            if(!focusableObjects.Contains(other.gameObject)) {
                focusableObjects.Add(other.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other) {
        Focusable focusable = other.GetComponent<Focusable>();
        if(focusable == null) {
            Debug.Log("Object: " + other.transform.name + " is not focusable.");
            return;
        } else {
            if(focusableObjects.Contains(other.gameObject)) {
                focusable.Unfocus();
                focusableObjects.Remove(other.gameObject);
            }
        }
    }

    void Cycle() {
        Debug.Log("Cycling through " + focusableObjects.Count + " objects in list.");
        //Cycle through focusable objects
        if(focusableObjects.Count == 0) {
            Debug.Log("No focusable objects in range.");
            return;
        } else {
            focusIndex++;
            if(focusIndex >= focusableObjects.Count) focusIndex = 0;
            if(focusedObject != null) focusedObject.GetComponent<Focusable>().Unfocus();
            focusedObject = focusableObjects[focusIndex];
            focusedObject.GetComponent<Focusable>().Focus();
        }
    }
}

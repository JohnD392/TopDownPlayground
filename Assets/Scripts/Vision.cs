using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour {
    public List<GameObject> playersInRange;
    public float visionWidth = .8f; // References Dot forward
    public bool seesPlayer;

    void Start() {
        playersInRange = new List<GameObject>();
    }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject == null) return;
        if(other.gameObject.tag == "Player") {
            playersInRange.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.gameObject == null) return;
        if(other.gameObject.tag == "Player" && playersInRange.Contains(other.gameObject)) {
            playersInRange.Remove(other.gameObject);
        }
    }

    void FixedUpdate() {
        //TODO can optimize to happen less frequently
        this.seesPlayer = SeesPlayer();
    }

    bool SeesPlayer() {
        int layerMask = 1 << LayerMask.NameToLayer("Enemy");
        layerMask = ~layerMask;
        foreach(GameObject player in playersInRange) {
            RaycastHit hit;
            Vector3 deltaPos = player.transform.position - transform.position;
            if(Physics.Raycast(transform.position, deltaPos, out hit, Vector3.Distance(player.transform.position, transform.position) * 2f, layerMask)) {
                if(hit.transform.tag == "Player") {
                    if(Vector3.Dot(transform.forward, deltaPos.normalized) > 1-visionWidth) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    float VisionRadius() {
        return GetComponent<BoxCollider>().size.x/2f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour {
    Transform target;
    float playerDetectionRadius = 10f;
    LayerMask playerMask;

    private void Awake() {
         playerMask = LayerMask.NameToLayer("Player");
    }

    bool PlayerInRange() {
        return Physics.CheckSphere(transform.position, playerDetectionRadius, playerMask);
    }
}

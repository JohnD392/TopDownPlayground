using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwingAttack : MonoBehaviour {

    private float timeBetweenAttacks;
    private float startTimeBetweenAttacks;

    public Transform attackPos;
    public LayerMask whatIsEnemies;
    public float attackRangeMin;
    public float attackRangeMax;
    public float attackSpeed;
    public float damage;
    public int arcDegrees;
    public Transform playerTransform;


    public void Update() {
        Gamepad gamepad = Gamepad.current;
        if(gamepad == null) {
            return;
        }
        if(gamepad.rightTrigger.isPressed) {
            Debug.Log("Right trigger!");
            Attack();
        }
    }
    public void Attack() {
        if(playerTransform == null) {
            Debug.LogError("No player transform found in SwingAttack.");
            return;
        }
        int halfArc = arcDegrees/2;
        for(int i=-halfArc; i<halfArc; i++) {
            Vector3 offsetStart = DegreesToOffset(attackRangeMin, (float)i);
            Vector3 offsetEnd = DegreesToOffset(attackRangeMax, (float)i);
            Debug.DrawLine(playerTransform.position + offsetStart, playerTransform.position + offsetEnd, Color.red, 3f);
        }
    }

    private Vector3 DegreesToOffset(float distance, float degrees) {
        Vector3 forward = playerTransform.forward * distance;
        float rad = (float)degrees * Mathf.Deg2Rad;
        Vector3 localOffset = new Vector3(
            forward.x * Mathf.Cos(rad) - forward.z * Mathf.Sin(rad),
            0f,
            forward.x * Mathf.Sin(rad) + forward.z * Mathf.Cos(rad)
        );
        return localOffset;
    }
}

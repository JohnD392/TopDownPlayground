using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    public float acceleration;
    public float maxSpeed = 5f;
    public float dashSpeed = 22f;
    public float turnSpeed = .5f;
    public float dashDuration = .4f;
    public float dashCooldown = 3f;
    float lastDashTime = 0f;
    bool isDashing = false;

    void FixedUpdate() {
        // CameraMovement();
        ControllerMovement();
        SpeedLimit();
    }

    void Move(Vector2 moveDirection) {
        GetComponent<Rigidbody>().velocity = transform.rotation * new Vector3(moveDirection.x, 0f, moveDirection.y) * maxSpeed;
    }

    void Turn(Vector2 turnDirection){
        transform.Rotate(new Vector3(0f, turnDirection.x * turnSpeed, 0f), Space.Self);
    }

    void SpeedLimit() {
        if(GetComponent<Rigidbody>().velocity.normalized.magnitude > maxSpeed) {
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized * maxSpeed;
        }
    }

    void ControllerMovement() {
        Gamepad gamepad = Gamepad.current;
        if(gamepad == null) {
            Debug.Log("No gamepad available.");
            return;
        }
        Vector2 turnDir = gamepad.rightStick.ReadValue();
        Turn(turnDir);
        Vector2 moveDir = gamepad.leftStick.ReadValue();
        if(!isDashing) Move(moveDir);
        if(gamepad.buttonEast.isPressed) Dash(moveDir);
    }

    IEnumerator DashCoroutine(Vector3 moveDir) {
        isDashing = true;
        lastDashTime = Time.time;
        float startTime = Time.time;
        Vector3 dashVelocity = transform.rotation * new Vector3(moveDir.x, 0f, moveDir.y) * dashSpeed;
        while(Time.time < startTime + dashDuration) {
            Debug.Log("Inside dashLoop");
            GetComponent<Rigidbody>().velocity = dashVelocity;
            yield return new WaitForSeconds(.1f);
        }
        isDashing = false;
        yield return null;
    }

    void Dash(Vector3 moveDir) {
        if(CanDash()) StartCoroutine(DashCoroutine(moveDir));
    }

    bool CanDash() {
        if(isDashing) return false;
        if(Time.time - lastDashTime > dashCooldown) return true;
        return false;
    }

    Vector3 ForwardVector() {
        Vector3 diff = transform.position - GetComponent<Camera>().transform.position;
        diff.y = 0f;
        diff = diff.normalized;
        return diff;
    }
}

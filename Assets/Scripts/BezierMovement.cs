using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierMovement : MonoBehaviour {
    public BezierCurve curve;
    public float cycleTime = 3f;
    public float a,exp,b,c;

    public void Update() {
        AnimatePath();
    }

    public void AnimatePath() {
        float x = Time.time % cycleTime;
        float t = a * Mathf.Pow(x, exp) + b * x + c;
        transform.position = curve.GetPoint(t);
    }
}

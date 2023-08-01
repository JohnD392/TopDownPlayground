using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BezierCurve : MonoBehaviour {
    public Vector3[] points;
    private int numPoints = 3;
    public int numSegments = 10;
    float pointGizmoSize = .3f;

    public void OnDrawGizmos() {
        for(int i=0; i<numSegments-1; i++) {
            float t0 = ((float)i)/((float)numSegments);
            float t1 = ((float)i+1)/((float)numSegments);
            Gizmos.DrawLine(
                GetPoint(points[0], points[1], points[2], t0), 
                GetPoint(points[0], points[1], points[2], t1)
            );
        }
    }

    public void ShowPoint(Vector3 point) {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(point, new Vector3(pointGizmoSize,pointGizmoSize,pointGizmoSize));
    }

    public Vector3 GetPoint (float t) {
		// return transform.TransformPoint(GetPoint(points[0], points[1], points[2], t));
        return GetPoint(points[0], points[1], points[2], t);
	}

	public static Vector3 GetPoint (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return oneMinusT * oneMinusT * p0 + 2f * oneMinusT * t * p1 + t * t * p2;
	}

    public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
		return 2f * (1f - t) * (p1 - p0) + 2f * t * (p2 - p1);
	}

    // public Vector3 GetVelocity (float t) {
	// 	return transform.TransformPoint(GetFirstDerivative(points[0], points[1], points[2], t)) - transform.position;
	// }

    // public Vector3 GetDirection (float t) {
	// 	return GetVelocity(t).normalized;
	// }

    public static Vector3 GetCubicPoint (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return oneMinusT * oneMinusT * oneMinusT * p0 + 3f * oneMinusT * oneMinusT * t * p1 + 3f * oneMinusT * t * t * p2 + t * t * t * p3;
	}
	
	public static Vector3 GetFirstCubicDerivative (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
		t = Mathf.Clamp01(t);
		float oneMinusT = 1f - t;
		return 3f * oneMinusT * oneMinusT * (p1 - p0) + 6f * oneMinusT * t * (p2 - p1) + 3f * t * t * (p3 - p2);
	}
}

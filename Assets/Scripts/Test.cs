using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    private void Update() {
        Debug.Log(Conditions.GetConditions(transform.position.x, transform.position.z));
    }
}

public class Conditions {
    float rainfall; // 0 - 1
    float temperature; // 0 - 1
    public Conditions(float rainfall, float temperature) {
        this.rainfall = rainfall;
        this.temperature = temperature;
    }

    public static Conditions GetConditions(float x, float z) {
        return new Conditions(
            new RainNoise().ValueAt(x, z),
            new TemperatureNoise().ValueAt(x, z)
        );
    }

    public string TempString() {
        return Remap(this.temperature, 0f, 1f, 20f, 120f).ToString() + "degrees F";
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public override string ToString() {
        return "Rainfall: " + rainfall + ", " + "Temperature: " + temperature;
    }
}
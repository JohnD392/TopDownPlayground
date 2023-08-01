using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPS : MonoBehaviour {
    public TMP_Text fps;
    void Update() {
        fps.text = (1.0f/Time.deltaTime).ToString("0.00");
    }
}

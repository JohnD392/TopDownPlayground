using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Focusable : MonoBehaviour {
    public float maxDistance = 3f;
    public Shader defaultShader;
    public Shader highlightShader;

    public void Start() {
        defaultShader = GetComponent<MeshRenderer>().material.shader;
    }

    public void Focus() {
        GetComponent<MeshRenderer>().material.shader = highlightShader;
    }

    public void Unfocus() {
        GetComponent<MeshRenderer>().material.shader = defaultShader;
    }
}

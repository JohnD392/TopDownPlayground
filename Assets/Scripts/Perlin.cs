using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perlin {
    public float scaleX; //smaller values result in larger blobs
    public float scaleY;
    public float threshold; // higher is less likely
    public float offsetX; // randomization
    public float offsetY;

    public Perlin(float scaleX, float scaleY, float threshold, float offsetX = 0f, float offsetY = 0f) {
        this.scaleX = scaleX;
        this.scaleY = scaleY;
        this.threshold = threshold;
        this.offsetX = offsetX;
        this.offsetY = offsetY;
    }

    public float Value(float x, float y, int chunkWidth, int chunkHeight) {
        return Mathf.PerlinNoise(this.scaleX * x / chunkWidth * 10f + this.offsetX, this.scaleY * y / chunkHeight * 10f + this.offsetY);
    }

    public float Value(float x, float y) {
        return Mathf.PerlinNoise(this.scaleX * x / 10f + this.offsetX, this.scaleY * y / 10f + this.offsetY);
    }

    public bool BinaryPerlin(float x, float y, int chunkWidth, int chunkHeight) {
        float perlinValue = Mathf.PerlinNoise(this.scaleX * x/chunkWidth*10f + this.offsetX, this.scaleY * y/chunkHeight*10f + this.offsetY);
        return perlinValue > this.threshold;
    }

    public static bool BinaryPerlin(
        float x, 
        float y, 
        float scaleX, 
        float scaleY, 
        int chunkWidth, 
        int chunkHeight, 
        float offsetX, 
        float offsetY, 
        float threshold
    ) {
        float perlinValue = Mathf.PerlinNoise(scaleX * x / chunkWidth * 10f + offsetX, scaleY * y / chunkHeight * 10f + offsetY);
        bool p = perlinValue > threshold;
        return p;
    }
}
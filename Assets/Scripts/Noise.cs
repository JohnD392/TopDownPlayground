using UnityEngine;

public abstract class Noise {
    public abstract Vector2 scale { get; }
    public abstract Vector2 offset { get; }
    public abstract float ValueAt(float x, float z);
}

public class TemperatureNoise : Noise {
    public override Vector2 scale => new Vector2(.0002f, .0002f);
    public override Vector2 offset => new Vector2(1000f, 500f);

    public override float ValueAt(float x, float z) {
        return Mathf.PerlinNoise(scale.x * x + 1000f / 10f, scale.y * z / 10f);
    }
}

public class RainNoise : Noise {
    public override Vector2 scale => new Vector2(.0002f, .0002f);
    public override Vector2 offset => new Vector2(123f, 1432f);

    public override float ValueAt(float x, float z) {
        return Mathf.PerlinNoise(scale.x * x / 10f, scale.y * z / 10f);
    }
}


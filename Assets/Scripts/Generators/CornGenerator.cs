using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornGenerator : ObjectGenerator {
    public override bool _DoesGenerate(float x, float z) {
        return Perlin.BinaryPerlin(x, z, .2f, .2f, 12, 12, 2543, 1120, .85f);
    }
}

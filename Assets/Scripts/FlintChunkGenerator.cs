using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlintChunkGenerator : ChunkGenerator {
    public override bool _DoesGenerate(float x, float z) {
        return Perlin.BinaryPerlin(x, z, .05f, .05f, this.chunkWidth, this.chunkHeight, 123, 123, .8f);
    }
}

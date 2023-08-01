public class RockChunkGenerator : ChunkGenerator {
    public override bool _DoesGenerate(float x, float z) {
        return Perlin.BinaryPerlin(x, z, .2f, .2f, 12, 12, 150, 120, .7f);
    }
}

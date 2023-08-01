public class TreeGenerator : ObjectGenerator {
    public override bool _DoesGenerate(float x, float z) {
        return Perlin.BinaryPerlin(x, z, .3f, .3f, 12, 12, 1150, 1120, .7f);
    }
}

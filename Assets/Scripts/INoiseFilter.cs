//provides a function so that an object generator can determine if it spawns or not
public interface INoiseFilter {
    public bool DoesGenerate(float x, float z);
}
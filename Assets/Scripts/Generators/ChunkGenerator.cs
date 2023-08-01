using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Generator : MonoBehaviour {
    public GameObject objPrefab;
    public Vector3 minObjectScale;
    public Vector3 maxObjectScale;
    public Vector3 maxRotation;
    public int numPerSquare;
    public List<Generator> parentGenerators; // A list of all generators to take into account when generating

    protected delegate bool DoesGenerate(float x, float z);

    public abstract bool _DoesGenerate(float x, float z);

    public bool DoesGenerateCountingParents(float x, float z) {
        foreach (Generator pg in this.parentGenerators) if (pg._DoesGenerate(x, z)) return false;
        return _DoesGenerate(x, z);
    }
}

// This class is responsible for removing objects too far from the target
public class ObjectUnloader {
    List<GameObject> objects;
    List<Transform> targets;
    float maxDistance;

    public ObjectUnloader(float maxDistance, List<Transform> targets) {
        this.maxDistance = maxDistance;
        this.targets = targets;
        objects = new List<GameObject>();
    }

    //Registers an object to be removed at the specified distance
    public void Register(GameObject obj) {
        if(!objects.Contains(obj)) objects.Add(obj);
    }

    public void Unload() {
        List<GameObject> objectsToRemove = new List<GameObject>();
        foreach(GameObject obj in objects) {
            foreach(Transform t in targets) {
                if(Vector3.Distance(obj.transform.position, t.position) > maxDistance) objectsToRemove.Add(obj);     
            }
        }

        foreach(GameObject obj in objectsToRemove) {
            this.objects.Remove(obj);
        }
    }
}

public abstract class ChunkGenerator : Generator {
    // Object specifications
    public GameObject chunkPrefab;
    // chunk info
    public int chunkWidth;
    public int chunkHeight;
    Dictionary<Vector2, GameObject> loadedChunks;
    public int xChunksToLoad;
    public int yChunksToLoad;

    public void Start() {
        loadedChunks = new Dictionary<Vector2, GameObject>();
        StartCoroutine(ChunkLoadingCoroutine());
    }

    public IEnumerator ChunkLoadingCoroutine() {
        //The Chunk loading coroutine runs in the background, and keeps the immediate area
        //Around the player loaded at all times, and also unloads the chunks too far from the player
        while (true) {
            // Every iteration of the loop is centered around the player position
            Vector3 playerPos = GameObject.Find("Player").transform.position;
            //Chunk Coords represents the chunk indices of the chunk in which the player is positioned
            Vector2 chunkCoords = CurrentChunkCoords(playerPos);
            //We iterate left and right of the player
            for (int i = (int)chunkCoords.x - xChunksToLoad / 2; i < (int)chunkCoords.x + xChunksToLoad / 2; i++) {
                //as well as "forward" and "back" (in the z axis for unity)
                for (int j = (int)chunkCoords.y - yChunksToLoad / 2; j < (int)chunkCoords.y + yChunksToLoad / 2; j++) {
                    //Check if the chunk we're looking to load is currently loaded
                    Vector2 v = new Vector2(i, j);
                    if (!loadedChunks.ContainsKey(v)) {
                        //if not, Generate the chunk
                        GameObject chunk = GenerateChunk(i, j, DoesGenerateCountingParents);
                        // Apply colliders if the generator calls for it. Ignore for grass, pebbles, bushes, things of that sort
                        loadedChunks[v] = chunk;
                        yield return null;
                    }
                }
            }
            ClearChunks(chunkCoords);
            yield return new WaitForSeconds(.5f);
        }
        yield return null;
    }

    public Vector2 CurrentChunkCoords(Vector3 worldPosition) {
        return new Vector2(worldPosition.x / chunkWidth, worldPosition.z / chunkHeight);
    }

    GameObject GenerateChunk(int x, int y, DoesGenerate doesGenerate) {
        GameObject chunk = Instantiate(chunkPrefab);
        chunk.transform.position = Vector3.zero;
        for (int i = x * chunkWidth; i < x * chunkWidth + chunkWidth; i++) {
            for (int j = y * chunkHeight; j < y * chunkHeight + chunkHeight; j++) {
                if (!doesGenerate(i, j)) continue;
                for (int ii = 0; ii < this.numPerSquare; ii++) {
                    for (int jj = 0; jj < this.numPerSquare; jj++) {
                        Vector3 position = new Vector3(i, 0f, j);
                        if(numPerSquare > 1) position = GetRandomOffset(i, j, ii, jj, numPerSquare);
                        //scale the object in the y direction
                        Vector3 objectScale = new Vector3(
                            Random.Range(minObjectScale.x, maxObjectScale.x),
                            Random.Range(minObjectScale.y, maxObjectScale.y),
                            Random.Range(minObjectScale.z, maxObjectScale.z)
                        );
                        //Rotate randomly
                        Quaternion rotation = Quaternion.Euler(
                            Random.Range(0f, maxRotation.x),
                            Random.Range(0f, maxRotation.y),
                            Random.Range(0f, maxRotation.z)
                        );
                        GameObject instance = Instantiate(objPrefab);
                        instance.transform.position = position;
                        instance.transform.localScale = objectScale;
                        instance.transform.rotation = rotation;
                        instance.transform.parent = chunk.transform;
                    }
                }
            }
        }
        GameObject combinedChunk = CombineChildrenObjects(chunk);
        return combinedChunk;
    }

    void ClearChunks(Vector2 referenceChunkCoords) {
        List<Vector2> toRemove = new List<Vector2>();
        foreach (KeyValuePair<Vector2, GameObject> pair in loadedChunks) {
            if (pair.Key.x < referenceChunkCoords.x - xChunksToLoad / 2 - 1 ||
                pair.Key.x > referenceChunkCoords.x + xChunksToLoad / 2 + 1 ||
                pair.Key.y < referenceChunkCoords.y - yChunksToLoad / 2 - 1 ||
                pair.Key.y > referenceChunkCoords.y + yChunksToLoad / 2 + 1
            ) {
                Destroy(pair.Value);
                toRemove.Add(pair.Key);
            }
        }
        foreach (Vector2 v in toRemove) loadedChunks.Remove(v);
    }

    GameObject CombineChildrenObjects(GameObject parent) {
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length) {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }
        parent.GetComponent<MeshFilter>().mesh = new Mesh();
        parent.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        if (parent.GetComponent<MeshCollider>() != null) parent.GetComponent<MeshCollider>().sharedMesh = parent.GetComponent<MeshFilter>().sharedMesh;
        parent.SetActive(true);
        foreach (Transform child in parent.transform) Object.Destroy(child.gameObject);
        return parent;
    }

    Vector3 GetRandomOffset(int xPos, int zPos, int xIndexInCell, int yIndexInCell, int numSquared) {
        float iiOffset = Random.Range(0f, 1f / (float)numSquared) + (float)xIndexInCell / (float)numSquared;
        float jjOffset = Random.Range(0f, 1f / (float)numSquared) + (float)yIndexInCell / (float)numSquared;
        Vector3 offset = new Vector3(
            (float)xPos + iiOffset,
            0f,
            (float)zPos + jjOffset
        );
        return offset;
    }
}

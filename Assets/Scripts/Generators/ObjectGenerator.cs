using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectGenerator : Generator {
    Dictionary<Vector2, GameObject> loadedObjects;

    public float distanceToLoad;

    public void Start() {
        loadedObjects = new Dictionary<Vector2, GameObject>();
        StartCoroutine(ObjectLoadingCoroutine());
    }

    public IEnumerator ObjectLoadingCoroutine() {
        //The Chunk loading coroutine runs in the background, and keeps the immediate area
        //Around the player loaded at all times, and also unloads the chunks too far from the player
        while (true) {
            // Every iteration of the loop is centered around the player position
            Vector3 playerPos = GameObject.Find("Player").transform.position;
            //We iterate left and right of the player
            for (int i = (int)(playerPos.x - distanceToLoad); i < (int)(playerPos.x + distanceToLoad); i++) {
                //as well as "forward" and "back" (in the z axis for unity)
                for (int j = (int)(playerPos.z - distanceToLoad); j < (int)(playerPos.z + distanceToLoad); j++) {
                    if (DoesGenerateCountingParents(i, j)) {
                        Vector2 v = new Vector2(i, j);
                        if(!loadedObjects.ContainsKey(v)) {
                            GameObject obj = Instantiate(objPrefab);
                            obj.transform.position = new Vector3(i, 0f, j);
                            loadedObjects[v] = obj;
                            yield return null;
                        }
                    }
                }
            }
            ClearObjects(new Vector2(playerPos.x, playerPos.z));
            yield return new WaitForSeconds(.5f);
        }
        yield return null;
    }

    void ClearObjects(Vector2 playerPosition) {
        List<Vector2> toRemove = new List<Vector2>();
        foreach (KeyValuePair<Vector2, GameObject> pair in loadedObjects) {
            if (Vector2.Distance(playerPosition, pair.Key) > distanceToLoad * 1.5f) {
                Destroy(pair.Value);
                toRemove.Add(pair.Key);
            }
        }
        foreach (Vector2 v in toRemove) loadedObjects.Remove(v);
    }
}

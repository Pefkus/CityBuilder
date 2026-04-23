using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpawnableObject
{
    public GameObject prefab;        
    public LayerMask allowedSurface; 
         
}
public class MapGeneretor : MonoBehaviour
{
    [Header("Ustawienia Spawnowania")]
    public List<SpawnableObject> objectsToSpawn; 
    public List<GameObject> allMapTiles;
    public List <GameObject> mapList = new List<GameObject>();
    public GameObject Parent;
    private void Awake()
    {
        int RandomMap = Random.Range(0, mapList.Count);
        GameObject Map = Instantiate(mapList[RandomMap], Parent.transform.position, Quaternion.identity, Parent.transform);
        Map.transform.position = Parent.transform.position;
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("tarrain");
        foreach(GameObject tile in tiles)
        {
            allMapTiles.Add(tile);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnRandomItem();
        }
    }

    public void SpawnRandomItem()
    {
        if (objectsToSpawn.Count == 0 || allMapTiles.Count == 0)
        {
            Debug.LogWarning("Brak obiektów do spawnowania lub kafelków na mapie!");
            return;
        }

        // 1. Losujemy obiekt z naszej listy
        int randomItemIndex = Random.Range(0, objectsToSpawn.Count);
        SpawnableObject selectedItem = objectsToSpawn[randomItemIndex];

        // 2. Szukamy wszystkich kafelków, które maj¹ warstwê zgodn¹ z 'allowedSurface'
        List<GameObject> validTiles = new List<GameObject>();

        foreach (GameObject tile in allMapTiles)
        {
            // Sprawdzamy, czy warstwa kafelka (tile.layer) znajduje siê w masce (allowedSurface)
            if ((selectedItem.allowedSurface.value & (1 << tile.layer)) != 0)
            {
                validTiles.Add(tile);
            }
        }

        // 3. Sprawdzamy, czy znaleŸliœmy chocia¿ jeden pasuj¹cy kafelek i tworzymy object
        if (validTiles.Count > 0)
        {
            GameObject chosenTile = validTiles[Random.Range(0, validTiles.Count)];
            Instantiate(selectedItem.prefab, chosenTile.transform.position, Quaternion.identity, chosenTile.transform);
        }
    }
}

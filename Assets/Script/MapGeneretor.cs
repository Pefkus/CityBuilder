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
    public static MapGeneretor Instance;
    [Header("Ustawienia Spawnowania")]
    public int Limit = 0;
    public int LimitObjects = 0;
     float timer= 0;
     float BaseInterval = 10;
    public List<SpawnableObject> objectsToSpawn; 
    public List<GameObject> allMapTiles;
    public List <GameObject> mapList = new List<GameObject>();
    public GameObject Parent;
    public float checkRadius = 0.2f;
    private void Awake()
    {
        Instance = this;
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
        timer += Time.deltaTime;
        if (timer >= BaseInterval)
        {
            if (Limit < LimitObjects)
            {
                SpawnRandomItem();
                Limit++;
                timer = 0;
            }
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
                if (!IsTileOccupied(tile.transform.position))
                {
                    validTiles.Add(tile);
                }
            }
        }

        // 3. Sprawdzamy, czy znaleŸliœmy chocia¿ jeden pasuj¹cy kafelek i tworzymy object
        if (validTiles.Count > 0)
        {
            GameObject chosenTile = validTiles[Random.Range(0, validTiles.Count)];
            Instantiate(selectedItem.prefab, chosenTile.transform.position, Quaternion.identity, chosenTile.transform);
        }
    }
    private bool IsTileOccupied(Vector3 checkPosition)
    {
        // Wersja dla gier 2D
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPosition, checkRadius);

        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Bulding") || col.CompareTag("Boosting Bulding"))
            {
                return true;
            }
        }
        return false;
    }
}

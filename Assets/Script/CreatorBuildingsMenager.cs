using UnityEngine;
using UnityEngine.Tilemaps;
public class CreatorBuildingsMenager : MonoBehaviour
{
    public static CreatorBuildingsMenager Instance { get; private set; }
    GameObject mouseController;
    MouseController MouseScrpit;
    public Tilemap Tilemapy;
    public LayerMask BuldingLayerMask;
    public bool IsThisTileMatching;
    private void Awake()
    {
        Instance = this;
        mouseController = GameObject.FindWithTag("Mouse");
        MouseScrpit = mouseController.GetComponent<MouseController>();
    }
    // Funkcja do sprawdzania, czy gracz ma wystarczaj¹co zasobów, ¿eby postawiæ dany budynek, który jest przekazywany jako argument, i jeœli tak, to wywo³anie funkcji do tworzenia budynku
    public void CheckTheBuilding(GameObject Building)
    {
        if(MouseScrpit.CreatingBuilding)
        {
            return;
        }
        bool canBuild = true;
        TypeOfBuilding typeOfBuilding = Building.GetComponent<TypeOfBuilding>();
        if (typeOfBuilding != null)
        {
            foreach (GameObject resource in typeOfBuilding.WhatResourcesNeedToBuild)
            {
                if (InventoryManager.Instance.GetValueOfItemInInventory(resource) < typeOfBuilding.CostsOfResources[typeOfBuilding.WhatResourcesNeedToBuild.IndexOf(resource)])
                {
                    canBuild = false;
                    break;
                }
            }
        }
        if (canBuild)
        {
            foreach (GameObject resource in typeOfBuilding.WhatResourcesNeedToBuild)
            {
                InventoryManager.Instance.ChangeValueOfItemInInventory(resource, -typeOfBuilding.CostsOfResources[typeOfBuilding.WhatResourcesNeedToBuild.IndexOf(resource)]);
            }
            CreateTheBulding(Building);
        }
           
    }
    // Funkcja do tworzenia budynku, który jest przekazywany jako argument, na pozycji kursora i przypisanie go do kolizji w MouseControllerze, ¿eby mo¿na by³o w niego klikaæ
    private void CreateTheBulding(GameObject name)
    {
        GameObject building = Instantiate(name, MouseScrpit.cursorMarker.position, Quaternion.identity, mouseController.transform);
        MouseScrpit.CreatingBuilding = true;
        MouseScrpit.PlacingTheBuilding = building;
        BuldingLayerMask.value = 1 << building.layer;
    }
    void Update()
    {
        if (MouseScrpit.CreatingBuilding)
        {
            IsTileGood();
            TileMatching();
            CheckThisTileForAnyBuilding();
        }
    }
    // Funkcja do sprawdzania, czy mo¿na postawiæ budynek na danej pozycji (czy nie ma tam innego budynku) i zmiana koloru znacznika kursora na zielony lub czerwony w zale¿noœci od tego, czy mo¿na postawiæ budynek
    public bool IsTileGood()
    {
        // Funkcja do sprawdzania, czy mo¿na postawiæ budynek na danej pozycji (czy nie ma tam innego budynku)
            if (MouseScrpit.Collision != null || !IsThisTileMatching)
            {
                MouseScrpit.cursorMarkerSpriteRenderer.GetComponent<SpriteRenderer>().color = Color.red;
                return false;
            }
            else
            {
                if (IsThisTileMatching)
                {
                    MouseScrpit.cursorMarkerSpriteRenderer.GetComponent<SpriteRenderer>().color = Color.green;
                    return true;
                }
                    
            }
        MouseScrpit.cursorMarkerSpriteRenderer.GetComponent<SpriteRenderer>().color = Color.white;
        return false;
    }
    // Funkcja do sprawdzania, czy na danej pozycji jest jakiœ budynek i przypisanie go do kolizji w MouseControllerze, ¿eby mo¿na by³o w niego klikaæ
    void CheckThisTileForAnyBuilding()
    {
        Vector3Int tileCoords = MouseScrpit.cellPosition;
        Vector3 cellWorldPos = Tilemapy.GetCellCenterWorld(tileCoords);
        // 2. Wykonaj test punktowy na konkretnej masce bitowej
        Collider2D[] hit = Physics2D.OverlapBoxAll(cellWorldPos, new Vector2(0.5f, 0.5f), 0, BuldingLayerMask); 
        if (hit != null)
        {
            foreach (Collider2D collider in hit)
            {
                if (collider != null)
                {
                    if (collider.CompareTag("Bulding") || collider.CompareTag("Boosting Bulding"))
                    {
                        if(MouseScrpit.Collision != collider.gameObject && MouseScrpit.PlacingTheBuilding != collider.gameObject)
                        {
                            MouseScrpit.Collision = collider.gameObject;
                        }
                    }
                }
            }
        }
    }
    // Funkcja do sprawdzania, czy na danej pozycji jest odpowiedni teren do postawienia budynku i przypisanie wyniku do zmiennej IsThisTileMatching, która jest u¿ywana w funkcji IsTileGood() do zmiany koloru znacznika kursora
    private void TileMatching()
    {
        Vector3Int tileCoords = MouseScrpit.cellPosition;
        Vector3 cellWorldPos = Tilemapy.GetCellCenterWorld(tileCoords);

        // 2. Wykonaj test punktowy na konkretnej masce bitowej
        Collider2D[] hit = Physics2D.OverlapBoxAll(cellWorldPos, new Vector2(0.5f, 0.5f), 0, BuldingLayerMask); 
        if (hit != null)
        {
            foreach (Collider2D collider in hit)
            {
                if (collider != null)
                {
                    if (collider.CompareTag("tarrain"))
                    {
                        IsThisTileMatching = true;
                    }
                    else
                    {
                        IsThisTileMatching = false;
                    }
                }
            }
        }
    }
}

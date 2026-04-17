using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
public class CreatorBuildingsMenager : MonoBehaviour
{
    public static CreatorBuildingsMenager Instance { get; private set; }
    public MouseController MouseScrpit;
    public Tilemap Tilemapy;
    public LayerMask BuldingLayerMask;
    public bool IsThisTileMatching;
    public bool IsThisTileInRangeOfMainBuilding;
    public bool IsThisTileInRangeOfAnyImportantBuilding;
    private HashSet<GameObject> buildingsInRange = new HashSet<GameObject>();
    private HashSet<GameObject> buildingsInRangeImportantBuildings = new HashSet<GameObject>();
    private LineRenderer lir;
    private void Awake()
    {
        lir = GetComponent<LineRenderer>();
        Instance = this;
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
        GameObject building = Instantiate(name, MouseScrpit.cursorMarker.position, Quaternion.identity, MouseScrpit.transform);
        building.name = name.name;
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
            DetectAdditionalImportantBuilding();
            DetectBuildingsFromMainBuilding();
        }
        else
        {
            buildingsInRangeImportantBuildings.Clear();
            buildingsInRange.Clear();
            lir.enabled = false;
        }
    }
    // Funkcja do sprawdzania, czy mo¿na postawiæ budynek na danej pozycji (czy nie ma tam innego budynku) i zmiana koloru znacznika kursora na zielony lub czerwony w zale¿noœci od tego, czy mo¿na postawiæ budynek
    public bool IsTileGood()
    {
        // Funkcja do sprawdzania, czy mo¿na postawiæ budynek na danej pozycji (czy nie ma tam innego budynku)
            if (MouseScrpit.Collision != null || !IsThisTileMatching || !IsThisTileInRangeOfMainBuilding || !IsThisTileInRangeOfAnyImportantBuilding)
            {
                MouseScrpit.cursorMarkerSpriteRenderer.GetComponent<SpriteRenderer>().color = Color.red;
                return false;
            }
            else
            {
                if (IsThisTileMatching && IsThisTileInRangeOfMainBuilding && IsThisTileInRangeOfAnyImportantBuilding)
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
    // Funkcja do wykrywania budynków w zasiêgu g³ównego budynku i przypisanie ich do zbioru buildingsInRange, który jest u¿ywany w funkcji IsTileGood() do sprawdzania, czy mo¿na postawiæ budynek na danej pozycji (czy jest w zasiêgu g³ównego budynku)
    void DetectBuildingsFromMainBuilding()
    {
        Vector2 center = TypesOfBuildingMenager.Instance.MainBuilding.transform.position;
        float radius = TypesOfBuildingMenager.Instance.MainBuilding.GetComponent<TypeOfBuilding>().BuildingRadius;
        Vector2 size = new Vector2(radius, radius);
        // 1. ZnajdŸ wszystkie collidery w zasiêgu
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(center, size, 0);
        UpdateLineRenderer(lir, center, size);
        // Zbiór budynków wykrytych w tej klatce
        HashSet<GameObject> currentlyDetected = new HashSet<GameObject>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Bulding") || hitCollider.CompareTag("Boosting Bulding"))
            {
                if (hitCollider.gameObject != MouseScrpit.PlacingTheBuilding)
                {
                    continue;
                }
                GameObject building = hitCollider.gameObject;
                currentlyDetected.Add(building);

                // Odpowiednik OnEnter: Jeœli nie by³o go wczeœniej, a jest teraz
                if (!buildingsInRange.Contains(building))
                {
                    buildingsInRange.Add(building);
                    IsThisTileInRangeOfMainBuilding = true;
                }
            }
        }

        // 2. Odpowiednik OnExit: Sprawdzamy, których budynków ju¿ nie ma w zasiêgu
        // Tworzymy kopiê do iteracji, aby móc usuwaæ elementy z orygina³u
        List<GameObject> toRemove = new List<GameObject>();
        foreach (var building in buildingsInRange)
        {
            if (!currentlyDetected.Contains(building))
            {
                toRemove.Add(building);
            }
        }

        foreach (var building in toRemove)
        {
            buildingsInRange.Remove(building);
            IsThisTileInRangeOfMainBuilding = false;
        }
    }
    void DetectAdditionalImportantBuilding()
    {
        if (MouseScrpit.PlacingTheBuilding != null && MouseScrpit.PlacingTheBuilding.GetComponent<TypeOfBuilding>().BuildingsPrefabs != null)
        {
            List<GameObject> AdditionalImportantBuilding = new List<GameObject>();
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Bulding");

            foreach (GameObject building in gameObjects)
            {
                if (building != MouseScrpit.PlacingTheBuilding && MouseScrpit.PlacingTheBuilding.GetComponent<TypeOfBuilding>().BuildingsPrefabs.name == building.name)
                {
                    AdditionalImportantBuilding.Add(building);
                }
            }

            // Zbiór budynków wykrytych w tej klatce przez WSZYSTKIE wa¿ne budynki
            HashSet<GameObject> currentlyDetected = new HashSet<GameObject>();

            // Dla ka¿dego wa¿nego budynku sprawdzamy, które budynki s¹ w jego zasiêgu i dodajemy je do zbioru currentlyDetected
            foreach (GameObject impBuilding in AdditionalImportantBuilding)
            {
                float radius = impBuilding.GetComponent<TypeOfBuilding>().BuildingRadius;
                Vector2 center = impBuilding.transform.position;
                Vector2 size = new Vector2(radius, radius);

                // Wykrywanie w zasiêgu konkretnego wa¿nego budynku
                Collider2D[] hitColliders = Physics2D.OverlapBoxAll(center, size, 0);

                UpdateLineRenderer(impBuilding.GetComponent<LineRenderer>(), center, size);

                // Dodajemy wszystkie wykryte budynki do zbioru currentlyDetected
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Boosting Bulding"))
                    {
                        if (hitCollider.gameObject.GetComponent<TypeOfBuilding>() != null && hitCollider.gameObject.name == MouseScrpit.PlacingTheBuilding.name)
                        {
                            currentlyDetected.Add(hitCollider.gameObject);
                        }
                    }
                }
            }
            // 1. Logika OnEnter: Jeœli coœ jest w zasiêgu, a wczeœniej nie by³o
            foreach (GameObject building in currentlyDetected)
            {
                if (!buildingsInRangeImportantBuildings.Contains(building))
                {
                    buildingsInRangeImportantBuildings.Add(building);
                }
            }

            // 2. Logika OnExit: Usuwamy obiekty, których ju¿ nie ma w zasiêgu ¯ADNEGO wa¿nego budynku
            List<GameObject> toRemove = new List<GameObject>();
            foreach (var building in buildingsInRangeImportantBuildings)
            {
                if (!currentlyDetected.Contains(building))
                {
                    toRemove.Add(building);
                }
            }

            foreach (var building in toRemove)
            {
                buildingsInRangeImportantBuildings.Remove(building);
            }

            // 3. Aktualizacja flagi stanu
            IsThisTileInRangeOfAnyImportantBuilding = buildingsInRangeImportantBuildings.Count > 0;
        }
        else
        {
            IsThisTileInRangeOfAnyImportantBuilding = true;
        }
    }
    // Funkcja do aktualizacji LineRenderer, który rysuje czerwon¹ ramkê wokó³ zasiêgu wa¿nych budynków, na podstawie pozycji i promienia budynku
    void UpdateLineRenderer(LineRenderer lr, Vector2 center, Vector2 radius)
    {
        lr.enabled = true;
        lr.useWorldSpace = true;
        lr.loop = true;
        lr.positionCount = 4;

        float hX = radius.x / 2f;
        float hY = radius.y / 2f;

        Vector3[] corners = new Vector3[]
        {
        new Vector3(center.x - hX, center.y - hY, 0),
        new Vector3(center.x + hX, center.y - hY, 0),
        new Vector3(center.x + hX, center.y + hY, 0),
        new Vector3(center.x - hX, center.y + hY, 0)
        };

        lr.SetPositions(corners);
    }
}

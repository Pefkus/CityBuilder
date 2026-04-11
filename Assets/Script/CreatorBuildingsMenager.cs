using Unity.VisualScripting;
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
    // Funkcja do tworzenia budynku, który jest przekazywany jako argument, na pozycji kursora i przypisanie go do kolizji w MouseControllerze, ¿eby mo¿na by³o w niego klikaæ
    public void SelectBulding(GameObject name)
    {
        GameObject building = Instantiate(name, MouseScrpit.cursorMarker.position, Quaternion.identity, mouseController.transform);
        MouseScrpit.CreatingBuilding = true;
        MouseScrpit.PlacingTheBuilding = building;
        BuldingLayerMask.value = 1 << building.layer;
        
    }
    void Update()
    {
        IsTileGood();
        TileMatching();
    }
    public bool IsTileGood()
    {

        // Funkcja do sprawdzania, czy mo¿na postawiæ budynek na danej pozycji (czy nie ma tam innego budynku)
        if (MouseScrpit.CreatingBuilding)
        {
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
        }
        MouseScrpit.cursorMarkerSpriteRenderer.GetComponent<SpriteRenderer>().color = Color.white;
        return false;
    }
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
                        if(collider.gameObject.layer != LayerMask.NameToLayer("fog")){
                            IsThisTileMatching = true;
                        }
                        else
                        {
                            IsThisTileMatching = false;
                            return;
                        }
                        
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

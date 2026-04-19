using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class MouseController : MonoBehaviour
{
    public static MouseController Instance { get; private set; }

    Camera mainCamera;
    public GameObject Collision;
    public GameObject PlacingTheBuilding;
    public bool CreatingBuilding = false;
    private List<GameObject> buildingsInRange = new List<GameObject>();

    public GameObject particlesystem;
    public Transform cursorMarker;
    public Vector3Int cellPosition;
    public GameObject cursorMarkerSpriteRenderer;
    public BoxCollider2D BoxCollider2D;
    public Grid myGrid;
    [Header("CLICK Settings")]
    public float Speed = 1.5f;
    public float timer = 0f;
    public float baseInterval = 1f; 
    [Header("CursorUi")]
    public GameObject CursorUi;
    void Start()
    {
        Instance = this;
        mainCamera = Camera.main;
    }
    void Update()
    {
        DetectCollision();
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // Ukrywamy znacznik kursora (wy³¹czamy obiekt w hierarchii)
            if (cursorMarkerSpriteRenderer.gameObject.activeSelf)
            {
                cursorMarkerSpriteRenderer.gameObject.SetActive(false);
                CursorUi.SetActive(true);
            }

            // Przerywamy dzia³anie funkcji, ¿eby nie przeliczaæ i nie ruszaæ kursora pod UI
            return;
        }
        else
        {
            // Jeœli nie jesteœmy nad UI, a znacznik by³ ukryty, to go pokazujemy
            if (!cursorMarkerSpriteRenderer.gameObject.activeSelf)
            {
                cursorMarkerSpriteRenderer.gameObject.SetActive(true);
                CursorUi.SetActive(false);
            }
        }
        Vector3 rawWorldPosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Grid automatycznie przelicza to na kordynaty komórki (np. x:1, y:2), a potem zwraca idealny œrodek tej komórki w œwiecie
        cellPosition = myGrid.WorldToCell(rawWorldPosition);
        if(CreatingBuilding)
        {
            if(PlacingTheBuilding != null)
            {
                if (PlacingTheBuilding.GetComponent<TypeOfBuilding>().BuildingSize > 1)
                {
                    cursorMarkerSpriteRenderer.transform.localScale = new Vector3(2f, 2f, 1f);
                    cursorMarker.GetComponent<BoxCollider2D>().size = new Vector2(1.75f, 1.75f);
                    cursorMarker.position = myGrid.GetCellCenterWorld(cellPosition) + new Vector3(0.5f, 0.5f, 0);
                }
                else
                {
                    cursorMarkerSpriteRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
                    cursorMarker.GetComponent<BoxCollider2D>().size = new Vector2(0.75f, 0.75f);
                    cursorMarker.position = myGrid.GetCellCenterWorld(cellPosition);
                }
            } 
        }
        else
        {
            if (buildingsInRange.Count > 0)
            {
                if (Collision.gameObject.CompareTag("Bulding") || Collision.gameObject.CompareTag("Boosting Bulding"))
                {
                    if (Collision.GetComponent<TypeOfBuilding>().BuildingSize > 1)
                    {
                        cursorMarkerSpriteRenderer.transform.localScale = new Vector3(2f, 2f, 1f);
                        cursorMarker.GetComponent<BoxCollider2D>().size = new Vector2(0.75f, 0.75f);
                        cursorMarkerSpriteRenderer.transform.position = Collision.transform.position;
                        cursorMarker.position = myGrid.GetCellCenterWorld(cellPosition);

                    }
                    else
                    {
                        cursorMarkerSpriteRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
                        cursorMarker.GetComponent<BoxCollider2D>().size = new Vector2(0.75f, 0.75f);
                        cursorMarker.position = myGrid.GetCellCenterWorld(cellPosition);
                        cursorMarkerSpriteRenderer.transform.position = Collision.transform.position;
                    }
                }
            }
            else
            {
                    cursorMarkerSpriteRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
                    cursorMarker.GetComponent<BoxCollider2D>().size = new Vector2(0.75f, 0.75f);
                    cursorMarkerSpriteRenderer.transform.position = cursorMarker.position;
                    cursorMarker.position = myGrid.GetCellCenterWorld(cellPosition);
            }
        }
        

        timer += Time.deltaTime * Speed;

        // Sprawdzamy, czy lewy przycisk myszy jest wciœniêty
        if (Input.GetMouseButton(0))
        {
            if (!CreatingBuilding)
            {
                // Jeœli nie tworzymy budynku i klikniemy, to sprawdzamy, czy mo¿emy klikn¹æ w budynek (czy jest jakiœ w kolizji) i jeœli tak, to klikamy w niego co sekundê
                if (timer >= baseInterval)
                {
                    if (Collision != null)
                    {
                        if (Collision.gameObject.CompareTag("Bulding"))
                        {
                            if(Collision.gameObject.GetComponent<Bulding>() != null)
                            {
                                if (Collision.gameObject.GetComponent<Bulding>().isProdusingBuilding)
                                    Collision.gameObject.GetComponent<Bulding>().ProdusingItem(0);
                            }
                            
                        }
                        Collision.gameObject.GetComponentInChildren<Animator>().SetTrigger("Click");
                    }
                    timer = 0f; // Resetujemy licznik
                }
            }
            else
            {
                // Jeœli tworzymy budynek i klikniemy, to przypinamy go do siatki i koñczymy tworzenie
                if (CreatorBuildingsMenager.Instance.IsTileGood())
                {
                    PlacingTheBuilding.transform.SetParent(myGrid.transform);
                    Instantiate(particlesystem, PlacingTheBuilding.transform.position, Quaternion.identity, PlacingTheBuilding.transform);
                    CreatingBuilding = false;
                    Collision = PlacingTheBuilding;
                    PlacingTheBuilding = null;
                    cursorMarkerSpriteRenderer.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
                
            
        }
    }
    // Ta funkcja wykrywa kolizje z budynkami i aktualizuje listê budynków w zasiêgu oraz aktualny obiekt kolizji
    void DetectCollision()
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(BoxCollider2D.bounds.center, BoxCollider2D.bounds.size, 0f);
        HashSet<GameObject> currentlyDetected = new HashSet<GameObject>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Bulding") || hitCollider.CompareTag("Boosting Bulding"))
            {
                if (hitCollider.gameObject == PlacingTheBuilding)
                {
                    continue;
                }
                GameObject building = hitCollider.gameObject;
                currentlyDetected.Add(building);

                // Odpowiednik OnEnter: Jeœli nie by³o go wczeœniej, a jest teraz
                if (!buildingsInRange.Contains(building))
                {
                    buildingsInRange.Add(building);
                    Collision = building;
                }
            }
        }

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
            if (Collision == building)
            {
                Collision = null;
            }
        }
    }
}

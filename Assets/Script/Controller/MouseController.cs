using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class MouseController : MonoBehaviour
{
    public static MouseController Instance { get; private set; }

    Camera mainCamera;
    [Header("Kolizje z obiektami")]
    public GameObject NpcCollision;
    public GameObject GrabedNpc;
    public GameObject Collision;
    public GameObject PlacingTheBuilding;
    [Header("Efekty bool")]
    public bool CreatingBuilding = false;
    public bool GrabingTheNPC = false;
    private List<GameObject> buildingsInRange = new List<GameObject>();
    private List<GameObject> NPCInRange = new List<GameObject>();
    
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
    public int ClickBonus = 0;
    [Header("CursorUi")]
    public GameObject CursorUi;
    void Start()
    {
        Instance = this;
        mainCamera = Camera.main;
    }
    void Update()
    {
        DetectNpc();
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
                    if(Collision.GetComponent<TypeOfBuilding>() != null)
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
                    else
                    {
                        cursorMarkerSpriteRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
                        cursorMarker.GetComponent<BoxCollider2D>().size = new Vector2(0.75f, 0.75f);
                        cursorMarkerSpriteRenderer.transform.position = cursorMarker.position;
                        cursorMarker.position = myGrid.GetCellCenterWorld(cellPosition);
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
                if (GrabingTheNPC)
                {
                    PutTheNpc();
                }
                else
                {
                    // Jeœli nie tworzymy budynku i klikniemy, to sprawdzamy, czy mo¿emy klikn¹æ w budynek (czy jest jakiœ w kolizji) i jeœli tak, to klikamy w niego co sekundê
                    if (timer >= baseInterval)
                    {
                        if (Collision != null)
                        {
                            if (Collision.gameObject.CompareTag("Bulding"))
                            {
                                if (Collision.gameObject.GetComponent<Bulding>() != null)
                                {
                                    if (Collision.gameObject.GetComponent<Bulding>().isProdusingBuilding)
                                        Collision.gameObject.GetComponent<Bulding>().ProdusingItem(ClickBonus);
                                }
                                else if(Collision.gameObject.GetComponent<MainBuilding>() != null)
                                {
                                    Collision.gameObject.GetComponent<MainBuilding>().CheckItems();
                                    
                                } 
                                else if(Collision.gameObject.GetComponent<ObcjectOnMap>() != null)
                                {
                                    Collision.gameObject.GetComponent<ObcjectOnMap>().PunchingTheObject(ClickBonus);
                                }
                            }
                            Collision.gameObject.GetComponentInChildren<Animator>().SetTrigger("Click");
                        }

                        timer = 0f; // Resetujemy licznik
                    }
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
        if (Input.GetMouseButton(1))
        {
            if (!CreatingBuilding && !GrabingTheNPC)
            {
                if (NpcCollision != null)
                {
                    GrabTheNpc();
                }
            }
        }
        if (GrabingTheNPC)
        {
            GrabedNpc.transform.position = this.transform.position;
        }
    }
    void GrabTheNpc()
    {
        if (NpcCollision != null)
        {
            GrabedNpc = NpcCollision;
            GrabedNpc.GetComponent<NPC>().HardWorking = false;
            GrabedNpc.GetComponent<NPC>().Building = null;
            GrabedNpc.GetComponent<MovmentNPC>().enabled = false;
            GrabingTheNPC = true;
            GrabedNpc.transform.SetParent(this.transform);
            
        }
    }
    void PutTheNpc()
    {
        if (GrabedNpc != null) 
        {
            if (Collision != null)
            {
                if (Collision.CompareTag("Bulding") && Collision.GetComponent<Bulding>() != null)
                {
                    GrabedNpc.transform.SetParent(Collision.transform);
                    GrabedNpc.GetComponent<NPC>().Building = Collision;
                    GrabingTheNPC = false;
                    GrabedNpc = null;
                }
            }
            else
            {
                if (NPCController.Instance.CheckTile())
                {
                    GrabedNpc.transform.SetParent(myGrid.transform);
                    GrabedNpc.GetComponent<MovmentNPC>().enabled = true;
                    GrabingTheNPC = false;
                    GrabedNpc = null;
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
    void DetectNpc()
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(BoxCollider2D.bounds.center, BoxCollider2D.bounds.size, 0f);
        HashSet<GameObject> currentlyDetected = new HashSet<GameObject>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("NPC"))
            {
                if (hitCollider.gameObject == GrabedNpc)
                {
                    continue;
                }
                GameObject Npc = hitCollider.gameObject;
                currentlyDetected.Add(Npc);

                // Odpowiednik OnEnter: Jeœli nie by³o go wczeœniej, a jest teraz
                if (!NPCInRange.Contains(Npc))
                {
                    NPCInRange.Add(Npc);
                    NpcCollision = Npc;
                }
            }
        }

        List<GameObject> toRemove = new List<GameObject>();
        foreach (var Npc in NPCInRange)
        {
            if (!currentlyDetected.Contains(Npc))
            {
                toRemove.Add(Npc);
            }
        }

        foreach (var Npc in toRemove)
        {
            NPCInRange.Remove(Npc);
            if (NpcCollision == Npc)
            {
                NpcCollision = null;
            }
        }
    }
}

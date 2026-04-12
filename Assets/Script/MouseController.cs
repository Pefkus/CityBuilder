using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
public class MouseController : MonoBehaviour
{
    Camera mainCamera;
    public GameObject Collision;
    public GameObject PlacingTheBuilding;
    public bool CreatingBuilding = false;

    public GameObject particlesystem;
    public Transform cursorMarker;
    public Vector3Int cellPosition;
    public GameObject cursorMarkerSpriteRenderer;
    public Grid myGrid;
    public float Speed = 1.5f;
    private float timer = 0f;
    private float baseInterval = 1f; // Bazowa sekunda

    void Start()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // Ukrywamy znacznik kursora (wy³¹czamy obiekt w hierarchii)
            if (cursorMarkerSpriteRenderer.gameObject.activeSelf)
            {
                cursorMarkerSpriteRenderer.gameObject.SetActive(false);
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
            cursorMarkerSpriteRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
            cursorMarker.GetComponent<BoxCollider2D>().size = new Vector2(0.75f, 0.75f);
            cursorMarker.position = myGrid.GetCellCenterWorld(cellPosition);
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
                            if (Collision.gameObject.GetComponent<Bulding>().isProdusingBuilding)
                                Collision.gameObject.GetComponent<Bulding>().ProdusingItem(0);
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

    


    // Funkcje do wykrywania kolizji z budynkami, ¿eby wiedzieæ, w co klikamy i gdzie mo¿emy postawiæ budynek
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bulding") || collision.gameObject.CompareTag("Boosting Bulding"))
        {
            if(PlacingTheBuilding == null || PlacingTheBuilding.gameObject != collision.gameObject)
            {
                Collision = collision.gameObject;
            }
           
        } 
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bulding") || collision.gameObject.CompareTag("Boosting Bulding"))
        {
                Collision = null;
        }
    }

}

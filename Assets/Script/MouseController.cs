using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class MouseController : MonoBehaviour
{
    Camera mainCamera;
    private GameObject Collision;
    public GameObject PlacingTheBuilding;
    public bool CreatingBuilding = false;

    public Transform cursorMarker;
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
        Vector3Int cellPosition = myGrid.WorldToCell(rawWorldPosition);
        cursorMarker.position = myGrid.GetCellCenterWorld(cellPosition);

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
                        if (Collision.gameObject.GetComponent<Bulding>().isProdusingBuilding)
                            Collision.gameObject.GetComponent<Bulding>().ProdusingItem(0);

                        Collision.gameObject.GetComponentInChildren<Animator>().SetTrigger("Click");
                    }
                    timer = 0f; // Resetujemy licznik
                }
            }
            else
            {
                
                    // Jeœli tworzymy budynek i klikniemy, to przypinamy go do siatki i koñczymy tworzenie
                    if (IsPlaneGood())
                    {
                        PlacingTheBuilding.transform.SetParent(myGrid.transform);
                        CreatingBuilding = false;
                        PlacingTheBuilding = null;
                    }
            }
                
            
        }
        IsPlaneGood();
    }

    bool IsPlaneGood()
    {
        // Funkcja do sprawdzania, czy mo¿na postawiæ budynek na danej pozycji (czy nie ma tam innego budynku)
        if (CreatingBuilding)
        {
            if (Collision != null)
            {
                cursorMarkerSpriteRenderer.GetComponent<SpriteRenderer>().color = Color.red;
                return false;
            }
            else
            {
                cursorMarkerSpriteRenderer.GetComponent<SpriteRenderer>().color = Color.green;
                return true;
            }
        }
        cursorMarkerSpriteRenderer.GetComponent<SpriteRenderer>().color = Color.white;
        return false;
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

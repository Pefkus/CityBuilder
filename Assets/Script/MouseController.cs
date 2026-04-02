using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class MouseController : MonoBehaviour
{
    Camera mainCamera;
    GameObject Collision;

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

        if (Input.GetMouseButton(0))
        {
            if (timer >= baseInterval)
            {
                if(Collision != null)
                {
                    if (Collision.gameObject.GetComponent<Bulding>().isProdusingBuilding)
                        Collision.gameObject.GetComponent<Bulding>().ProdusingItem(0);

                    Collision.gameObject.GetComponentInChildren<Animator>().SetTrigger("Click");
                }
                timer = 0f; // Resetujemy licznik
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bulding"))
        {
            Collision = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bulding"))
        {
            Collision = null;
        }
    }

}

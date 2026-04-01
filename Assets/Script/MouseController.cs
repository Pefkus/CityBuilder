using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class MouseController : MonoBehaviour
{
    Camera mainCamera;
    public Transform cursorMarker;
    public Grid myGrid;
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // Ukrywamy znacznik kursora (wy³¹czamy obiekt w hierarchii)
            if (cursorMarker.gameObject.activeSelf)
            {
                cursorMarker.gameObject.SetActive(false);
            }

            // Przerywamy dzia³anie funkcji, ¿eby nie przeliczaæ i nie ruszaæ kursora pod UI
            return;
        }
        else
        {
            // Jeœli nie jesteœmy nad UI, a znacznik by³ ukryty, to go pokazujemy
            if (!cursorMarker.gameObject.activeSelf)
            {
                cursorMarker.gameObject.SetActive(true);
            }
        }

        Vector3 rawWorldPosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Grid automatycznie przelicza to na kordynaty komórki (np. x:1, y:2), a potem zwraca idealny œrodek tej komórki w œwiecie
        Vector3Int cellPosition = myGrid.WorldToCell(rawWorldPosition);
        cursorMarker.position = myGrid.GetCellCenterWorld(cellPosition);

        
    }

    

}

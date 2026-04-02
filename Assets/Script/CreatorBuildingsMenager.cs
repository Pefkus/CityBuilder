using UnityEngine;

public class CreatorBuildingsMenager : MonoBehaviour
{
    public static CreatorBuildingsMenager Instance { get; private set; }
    GameObject mouseController;
    
    private void Awake()
    {
        Instance = this;
        mouseController = GameObject.FindWithTag("Mouse");
    }
    // Funkcja do tworzenia budynku, który jest przekazywany jako argument, na pozycji kursora i przypisanie go do kolizji w MouseControllerze, ¿eby mo¿na by³o w niego klikaæ
    public void SelectBulding(GameObject name)
    {
        GameObject building = Instantiate(name, mouseController.GetComponent<MouseController>().cursorMarker.position, Quaternion.identity, mouseController.transform);
            mouseController.GetComponent<MouseController>().CreatingBuilding = true;
            mouseController.GetComponent<MouseController>().PlacingTheBuilding = building;
    }
}

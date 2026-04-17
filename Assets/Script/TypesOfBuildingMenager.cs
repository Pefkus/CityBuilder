using System.Collections.Generic;
using UnityEngine;

public class TypesOfBuildingMenager : MonoBehaviour
{
    public static TypesOfBuildingMenager Instance { get; private set; }
    public GameObject MainBuilding;
    public List<GameObject> TypesOfBuildings = new List<GameObject>();
    void Start()
    {
        Instance = this;
        GameObject[] BuildingsOnMap = GameObject.FindGameObjectsWithTag("Bulding");
        foreach (GameObject building in BuildingsOnMap)
        {
            if (building.GetComponent<TypeOfBuilding>() != null && building.GetComponent<TypeOfBuilding>().MainBuilding)
            {
                if (building.GetComponent<StorageBuilding>() != null)
                {
                    MainBuilding = building;
                }
                break;
            }
        }
    }
    public GameObject SearchForBuilding(string name)
    {
        GameObject building = TypesOfBuildings.Find(x => x.name == name);
        if (building == null)
        {
            Debug.LogError("Nie istnieje budynek o nazwie: " + name);
            return null;
        }
        return building;
    }
}

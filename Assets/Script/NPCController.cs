using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public GameObject NewNpc;
    public static NPCController Instance;
    public List<GameObject> Npc = new List<GameObject>();
    public int MaxPeopleStorage = 2;
    GameObject mainBuilding;
    public Grid Mapa;
    void Start()
    {
        Instance = this;
        GameObject[] BuildingsOnMap = GameObject.FindGameObjectsWithTag("Bulding");
        foreach (GameObject Mainbuilding in BuildingsOnMap)
        {
            if (Mainbuilding.GetComponent<TypeOfBuilding>() != null && Mainbuilding.GetComponent<TypeOfBuilding>().MainBuilding)
            {
                if (Mainbuilding.GetComponent<StorageBuilding>() != null)
                {
                    mainBuilding = Mainbuilding;
                    MaxPeopleStorage = Mainbuilding.GetComponent<StorageBuilding>().MaxPeopleStorage;
                    break;
                }
            }
        }
    }
    public void CreateNewNpc()
    {
        GameObject newNpc = Instantiate(NewNpc, mainBuilding.transform.position, Quaternion.identity, Mapa.transform);
    }
    public bool CanCreateNewNPC()
    {
        if(MaxPeopleStorage > Npc.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void ChangePeopleStorage(int storage)
    {
        MaxPeopleStorage += storage;
    }
    // Update is called once per frame
    void Update()
    {
        SearchForNpcOnMap();
    }
    void SearchForNpcOnMap()
    {
        GameObject[] list = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject obj in list)
        {
            if (obj != null)
            {
                if(obj != Npc.Contains(obj))
                {
                    Npc.Add(obj);
                }
            }
        }

    }

}

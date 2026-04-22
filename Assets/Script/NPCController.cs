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
    public LayerMask NpcLayerMask;
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
    public bool CheckTile()
    {
        Vector3Int tileCoords = MouseController.Instance.cellPosition;
        Vector3 cellWorldPos = Mapa.GetCellCenterWorld(tileCoords);

        // 2. Wykonaj test punktowy na konkretnej masce bitowej
        Collider2D[] hit = Physics2D.OverlapBoxAll(cellWorldPos, new Vector2(0.5f, 0.5f), 0, NpcLayerMask);
        if (hit != null)
        {
            foreach (Collider2D collider in hit)
            {
                if (collider != null)
                {
                    return true ;
                }
            }
        }
        return false;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public static NPCController instance;
    public List<GameObject> Npc = new List<GameObject>();
    void Start()
    {
       instance = this; 
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

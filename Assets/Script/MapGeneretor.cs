using System.Collections.Generic;
using UnityEngine;

public class MapGeneretor : MonoBehaviour
{
    public List <GameObject> mapList = new List<GameObject>();
    public GameObject Parent;
    private void Awake()
    {
        int RandomMap = Random.Range(0, mapList.Count);
        GameObject Map = Instantiate(mapList[RandomMap], Parent.transform.position, Quaternion.identity, Parent.transform);
        Map.transform.position = Parent.transform.position;
    }
}

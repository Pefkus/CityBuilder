using System.Collections.Generic;
using UnityEngine;

public class TypeOfBuilding : MonoBehaviour
{
    [Header("Dane Budynku")]
    public List<int> CostsOfResources = new List<int>();
    public List<GameObject> WhatResourcesNeedToBuild = new List<GameObject>();
    public int BuildingSize = 1;
    [Header("Czy budynek jest g³ównym budynkiem?")]
    public bool MainBuilding = false;
    [Header("Promieñ budynku w jakim mo¿na stawiaæ inne budynki oraz budynki mo¿na wstawiæ")]
    public float BuildingRadius = 0f;
    public GameObject BuildingsPrefabs ;
    public LineRenderer LineRenderer;
    [Header("Opis Budynku")]
    [TextArea(3, 10)]
    public string Description;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(BuildingRadius, BuildingRadius, 0));
    }
    private void Update()
    {
        if(LineRenderer != null)
        {
            if(!MouseController.Instance.CreatingBuilding)
            {
                LineRenderer.enabled = false;
            }
        }
    }
}

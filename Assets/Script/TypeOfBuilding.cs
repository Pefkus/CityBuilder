using System.Collections.Generic;
using UnityEngine;

public class TypeOfBuilding : MonoBehaviour
{
    [Header("Dane Budynku")]
    public List<int> CostsOfResources = new List<int>();
    public List<GameObject> WhatResourcesNeedToBuild = new List<GameObject>();
    public int BuildingSize = 1;
    [Header("Opis Budynku")]
    [TextArea(3, 10)]
    public string Description;
}

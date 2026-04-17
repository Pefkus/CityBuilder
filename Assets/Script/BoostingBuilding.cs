using System;
using System.Collections.Generic;
using UnityEngine;

public class BoostingBuilding : MonoBehaviour
{
    public float boostAmount; // Iloœæ boosta, który dodaje do prêdkoœci myszy
    public float detectionRadius = 2.5f; 
    public bool ColectingSpeedBoost = false;
    private HashSet<GameObject> buildingsInRange = new HashSet<GameObject>();
    public GameObject Forspecificbuilding;

    void Update()
    {
        DetectBuildings();
    }

    void DetectBuildings()
    {
        // 1. ZnajdŸ wszystkie collidery w zasiêgu
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        // Zbiór budynków wykrytych w tej klatce
        HashSet<GameObject> currentlyDetected = new HashSet<GameObject>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Bulding"))
            {
                if(hitCollider.gameObject.GetComponent<Bulding>() == null)
                {
                    continue;
                }
                GameObject building = hitCollider.gameObject;
                currentlyDetected.Add(building);

                // Odpowiednik OnEnter: Jeœli nie by³o go wczeœniej, a jest teraz
                if (!buildingsInRange.Contains(building))
                {
                    buildingsInRange.Add(building);
                    OnBuildingEnter(building);
                }
            }
        }

        // 2. Odpowiednik OnExit: Sprawdzamy, których budynków ju¿ nie ma w zasiêgu
        // Tworzymy kopiê do iteracji, aby móc usuwaæ elementy z orygina³u
        List<GameObject> toRemove = new List<GameObject>();
        foreach (var building in buildingsInRange)
        {
            if (!currentlyDetected.Contains(building))
            {
                toRemove.Add(building);
            }
        }

        foreach (var building in toRemove)
        {
            buildingsInRange.Remove(building);
            OnBuildingExit(building);
        }
    }


    // Wywo³ywane raz, gdy budynek znajdzie siê w kole
    void OnBuildingEnter(GameObject building)
    {
        if (!ColectingSpeedBoost)
        {   
            if(Forspecificbuilding != null)
            {
                if(building.name == Forspecificbuilding.name)
                {
                    building.gameObject.GetComponent<Bulding>().AdaptiveBoost += Convert.ToInt32(boostAmount);
                }
                return;
            }
            else
            {
                building.gameObject.GetComponent<Bulding>().AdaptiveBoost += Convert.ToInt32(boostAmount);
            }
        }
            
    }

    // Wywo³ywane raz, gdy budynek opuœci ko³o
    void OnBuildingExit(GameObject building)
    {
        if (!ColectingSpeedBoost)
        {
            if (Forspecificbuilding != null)
            {
                if (building.name == Forspecificbuilding.name)
                {
                    building.gameObject.GetComponent<Bulding>().AdaptiveBoost -= Convert.ToInt32(boostAmount);
                }

                return;
            }
            else
            {
                    building.gameObject.GetComponent<Bulding>().AdaptiveBoost -= Convert.ToInt32(boostAmount);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

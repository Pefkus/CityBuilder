using System.Collections.Generic;
using UnityEngine;

public class BoostingBuilding : MonoBehaviour
{
    public float detectionRadius = 2.5f; // Promieñ sprawdzania
                                       // Lista budynków, które s¹ aktualnie w zasiêgu
    private HashSet<GameObject> buildingsInRange = new HashSet<GameObject>();

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
        building.gameObject.GetComponent<Bulding>().AdaptiveBoost += 1;
    }

    // Wywo³ywane raz, gdy budynek opuœci ko³o
    void OnBuildingExit(GameObject building)
    {
        building.gameObject.GetComponent<Bulding>().AdaptiveBoost -= 1;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

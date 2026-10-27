using System;
using System.Collections.Generic;
using UnityEngine;

public class BoostingBuilding : MonoBehaviour
{
    // Wartoœæ premii dodawanej do wykrytego budynku (np. +10 do prêdkoœci).
    public float boostAmount;

    // Zasiêg, w jakim budynek szuka innych obiektów.
    public float detectionRadius = 2.5f;

    // Prze³¹cznik, który obecnie blokuje dodawanie/odejmowanie 'AdaptiveBoost', jeœli jest na 'true'.
    public bool ColectingSpeedBoost = false;

    // Pamiêæ podrêczna przechowuj¹ca budynki, które JU¯ s¹ w zasiêgu. 
    // Dziêki temu bonus nie jest dodawany co klatkê, a tylko raz przy wejœciu.
    private HashSet<GameObject> buildingsInRange = new HashSet<GameObject>();

    // Zmienna pozwalaj¹ca okreœliæ, czy aura ma dzia³aæ na konkretny prefab/budynek. 
    // Jeœli puste (null), dzia³a na wszystkie obiekty z tagiem "Bulding".
    public GameObject Forspecificbuilding;

    void Update()
    {
        // Sprawdzanie otoczenia wywo³ywane co klatkê (np. 60 razy na sekundê).
        DetectBuildings();
    }

    void DetectBuildings()
    {
        // 1. ZNALEZIENIE OBIEKTÓW W ZASIÊGU
        // Rysuje niewidzialne ko³o o promieniu 'detectionRadius' i zbiera wszystko, co ma Collider2D.
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        // Tymczasowa lista budynków wykrytych dok³adnie w TEJ jednej klatce gry.
        HashSet<GameObject> currentlyDetected = new HashSet<GameObject>();

        foreach (var hitCollider in hitColliders)
        {
            // Filtruje obiekty - interesuj¹ nas tylko te z tagiem "Bulding" (uwaga na literówkê w tagu).
            if (hitCollider.CompareTag("Bulding"))
            {
                // Zabezpieczenie: sprawdza, czy obiekt ma przypisany skrypt 'Bulding'. 
                // Jeœli nie, ignoruje go i idzie dalej.
                if (hitCollider.gameObject.GetComponent<Bulding>() == null)
                {
                    continue;
                }

                GameObject building = hitCollider.gameObject;

                // Dodaje budynek do tymczasowej listy aktualnie widocznych.
                currentlyDetected.Add(building);

                // WEJŒCIE W ZASIÊG (Odpowiednik OnTriggerEnter)
                // Jeœli tego budynku nie by³o w naszej g³ównej pamiêci, to znaczy, ¿e w³aœnie wszed³ w zasiêg.
                if (!buildingsInRange.Contains(building))
                {
                    buildingsInRange.Add(building); // Zapisz go w pamiêci
                    OnBuildingEnter(building);      // Nadaj mu bonus
                }
            }
        }

        // 2. OPUSZCZENIE ZASIÊGU (Odpowiednik OnTriggerExit)
        // Porównujemy nasz¹ sta³¹ pamiêæ (buildingsInRange) z tym, co widzimy teraz (currentlyDetected).
        List<GameObject> toRemove = new List<GameObject>();
        foreach (var building in buildingsInRange)
        {
            // Jeœli budynek by³ w zasiêgu, ale ju¿ go nie wykrywamy...
            if (!currentlyDetected.Contains(building))
            {
                toRemove.Add(building); // Zaznacz do usuniêcia
            }
        }

        // Fizyczne usuniêcie z pamiêci i zabranie bonusu.
        foreach (var building in toRemove)
        {
            buildingsInRange.Remove(building);
            OnBuildingExit(building); // Odbierz bonus
        }
    }

    // Funkcja wywo³ywana TYLKO RAZ w momencie wykrycia nowego budynku
    void OnBuildingEnter(GameObject building)
    {
        // Wykonaj tylko, jeœli nie zbieramy akurat innego typu boosta
        if (!ColectingSpeedBoost)
        {
            // Jeœli przypisano konkretny budynek w Inspektorze (np. tylko Tartak)...
            if (Forspecificbuilding != null)
            {
                // Sprawdza, czy nazwa wykrytego obiektu pokrywa siê z wymagan¹ (uwaga: Unity czêsto dodaje "(Clone)" do nazwy).
                if (building.name == Forspecificbuilding.name)
                {
                    building.gameObject.GetComponent<Bulding>().AdaptiveBoost += Convert.ToInt32(boostAmount);
                }
                return; // Przerwij dzia³anie funkcji, ¿eby nie wejœæ do bloku 'else'
            }
            else // Jeœli 'Forspecificbuilding' jest puste, daj bonus KA¯DEMU budynkowi w zasiêgu
            {
                building.gameObject.GetComponent<Bulding>().AdaptiveBoost += Convert.ToInt32(boostAmount);
            }
        }
    }

    // Funkcja wywo³ywana TYLKO RAZ, gdy budynek opuœci okr¹g (lub zostanie zniszczony)
    void OnBuildingExit(GameObject building)
    {
        if (!ColectingSpeedBoost)
        {
            // Analogiczna logika jak przy wejœciu, ale tutaj ODEJMUJEMY 'boostAmount', aby zresetowaæ stan budynku.
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

    // Metoda zintegrowana w Unity, która rysuje b³êkitny okr¹g w edytorze.
    // Pomaga to wizualnie ustawiæ wielkoœæ 'detectionRadius' bez w³¹czania gry.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

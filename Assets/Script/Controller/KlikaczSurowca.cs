using UnityEngine;

public class KlikaczSurowca : MonoBehaviour
{
    [Header("Co ma wyskakiwać?")]
    public GameObject prefabCzasteczki; // Tu włożymy prefab jagody/kamienia
    
    [Header("Gdzie to wrzucić?")]
    public Transform miejsceNaCzasteczki; // Tu przeciągnij swój Canvas (lub panel), żeby jagody się na nim rysowały

    // Tę funkcję podepniemy pod guzik!
    [Header("Dzwięki")]
    public AudioClip ClickTheSound;
    public void KliknietoSurowiec()
    {
        // Sprawdzamy, czy podpieliśmy prefab
        if (prefabCzasteczki != null && miejsceNaCzasteczki != null)
        {
            // 1. Odkrywamy pozycję myszki na ekranie
            Vector3 pozycjaMyszki = Input.mousePosition;

            // Zróbmy mały rozrzut, żeby nie leciały idealnie w jednej linii
            float losoweX = Random.Range(-30f, 30f);
            pozycjaMyszki.x += losoweX;

            // 2. Tworzymy naszą jagodę (prefab) w miejscu myszki
            Instantiate(prefabCzasteczki, pozycjaMyszki, Quaternion.identity, miejsceNaCzasteczki);
        }
    }
}
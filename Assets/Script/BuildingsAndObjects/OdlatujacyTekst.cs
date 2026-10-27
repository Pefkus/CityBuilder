using UnityEngine;
using UnityEngine.UI;

// Skrypt odpowiadający za animację odlatującego i znikającego powiadomienia (np. zebranej rudy czy drewna)
public class OdlatujacyTekst : MonoBehaviour
{
    [Header("Ustawienia animacji")]
    public float szybkoscLotu = 100f; // Prędkość, z jaką obiekt przesuwa się w osi Y (do góry)
    public float czasZycia = 1f;      // Czas w sekundach, po którym obiekt zostanie zniszczony

    private Image obrazek; // Referencja do komponentu Image (ikonki surowca)
    private Color kolor;   // Zmienna przechowująca aktualny kolor (używana do modyfikacji przezroczystości)

    void Start()
    {
        // Pobiera komponent Image doczepiony do tego samego obiektu
        obrazek = GetComponent<Image>();

        // Zapisuje początkowy kolor obrazka (w tym jego kanał Alpha odpowiadający za widoczność)
        kolor = obrazek.color;

        // Automatyczne zniszczenie: funkcja Destroy pozwala przekazać drugi parametr (czasZycia).
        // Dzięki temu obiekt sam usunie się ze sceny po upływie określonego czasu.
        Destroy(gameObject, czasZycia);
    }

    void Update()
    {
        // 1. RUCH DO GÓRY
        // Przesuwa obiekt w górę (Vector3.up to to samo co new Vector3(0, 1, 0)).
        // Mnożymy przez Time.deltaTime, aby ruch był płynny i niezależny od liczby klatek na sekundę (FPS).
        transform.position += Vector3.up * szybkoscLotu * Time.deltaTime;

        // 2. PŁYNNE ZNIKANIE (FADE OUT)
        // Odejmujemy wartość od kanału 'a' (Alpha). 
        // Dzielenie Time.deltaTime przez czasZycia sprawia, że przezroczystość idealnie spadnie z 1.0 (100%) do 0.0 (0%) 
        // dokładnie w momencie, w którym obiekt ma zostać zniszczony.
        kolor.a -= Time.deltaTime / czasZycia;

        // Przypisanie zmodyfikowanego koloru (z nową, mniejszą wartością Alpha) z powrotem do komponentu Image.
        obrazek.color = kolor;
    }
}
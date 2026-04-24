using UnityEngine;

public class PlywajacyObiekt : MonoBehaviour
{
    [Header("Ustawienia lewitacji")]
    public float szybkosc = 2f;    // Jak szybko ma się poruszać
    public float wysokosc = 10f;   // Jak wysoko ma podskakiwać

    private RectTransform rectTransform;
    private float startY;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startY = rectTransform.anchoredPosition.y; // Zapisujemy pozycję startową
    }

    void Update()
    {
        // Wyliczamy nową pozycję używając fali matematycznej (Sinus)
        float nowaPozycjaY = startY + Mathf.Sin(Time.time * szybkosc) * wysokosc;
        
        // Aktualizujemy pozycję
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, nowaPozycjaY);
    }
}
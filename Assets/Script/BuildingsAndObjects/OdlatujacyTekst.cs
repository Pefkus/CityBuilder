using UnityEngine;
using UnityEngine.UI;

public class OdlatujacyTekst : MonoBehaviour
{
    public float szybkoscLotu = 100f; // Jak szybko leci do góry
    public float czasZycia = 1f;      // Po jakim czasie znika
    
    private Image obrazek;
    private Color kolor;

    void Start()
    {
        obrazek = GetComponent<Image>();
        kolor = obrazek.color;
        
        // Zniszcz ten obiekt po upływie 'czasZycia'
        Destroy(gameObject, czasZycia);
    }

    void Update()
    {
        // Lecimy do góry!
        transform.position += Vector3.up * szybkoscLotu * Time.deltaTime;

        // Powolne znikanie (blednięcie)
        kolor.a -= Time.deltaTime / czasZycia;
        obrazek.color = kolor;
    }
}
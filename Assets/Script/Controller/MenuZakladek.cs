using UnityEngine;
using UnityEngine.UI;

public class MenuZakladek : MonoBehaviour
{
    [Header("Przyciski zakładek (te 5 u góry):")]
    public Button[] przyciski;

    [Header("Panele do włączania (treść zakładek):")]
    public GameObject[] panele;

    [Header("Wygląd (opcjonalnie):")]
    public Color kolorAktywny = new Color(0.8f, 0.8f, 0.8f); // Lekko szary dla wciśniętego
    public Color kolorNieaktywny = Color.white;              // Biały dla reszty

    void Start()
    {
        // Automatyczne przypisanie kliknięć! Nie musisz wyklikiwać "On Click" w Inspektorze.
        for (int i = 0; i < przyciski.Length; i++)
        {
            int index = i; // Zabezpieczenie zmiennej dla delegata
            if (przyciski[i] != null)
            {
                przyciski[i].onClick.AddListener(() => WlaczZakladke(index));
            }
        }

        // Włączamy domyślnie pierwszą zakładkę na starcie gry
        if (przyciski.Length > 0 && panele.Length > 0)
        {
            WlaczZakladke(0);
        }
    }

    public void WlaczZakladke(int index)
    {
        for (int i = 0; i < panele.Length; i++)
        {
            // 1. Włączamy/wyłączamy odpowiedni panel
            if (panele[i] != null)
            {
                panele[i].SetActive(i == index);
            }

            // 2. Podmieniamy kolor przycisku, żeby było widać, który jest aktywny
            if (i < przyciski.Length && przyciski[i] != null)
            {
                Image tloPrzycisku = przyciski[i].GetComponent<Image>();
                if (tloPrzycisku != null)
                {
                    tloPrzycisku.color = (i == index) ? kolorAktywny : kolorNieaktywny;
                }
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TabManager : MonoBehaviour
{
    [Header("Panele (Strony):")]
    public GameObject[] tabPages;

    [Header("Komponenty Image przycisków:")]
    public Image[] tabButtons;

    [Header("Rysunki (Sprite'y):")]
    public Sprite spriteAktywny;
    public Sprite spriteNieaktywny;

    [Header("Kolory Tekstu (TextMeshPro):")]
    public Color kolorTekstuAktywnego = Color.black; 
    public Color kolorTekstuNieaktywnego = Color.white;

    [Header("Blokowanie Górnych Zakładek:")]
    public CanvasGroup gorneZakladki;

    void Start()
    {
        SwitchTab(0);
    }

    public void SwitchTab(int tabIndex)
    {
        if (gorneZakladki != null)
        {
            if (tabIndex == 1) 
                gorneZakladki.gameObject.SetActive(false);
            else 
                gorneZakladki.gameObject.SetActive(true);
        }

        for (int i = 0; i < tabPages.Length; i++)
        {
            tabPages[i].SetActive(i == tabIndex);

            if (i < tabButtons.Length && tabButtons[i] != null)
            {
                TextMeshProUGUI tekstPrzycisku = tabButtons[i].GetComponentInChildren<TextMeshProUGUI>();

                if (i == tabIndex)
                {
                    tabButtons[i].sprite = spriteAktywny;
                    if (tekstPrzycisku != null) tekstPrzycisku.color = kolorTekstuAktywnego;
                }
                else
                {
                    tabButtons[i].sprite = spriteNieaktywny;
                    if (tekstPrzycisku != null) tekstPrzycisku.color = kolorTekstuNieaktywnego;
                }
                
                tabButtons[i].color = Color.white;
            }
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenedzerMenu : MonoBehaviour
{
    [Header("Ustawienia Muzyki")]
    public AudioSource głośnikZMuzyką; 


    
    public void NowaGra()
    {
        Debug.Log("Ładuję mapę!");

        SceneManager.LoadScene("Kotki"); 
    }

    public void WczytajGre()
    {

        if (PlayerPrefs.HasKey("ZapisanyPoziom"))
        {
            string mapaDoWczytania = PlayerPrefs.GetString("ZapisanyPoziom");
            Debug.Log("Znaleziono zapis! Wczytuję: " + mapaDoWczytania);
            SceneManager.LoadScene(mapaDoWczytania);
        }
        else
        {
            Debug.Log("Brak zapisanej gry! Kliknij Nowa Gra.");
        }
    }

    public void WyjdzZGry()
    {
        Debug.Log("Wychodzę z gry!");
        Application.Quit();
    }

    public void ZmienGlosnosc(float wartoscSuwaka)
    {
        if (głośnikZMuzyką != null)
        {
            głośnikZMuzyką.volume = wartoscSuwaka;
        }
    }


    public void ZmienPelnyEkran(bool czyZaznaczony)
    {
        Screen.fullScreen = czyZaznaczony;
        Debug.Log("Pełny ekran: " + czyZaznaczony);
    }
}
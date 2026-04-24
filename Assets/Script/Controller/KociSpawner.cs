using UnityEngine;

public class KociSpawner : MonoBehaviour
{
    [Header("Co chcemy klonować?")]
    public GameObject szablonKota;

    [Header("Gdzie mają się pojawiać?")]
    public RectTransform obszarKamieni;

    public void DodajNowegoKota()
    {

        GameObject nowyKot = Instantiate(szablonKota, obszarKamieni);
        RectTransform rtKota = nowyKot.GetComponent<RectTransform>();

        rtKota.anchorMin = new Vector2(0.5f, 0.5f);
        rtKota.anchorMax = new Vector2(0.5f, 0.5f);
        rtKota.pivot = new Vector2(0.5f, 0.5f);
        nowyKot.transform.localScale = Vector3.one;

        float szerokosc = obszarKamieni.rect.width;
        float wysokosc = obszarKamieni.rect.height;

        if (szerokosc <= 10f) szerokosc = 300f; 
        if (wysokosc <= 10f) wysokosc = 200f;   

        float losowyX = Random.Range(-szerokosc / 2.2f, szerokosc / 2.2f);
        float losowyY = Random.Range(-wysokosc / 2.2f, wysokosc / 2.2f);

        rtKota.anchoredPosition = new Vector2(losowyX, losowyY);
    }
}
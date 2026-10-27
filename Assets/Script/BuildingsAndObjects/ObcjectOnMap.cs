using UnityEngine;
using UnityEngine.UI;



public class ObcjectOnMap : MonoBehaviour
{
    [Header("Statystyki Z³o¿a")]
    public int DurabilityAmount;  // Aktualna wytrzyma³oœæ z³o¿a (odpowiednik HP)
    int MaxAmountCanProduce;       // Bazowa iloœæ surowca przekazywana graczu po zniszczeniu

    [Header("Surowiec i Efekty")]
    public GameObject ItemProdusing;  // Prefab surowca, jaki gracz otrzyma (np. Drewno, Kamieñ)
    public GameObject PopUp;         // Prefab efektu cz¹steczkowego z ikon¹ zebranego przedmiotu
    public GameObject DestroyPS;    // Prefab efektu cz¹steczkowego samego zniszczenia obiektu

    [Header("Interfejs UI")]
    public GameObject ContainerForSlider;  // Rodzic/Pojemnik paska postêpu (ukryty na start, pojawia siê po 1. uderzeniu)
    public Slider slider;          // Pasek wizualizuj¹cy pozosta³¹ wytrzyma³oœæ

    [Header("Dzwiêki")]
    public AudioClip ClickTheSound;   // DŸwiêk uderzenia w obiekt

    void Start()
    {
        // Pobranie poziomu g³ównego budynku z Singletona
        int lvl = TypesOfBuildingMenager.Instance.MainBuilding.GetComponent<MainBuilding>().LevelOfBuilding;
        // Wytrzyma³oœæ roœnie kwadratowo z poziomem budynku (np. Lvl 1: 10-25 HP, Lvl 2: 40-100 HP)
        DurabilityAmount = Random.Range(10 * (lvl * lvl), 25 * (lvl * lvl));
        // Wyliczenie nagrody: 2x wiêcej surowca ni¿ wynosi³a bazowa wytrzyma³oœæ
        MaxAmountCanProduce = DurabilityAmount * 2;
        // Ustawienie maksymalnej wartoœci paska postêpu
        slider.maxValue = DurabilityAmount;
    }
    private void Update()
    {
        // Aktualizacja paska postêpu i jego koloru co klatkê
        ChangeSlider();
    }
    // Wywo³ywane, gdy gracz klika/uderza w obiekt na mapie
    public void PunchingTheObject(int Clik)

    {
        // Po pierwszym uderzeniu pokazujemy pasek HP nad obiektem
        ContainerForSlider.SetActive(true);
        // Zmniejszamy wytrzyma³oœæ o wartoœæ si³y klikniêcia + 1
        DurabilityAmount += -Clik - 1;
        // Jeœli wytrzyma³oœæ spadnie poni¿ej zero, przyznajemy nagrodê i niszczymy z³o¿e
        if (DurabilityAmount < 0)
        {
            GiveTheItem(Clik);
        }
    }
    // Przekazanie nagrody do ekwipunku
    void GiveTheItem(int Boost)

    {
        // Zmniejszenie licznika obiektów na mapie, aby generator móg³ zespawnowaæ nowe
        MapGeneretor.Instance.Limit--;
        // Dodanie surowca do ekwipunku gracza (iloœæ bazowa + bonus z klikniêcia)
        InventoryManager.Instance.ChangeValueOfItemInInventory(ItemProdusing, MaxAmountCanProduce + Boost);
        // Stworzenie efektów i usuniêcie obiektu
        CreatePopItem();
    }
    // Odtworzenie efektów wizualnych i zniszczenie obiektu
    void CreatePopItem()
    {
        if (PopUp != null)
        {
            // Podmiana tekstury w Particle Systemie na grafikê wydobytego surowca
            PopUp.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, ItemProdusing.GetComponent<SpriteRenderer>().sprite);
            // Spawnowanie efektów w miejscu z³o¿a
            Instantiate(PopUp, transform.position, Quaternion.identity);
            Instantiate(DestroyPS, transform.position, Quaternion.identity);
            // Usuniêcie z³o¿a z mapy
            Destroy(this.gameObject);
        }
    }
    // Odœwie¿enie wartoœci slidera oraz zmiana koloru w zale¿noœci od % HP
    void ChangeSlider()
    {
        slider.value = DurabilityAmount;
        // Poni¿ej 25% HP -> Czerwony
        if (slider.value <= slider.maxValue * 0.25f)
        {
            slider.fillRect.GetComponent<Image>().color = Color.red;
        }
        // Poni¿ej 50% HP -> ¯ó³ty
        else if (slider.value <= slider.maxValue * 0.50f)
        {
            slider.fillRect.GetComponent<Image>().color = Color.yellow;
        }
        // Powy¿ej 50% HP -> Zielony
        else
        {
            slider.fillRect.GetComponent<Image>().color = Color.green;
        }
    }
}
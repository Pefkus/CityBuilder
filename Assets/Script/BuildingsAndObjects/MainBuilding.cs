using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MainBuilding : MonoBehaviour
{
    [Header("What item needed to upgrade")]
    // Listy przechowuj¹ce wymogi do ulepszenia budynku.
    public List<GameObject> ItemNeededRightNow = new List<GameObject>(); // Co jest potrzebne na ten moment
    public List<int> AmountOfItemNeeded = new List<int>();               // W jakich iloœciach

    // Konkretne prefabrykaty/surowce wymagane do przejœcia na poziom 2 i 3
    public List<GameObject> ItemNeededLvL2 = new List<GameObject>();
    public List<GameObject> ItemNeededLvL3 = new List<GameObject>();
    public int AmountItemsNeeded; // Bazowa iloœæ wymaganych przedmiotów, mno¿ona potem przez poziom

    [Header("UI")]
    public GameObject Canvas;      // Interfejs budynku (prawdopodobnie nad nim)
    public Slider slider;          // Pasek postêpu zbierania surowców do ulepszenia
    public GameObject Image;       // Prefabrykat ikonki surowca do wygenerowania w UI
    public List<GameObject> Images = new List<GameObject>(); // Lista wygenerowanych ikonek (aby je potem usun¹æ)
    public GameObject Contenrer;   // Kontener (Rodzic - np. Horizontal Layout Group) dla ikonek. (Uwaga: literówka 'Contenrer' zamiast 'Container')

    [Header("Sprites")]
    public SpriteRenderer MainSprite; // Komponent odpowiadaj¹cy za wygl¹d budynku
    public Sprite[] Sprites;          // Tablica grafik dla poszczególnych poziomów (indeks 0 to lvl 1, 1 to lvl 2 itd.)

    [Header("Settings")]
    public int LevelOfBuilding = 1; // Aktualny poziom budynku
    public int FoodAmount = 300;    // Limit jedzenia dodawany przez ten budynek
    public int People = 2;          // Limit populacji dodawany przez ten budynek

    private void Start()
    {
        AditionalChange(); // Inicjalizuje startowe statystyki
        LevelOfBuilding = 1; // Wymusza 1 poziom na start (nadpisuje ewentualne zmiany z AditionalChange)
    }

    private void Update()
    {
        // Aktualizuje pasek postêpu co klatkê
        ChangeValueSlider();
        SliderChangeColllor();
    }

    // Funkcja wywo³ywana przy awansie (oraz na start), aktualizuje statystyki globalne
    public void AditionalChange()
    {
        // UWAGA DO LOGIKI: Jeœli LevelOfBuilding wynosi 3, wykona siê ten blok 'if (<= 3)', 
        // a blok 'else if(LevelOfBuilding == 3)' na dole NIGDY siê nie wywo³a.
        if (LevelOfBuilding <= 3)
        {
            // Pobiera bazowe wartoœci ze skryptu StorageBuilding podpiêtego pod ten sam obiekt i mno¿y przez poziom
            FoodAmount = gameObject.GetComponent<StorageBuilding>().AditionalFoodStorage * LevelOfBuilding;
            People = gameObject.GetComponent<StorageBuilding>().MaxPeopleStorage * LevelOfBuilding;

            // Zmienia grafikê na odpowiedni¹ dla danego poziomu
            MainSprite.sprite = Sprites[LevelOfBuilding - 1];

            // Aktualizuje globalne limity w Singletonach (Mened¿erach gry)
            FoodController.Instance.ChangeMaxFoodAmount(FoodAmount);
            NPCController.Instance.ChangePeopleStorage(People);
            MapGeneretor.Instance.LimitObjects += 10;

            // Zwiêksza strefê dzia³ania budynku
            gameObject.GetComponent<TypeOfBuilding>().BuildingRadius = 15f * LevelOfBuilding;

            // Odœwie¿a UI z nowymi wymaganiami
            ChangeNeededItems();
        }
        else if (LevelOfBuilding == 3) // Ten warunek jest martwy, bo 'LevelOfBuilding <= 3' wy³apuje trójkê wy¿ej.
        {
            Canvas.SetActive(false); // Mia³o ukrywaæ UI po osi¹gniêciu maksymalnego poziomu.
        }
    }

    // Podmienia wymogi w zale¿noœci od poziomu, na który chcemy awansowaæ
    void ChangeNeededItems()
    {
        // Czyszczenie starych list z poprzedniego poziomu
        ItemNeededRightNow.Clear();
        AmountOfItemNeeded.Clear();
        foreach (GameObject img in Images)
        {
            Destroy(img); // Usuwa stare ikonki z UI
        }
        Images.Clear();

        // Ustalanie nowych potrzeb
        if (LevelOfBuilding == 1)
        {
            foreach (GameObject item in ItemNeededLvL2)
                ItemNeededRightNow.Add(item);
        }
        else if (LevelOfBuilding == 2)
        {
            foreach (GameObject item in ItemNeededLvL3)
                ItemNeededRightNow.Add(item);
        }

        // Generowanie nowych ikonek w UI i obliczanie docelowej iloœci
        foreach (GameObject item in ItemNeededRightNow)
        {
            // Formu³a kosztu: BazowyKoszt * Poziom * 2
            AmountOfItemNeeded.Add(AmountItemsNeeded * LevelOfBuilding * 2);

            // Tworzenie ikonki w kontenerze UI
            GameObject img = Instantiate(Image, Contenrer.transform);
            Images.Add(img);

            // Pobieranie sprite'a wprost z prefaba surowca
            img.GetComponent<Image>().sprite = item.GetComponent<SpriteRenderer>().sprite;
            // Wypisywanie wymaganej iloœci pod ikonk¹
            img.GetComponentInChildren<TextMeshProUGUI>().text = AmountOfItemNeeded[ItemNeededRightNow.IndexOf(item)].ToString();
        }
        ChangeMaxValueSlider(); // Ustawia maks na pasku postêpu
    }

    // Sumuje wszystkie potrzebne surowce do jednej liczby (maksymalna wartoœæ paska)
    void ChangeMaxValueSlider()
    {
        float amount = 0f;
        foreach (GameObject item in ItemNeededRightNow)
        {
            amount += AmountOfItemNeeded[ItemNeededRightNow.IndexOf(item)];
        }
        slider.maxValue = amount;
    }

    // Oblicza, ile surowców gracz ju¿ posiada i wype³nia pasek
    void ChangeValueSlider()
    {
        float amount = 0;
        foreach (GameObject item in ItemNeededRightNow)
        {
            // Jeœli gracz ma mniej surowca ni¿ potrzeba, dodaj to co ma
            if (InventoryManager.Instance.GetValueOfItemInInventory(item) <= AmountOfItemNeeded[ItemNeededRightNow.IndexOf(item)])
            {
                amount += InventoryManager.Instance.GetValueOfItemInInventory(item);
            }
            else // Jeœli ma wiêcej (nadmiar), dodaj tylko tyle, ile jest WYMAGANE (aby suwak nie wyszed³ poza skale)
            {
                amount += AmountOfItemNeeded[ItemNeededRightNow.IndexOf(item)];
            }
        }
        slider.value = amount;
    }

    // Dynamiczna zmiana koloru paska w zale¿noœci od postêpu (%)
    public void SliderChangeColllor()
    {
        if (slider.value <= slider.maxValue * 0.49f)
        {
            slider.fillRect.GetComponent<Image>().color = Color.red;
        }
        else if (slider.value <= slider.maxValue * 0.99f)
        {
            slider.fillRect.GetComponent<Image>().color = Color.yellow;
        }
        else // Gdy pe³ny (mo¿na ulepszyæ)
        {
            slider.fillRect.GetComponent<Image>().color = Color.green;
        }
    }

    // Funkcja wywo³ywana np. przez przycisk UI. Sprawdza, czy staæ nas na ulepszenie.
    public void CheckItems()
    {
        bool CanUpgrade = true;

        // Faza 1: Sprawdzenie, czy brakuje chocia¿ jednego surowca
        foreach (GameObject item in ItemNeededRightNow)
        {
            if (AmountOfItemNeeded[ItemNeededRightNow.IndexOf(item)] > InventoryManager.Instance.GetValueOfItemInInventory(item))
            {
                CanUpgrade = false;
                break; // Jeœli brakuje, od razu przerywa pêtle
            }
        }

        // Faza 2: Jeœli nas staæ, pobierz surowce i ulepsz budynek
        if (CanUpgrade)
        {
            foreach (GameObject item in ItemNeededRightNow)
            {
                // Odejmuje potrzebn¹ wartoœæ z ekwipunku (podaj¹c ujemn¹ liczbê)
                InventoryManager.Instance.ChangeValueOfItemInInventory(item, -AmountOfItemNeeded[ItemNeededRightNow.IndexOf(item)]);
            }
            LevelOfBuilding++; // Awans!
            AditionalChange(); // Odœwie¿enie statystyk

            // Punkt drzewka presti¿u / odrodzenia dla gracza
            UpgradeController.Instance.TokensForRebirth++;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
public class InventoryManager : MonoBehaviour
{
    // Singleton, ¿eby mieæ ³atwy dostêp do ekwipunku z ka¿dego miejsca
    public static InventoryManager Instance { get; private set; }

    // Lista slotów w ekwipunku, które mog¹ byæ przypisane do przedmiotów
    public List<GameObject> itemslots = new List<GameObject>(); 

    // Lista nazw przedmiotów w ekwipunku
    public List<GameObject> itemsInInventory = new List<GameObject>();

    // Lista iloœci ka¿dego przedmiotu w ekwipunku, indeks odpowiada indeksowi w itemsInInventory
    public List<int> amountOfItemsInInventory = new List<int>();

    public List<GameObject> AllFood = new List<GameObject>();
    public List<GameObject> CurrentFood = new List<GameObject>();
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // Inicjalizacja iloœci przedmiotów w ekwipunku na 0        UwAGA: za³o¿enie, ¿e itemsInInventory jest ju¿ wype³nione nazwami przedmiotów, które mog¹ byæ w ekwipunku
        foreach (GameObject item in itemsInInventory)
        {
            amountOfItemsInInventory.Add(0);
        }
        int index = 0;
        foreach (GameObject slot in itemslots)
        {
            slot.name = "EmptySlot " + index;
            slot.GetComponent<Image>().enabled = false;
            slot.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            index++;
        }
        foreach (GameObject item in itemsInInventory)
        {
            if(item.CompareTag("Food"))
            {
                AllFood.Add(item);
            }
        }
    }
    public GameObject GetRandomItemFood()
    {
        GameObject food = null;
        if (CurrentFood.Count == 0)
        {
            food = AllFood.Find(x => x.name == "jagoda");
        }
        else
        {
            int number = Random.Range(0, CurrentFood.Count);
            food = CurrentFood[number];
        }
        return food;
    }
    // Funkcja do pobierania iloœci danego przedmiotu w ekwipunku
    public int GetValueOfItemInInventory(GameObject name)
    {
        GameObject item = itemsInInventory.Find(x => x.name == name.name);
        if (item == null)
        {
            Debug.LogError("Nieistnieje item o nazwie: " + name);
            return 0;
        }
        return amountOfItemsInInventory[itemsInInventory.IndexOf(item)];
    }
    // Funkcja do zmiany iloœci danego przedmiotu w ekwipunku (dodawanie lub odejmowanie)
    public void ChangeValueOfItemInInventory(GameObject name, int amount)
    {
        GameObject item = itemsInInventory.Find(x => x.name == name.name);
        if (item == null)
        {
            Debug.LogError("Nieistnieje item o nazwie: " +name );
            return;
        }
        amountOfItemsInInventory[itemsInInventory.IndexOf(item)] += amount;
        if(amountOfItemsInInventory[itemsInInventory.IndexOf(item)] < 0)
        {
            amountOfItemsInInventory[itemsInInventory.IndexOf(item)] = 0;
        }
        ItHasAlreadyASlot(item);
    }
    public void ChangeValueOfItemInInventoryTo(GameObject name, int amount)
    {
        GameObject item = itemsInInventory.Find(x => x.name == name.name);
        if (item == null)
        {
            Debug.LogError("Nieistnieje item o nazwie: " + name);
            return;
        }
        amountOfItemsInInventory[itemsInInventory.IndexOf(item)] = amount;
        ItHasAlreadyASlot(item);
    }
    // Funkcja do sprawdzania, czy przedmiot ju¿ ma przypisany slot w ekwipunku, jeœli tak to aktualizuje jego iloœæ, jeœli nie to szuka pustego slotu
    void ItHasAlreadyASlot(GameObject item)
    {
        foreach (GameObject slot in itemslots)
        {
            if (slot.name == item.name)
            {
                ChangingTxtOfAmount(item, slot);
                return;
            }
        }
        SearchForEmptySlot(item);
    }
    // Funkcja do zmiany wygl¹du slotu w ekwipunku, przypisuje mu obraz przedmiotu i aktualizuje iloœæ
    void ChangeImageaAndTextOfSlot(GameObject slot, GameObject item)
    {
        if (item != null) 
        { 
            slot.GetComponent<Image>().enabled = true;
            slot.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
            ChangingTxtOfAmount(item, slot);
            slot.name = item.name;
            slot.GetComponent<Image>().sprite = item.GetComponent<SpriteRenderer>().sprite;
        }
    }
    // Funkcja do szukania pustego slotu w ekwipunku i przypisania mu przedmiotu
    void SearchForEmptySlot(GameObject item)
    {
        foreach (GameObject slot in itemslots)
        {
            if (slot.GetComponent<Image>().enabled == false)
            {
                ChangeImageaAndTextOfSlot(slot, item);
                return;
            }
        }
    }
    void ChangingTxtOfAmount(GameObject item, GameObject slot)
    {
        long amount = GetValueOfItemInInventory(item);
        switch (amount)
        {
            case >= 1000000000000:
                slot.GetComponentInChildren<TextMeshProUGUI>().text = (amount / 1000000000000).ToString() + "T";
                break;
            case >= 1000000000:
                slot.GetComponentInChildren<TextMeshProUGUI>().text = (amount / 1000000000).ToString() + "B";
                break;
            case >= 1000000:
                slot.GetComponentInChildren<TextMeshProUGUI>().text = (amount / 1000000).ToString() + "M";
                break;
            case >= 1000:
                slot.GetComponentInChildren<TextMeshProUGUI>().text = (amount / 1000).ToString() + "K";
                break;
            default:
                slot.GetComponentInChildren<TextMeshProUGUI>().text = amount.ToString();
                break;
        }
        return;
           
    }
    private void Update()
    {
        //sprawdzanie czy iloœæ przedmiotów w ekwipunku jest wiêksza ni¿ 0, jeœli tak to aktualizuje wygl¹d slotu, jeœli nie to resetuje slot do pustego
        foreach (GameObject item in itemsInInventory)
        {
            if (GetValueOfItemInInventory(item) > 0)
            {
                ItHasAlreadyASlot(item);
            }
        }
        foreach (GameObject item in AllFood)
        {
            if (GetValueOfItemInInventory(item) > 0)
            {
                if (!CurrentFood.Contains(item))
                {
                    CurrentFood.Add(item);
                }
            }
            else
            {
                if (CurrentFood.Contains(item))
                {
                    CurrentFood.Remove(item);
                }
            }
        }
    }
}

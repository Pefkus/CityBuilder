using UnityEngine;
using System.Collections.Generic;
public class InventoryManager : MonoBehaviour
{
    // Singleton, ¿eby mieæ ³atwy dostêp do ekwipunku z ka¿dego miejsca
    public static InventoryManager Instance { get; private set; }

    // Lista nazw przedmiotów w ekwipunku
    public List<GameObject> itemsInInventory = new List<GameObject>();

    // Lista iloœci ka¿dego przedmiotu w ekwipunku, indeks odpowiada indeksowi w itemsInInventory
    public List<int> amountOfItemsInInventory = new List<int>();

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        // Inicjalizacja iloœci przedmiotów w ekwipunku na 0        UwAGA: za³o¿enie, ¿e itemsInInventory jest ju¿ wype³nione nazwami przedmiotów, które mog¹ byæ w ekwipunku
        foreach (GameObject item in itemsInInventory)
        {
            amountOfItemsInInventory.Add(0);
        }
    }
    // Funkcja do pobierania iloœci danego przedmiotu w ekwipunku
    public int GetValueOfItemInInventory(GameObject name)
    {
        if (!itemsInInventory.Contains(name))
        {
            Debug.LogError("Nieistnieje item o nazwie: " + name);
            return 0;
        }
        return amountOfItemsInInventory[itemsInInventory.IndexOf(name)];
    }
    // Funkcja do zmiany iloœci danego przedmiotu w ekwipunku (dodawanie lub odejmowanie)
    public void ChangeValueOfItemInInventory(GameObject name, int amount)
    {
        if (!itemsInInventory.Contains(name))
        {
            Debug.LogError("Nieistnieje item o nazwie: " +name );
            return;
        }
        amountOfItemsInInventory[itemsInInventory.IndexOf(name)] += amount;
    }
    
}

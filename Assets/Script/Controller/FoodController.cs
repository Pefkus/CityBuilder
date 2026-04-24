using UnityEngine;
using UnityEngine.UI;

public class FoodController : MonoBehaviour
{
    public static FoodController Instance { get; private set; }
    [Header("Food Settings in kgs")]
    public int MaxFoodAmount;
    public Slider FoodSlider;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameObject[] BuildingsOnMap = GameObject.FindGameObjectsWithTag("Bulding");
        foreach (GameObject Mainbuilding in BuildingsOnMap)
        {
            if (Mainbuilding.GetComponent<TypeOfBuilding>() != null && Mainbuilding.GetComponent<TypeOfBuilding>().MainBuilding)
            {
                if (Mainbuilding.GetComponent<StorageBuilding>() != null)
                {
                    MaxFoodAmount = Mainbuilding.GetComponent<StorageBuilding>().AditionalFoodStorage;
                    FoodSlider.maxValue = MaxFoodAmount;
                    break;
                }
            }
        }
    }
    // Funkcja do zmiany maksymalnej iloœci jedzenia, która jest przechowywana, i aktualizacja suwaka, ¿eby odzwierciedla³ tê zmianê
    public void ChangeMaxFoodAmount(int amount)
    {
        MaxFoodAmount += amount;
        FoodSlider.maxValue = MaxFoodAmount;
    }
    public void ChangeFoodAmountTo(float amount)
    {
        FoodSlider.value = amount;
    }
    public float GetCurrentFoodAmount()
    {
        return FoodSlider.value;
    }
    public float GetCurrentKgOfCurrentFood(GameObject food)
    {
        float unit =  InventoryManager.Instance.GetValueOfItemInInventory(food) * food.GetComponent<Food>().KgPerUnit ;
        return unit;
    }

    // Funkcja do zmiany koloru suwaka w zale¿noœci od aktualnej iloœci jedzenia, ¿eby ³atwiej by³o zobaczyæ, kiedy zaczyna go brakowaæ
    void ChangeSlider()
    {
        if (FoodSlider.value <= MaxFoodAmount * 0.25f)
        {
            FoodSlider.fillRect.GetComponent<Image>().color = Color.red;
            
        }
        else if (FoodSlider.value <= MaxFoodAmount * 0.5f)
        {
            FoodSlider.fillRect.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            FoodSlider.fillRect.GetComponent<Image>().color = Color.green;
        }
    }
    // Funkcja do jedzenia jedzenia, która sprawdza, czy jest wystarczaj¹co jedzenia w suwaku, a nastêpnie szuka przedmiotu z tagiem "Food" w ekwipunku i odejmuje z niego okreœlon¹ iloœæ, a tak¿e zmienia iloœæ jedzenia w suwaku
    public void EatTheFood(float AmountInKg, GameObject food)
    {
        if (InventoryManager.Instance.GetValueOfItemInInventory(food) > 0)
        {
            int CorrectUnitOfFood = Mathf.RoundToInt(AmountInKg / food.GetComponent<Food>().KgPerUnit);
            InventoryManager.Instance.ChangeValueOfItemInInventory(food, -CorrectUnitOfFood);
        }
    }
    
    public void ChangevalueSlider()
    {
        float amonut = 0;
        foreach(GameObject food in InventoryManager.Instance.CurrentFood)
        {
            Food foodScript = food.GetComponent<Food>();
            if (foodScript != null)
            {
                float AmountInKg = InventoryManager.Instance.GetValueOfItemInInventory(food) * foodScript.KgPerUnit;
                amonut += AmountInKg;
            }
        }
        FoodSlider.value = amonut;
    }
    void Update()
    {
        ChangevalueSlider();
        ChangeSlider();
    }
}

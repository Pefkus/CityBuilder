using UnityEngine;
using UnityEngine.UI;

public class FoodController : MonoBehaviour
{
    public static FoodController Instance { get; private set; }
    [Header("Food Settings in kgs")]
    public int MaxFoodAmount;
    public int MaxPeopleStorage;
    public Slider FoodSlider;

    float timer = 0f;
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
                    MaxPeopleStorage = Mainbuilding.GetComponent<StorageBuilding>().MaxPeopleStorage;
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
    public void ChangeFoodAmount(float amount, GameObject food)
    {
        if (food != null)
        {
            Food foodScript = food.GetComponent<Food>();
            if (foodScript != null)
            {
                amount *= foodScript.KgPerUnit;
                FoodSlider.value += amount;
            }
        }
    }
    public void ChangeFoodAmountTo(float amount)
    {
        FoodSlider.value = amount;
    }
    public float GetCurrentFoodAmount()
    {
        return FoodSlider.value;
    }


    // Funkcja do zmiany koloru suwaka w zale¿noœci od aktualnej iloœci jedzenia, ¿eby ³atwiej by³o zobaczyæ, kiedy zaczyna go brakowaæ
    void ChangeSliderColor()
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
    // Funkcja do jedzenia jedzenia, która sprawdza, czy jest wystarczaj¹co jedzenia w suwaku, a nastêpnie szuka pierwszego przedmiotu z tagiem "Food" w ekwipunku i odejmuje z niego okreœlon¹ iloœæ, a tak¿e zmienia iloœæ jedzenia w suwaku
    void EatTheFood(int amount)
    {
        if (FoodSlider.value > 0)
        {
            foreach(GameObject food in InventoryManager.Instance.itemsInInventory)
            {
                if (food.CompareTag("Food") && InventoryManager.Instance.GetValueOfItemInInventory(food) > 0)
                {
                    InventoryManager.Instance.ChangeValueOfItemInInventory(food, -amount);
                    ChangeFoodAmount(-amount, food);
                    break;
                }
            }
        }
    }



    public void ChangePeopleStorage(int storage) {  
        MaxPeopleStorage += storage;
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            timer = 0f;
            EatTheFood(1);
        }
        ChangeSliderColor();
    }
}

using UnityEngine;

public class NPC : MonoBehaviour
{
    private MovmentNPC movment;
    public GameObject FavoriteFood;
    private InventoryManager Inventory;
    private FoodController FoodController;
    public int AmountOfEat = 1;
    public bool HardWorking = false;
    public bool Mad = false;
    public float Happines = 100f;
    float timer = 0f;
    public float EatTimer = 10f;
    private void Start()
    {
        Inventory = InventoryManager.Instance;
        movment = GetComponent<MovmentNPC>();
        FoodController = FoodController.Instance;
    }
    void EatFavoriteFood()
    {
        if(FavoriteFood != null)
        {
            //Jeœli ma swoje ulubione jedzenie w inventoy to je zjada jeœli niema go w inventory to zjda pierwsze z brzegu i jest smutniejszy ;-;
            if(Inventory.GetValueOfItemInInventory(FavoriteFood) >= AmountOfEat)
            {
                FoodController.EatTheFood(AmountOfEat, FavoriteFood);
                ChanageHappines(1);
            }
            else
            {
                bool HaveFood = true;
                foreach(GameObject item in Inventory.itemsInInventory)
                {
                    if(item.CompareTag("Food") && Inventory.GetValueOfItemInInventory(item) > 0)
                    {
                        FoodController.EatTheFood(AmountOfEat, item);
                        HaveFood = true;
                        break;
                    }
                    else
                    {
                        HaveFood = false;
                    }
                }
                if (HaveFood)
                {
                    ChanageHappines(-1);
                }
                else
                {
                    ChanageHappines(-2);
                }
            }
        }
    }
    private void Update()
    {
        if (FavoriteFood == null)
        {
            FavoriteFood = Inventory.GetRandomItemFood();
        }
        timer += Time.deltaTime;
        if(timer >= EatTimer)
        {
            EatFavoriteFood();
            timer = 0;
        }
        if (HardWorking)
        {
            AmountOfEat = 2;
        }
        else
        {
            AmountOfEat = 1;
        }
        // jes³i jest wkurzony g³ód spada szybciej i porusza siê szybiej 
        if (Mad)
        {
            movment.moveSpeed = 8f;
            movment.maxWaitTime = 1f;
            if (HardWorking)
            {
                EatTimer = 3f;
            }
            else
            {
                EatTimer = 7f;
            }
        }
        else
        {
            movment.moveSpeed = 4.5f;
            movment.maxWaitTime = 3f;
            if (HardWorking)
            {
                EatTimer = 5f;
            }
            else
            {
                EatTimer = 10f;
            }
        }

    }
    void ChanageHappines(float amount)
    {
        if (HardWorking && (amount < 0) || Mad)
        {
            Happines += (amount * 2);
        }
        else
        {
            if (Mad)
            {
                Happines += amount/2;
            }
            else
            {
                Happines += amount;
            }        
        }
        HappinesEffects();
    }
    void HappinesEffects()
    {
        // jeœli Happines jest mniejsze od 40 to jest wkurzony
        if (Happines > 40)
        {
            if (Happines > 100)
            {
                Happines = 100;
            }

            Mad = false;
            
        }
        else
        {
            Mad = true;

            if (Happines < 0)
            {
                Happines = 0;

            }
        }
    }
}

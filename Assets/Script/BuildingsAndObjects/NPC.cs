using UnityEngine;

public class NPC : MonoBehaviour
{
    private MovmentNPC movment;
    public GameObject FavoriteFood;
    private InventoryManager Inventory;
    private FoodController FoodController;
    [Header("Praca")]
    public GameObject Building;
    public bool HardWorking = false;
    public float speed = 1f;
    [Header("Samopoczucie")]
    public float Happines = 100f;
    public bool Mad = false;
    public bool Happy = false;
    [Header("G³ód")]
    float timer = 0f;
    float WorkinTimer = 0f;
    public float AmountOfHunger = 1.25f;
    public float TimerToEat = 10f;
    
    private void Start()
    {
        AmountOfHunger = Random.Range(1f, 3f);
        TimerToEat = ChangeEatTimer(1);
        Inventory = InventoryManager.Instance;
        movment = GetComponent<MovmentNPC>();
        FoodController = FoodController.Instance;
    }
    void EatFavoriteFood()
    {
        if(FavoriteFood != null)
        {
            //Jeœli ma swoje ulubione jedzenie w inventoy to je zjada jeœli niema go w inventory to zjda pierwsze z brzegu i jest smutniejszy ;-;
            if(FoodController.GetCurrentKgOfCurrentFood(FavoriteFood) >= AmountOfHunger)
            {
                FoodController.EatTheFood(AmountOfHunger, FavoriteFood);
                ChanageHappines(1);
                TimerToEat = ChangeEatTimer(1);
            }
            else
            {
                SearchForFood();
            }
        }
        else
        {
            SearchForFood();
        }
    }
    //Wyszukaj jedzenie z inevenmtory jakie posaida jesli niema ¿adnego umiera
    void SearchForFood()
    {
        bool HaveFood = false;
        foreach (GameObject item in Inventory.itemsInInventory)
        {
            if (item.CompareTag("Food"))
            {
                if(FoodController.GetCurrentKgOfCurrentFood(item) >= AmountOfHunger)
                {
                    FoodController.EatTheFood(AmountOfHunger, item);
                    HaveFood = true;
                    Debug.Log("Wystarczaj¹co jedzenia");
                    break;

                }
            }
        }

        if (!HaveFood)
        {
            bool HaveAnyFood = false;
            foreach (GameObject item in Inventory.itemsInInventory)
            {
                if (item.CompareTag("Food"))
                {
                    if (Inventory.GetValueOfItemInInventory(item) > 0)
                    {
                        Inventory.ChangeValueOfItemInInventoryTo(item, 0);
                        
                        HaveAnyFood = true;
                        break;
                    }
                }
            }
            if (HaveAnyFood)
            {
                Debug.Log("nie Wystarczaj¹co jedzenia");
                TimerToEat = ChangeEatTimer(2);
                ChanageHappines(-2);
            }
            else
            {
                Die();
            }

        }
        else
        {
            ChanageHappines(-1);
        }
    }
    void Die()
    {
        Destroy(this.gameObject);
    }
    private void Update()
    {
        
        if (FavoriteFood == null)
        {
            FavoriteFood = Inventory.GetRandomItemFood();
        }

        timer += Time.deltaTime;
        if(timer >= TimerToEat)
        {
            FavoriteFood = Inventory.GetRandomItemFood();
            EatFavoriteFood();
            timer = 0;
        }

        // jes³i jest wkurzony g³ód spada szybciej i porusza siê szybiej 
        if (Mad)
        {
            movment.moveSpeed = 8f;
            movment.maxWaitTime = 1f;
        }
        else
        {
            movment.moveSpeed = 4.5f;
            movment.maxWaitTime = 3f;
        }
        if (Building != null) 
        {
            this.gameObject.GetComponent<MovmentNPC>().enabled = false;
            transform.position = Building.transform.position + new Vector3(0, -0.6f);
            HardWorking = true;
            WorkinTimer += Time.deltaTime * speed;
            if(WorkinTimer > 2f)
            {
                if (Building.gameObject.GetComponent<Bulding>().isProdusingBuilding)
                    Building.GetComponent<Bulding>().ProdusingItem(MouseController.Instance.ClickBonus);
                WorkinTimer = 0f;
            }
        }
        else
        {
            HardWorking = false;    
        }
    }
    float ChangeEatTimer(float Cut)
    {
        float time = 0f;
        float speed = 3f;
        if (HardWorking)
        {
            speed = 1.5f;
        }
        if (Mad)
        {
            speed -= 0.5f;
        }
        if (Cut <= 1)
        {
            time = (AmountOfHunger * 1.5f) * speed;
        }
        else
        {
            time = (TimerToEat * 1.5f) / Cut;
        }
        return time;
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
            if(Happines >= 80)
            {
                speed = 2;
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

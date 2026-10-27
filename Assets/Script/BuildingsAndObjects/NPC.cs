using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private MovmentNPC movment; // Referencja do skryptu poruszania siê
    public GameObject FavoriteFood; // Ulubiony rodzaj jedzenia tego konkretnego NPC
    private InventoryManager Inventory;
    private FoodController FoodController;
    public SpriteRenderer spriteRenderer;

    // Lista mo¿liwych wygl¹dów (podmieniana losowo przy narodzinach)
    public List<Sprite> sprites = new List<Sprite>();

    [Header("Praca")]
    public GameObject Building; // Budynek, do którego NPC jest aktualnie przypisany
    public bool HardWorking = false; // Czy aktualnie ciê¿ko pracuje?
    public float speed = 1f; // Mno¿nik szybkoœci pracy (zale¿ny od szczêœcia)

    [Header("Samopoczucie")]
    public float Happines = 100f; // Poziom szczêœcia (0-100)
    public bool Mad = false; // Czy NPC jest wœciek³y (szczêœcie < 40)
    public bool Happy = false; // Zmienna przygotowana, ale obecnie nieu¿ywana w kodzie

    [Header("G³ód")]
    float timer = 0f; // Odmierza czas do nastêpnego posi³ku
    float WorkinTimer = 0f; // Odmierza czas do wyprodukowania surowca w pracy
    public float AmountOfHunger = 1.25f; // Ile kilogramów jedzenia NPC musi zjeœæ na raz
    public float TimerToEat = 10f; // Co ile sekund NPC robi siê g³odny

    private void Start()
    {
        // Losowanie apetytu (ka¿dy pracownik je trochê inaczej)
        AmountOfHunger = Random.Range(1f, 3f);

        // Inicjalizacja referencji do g³ównych systemów gry
        Inventory = InventoryManager.Instance;
        movment = GetComponent<MovmentNPC>();
        FoodController = FoodController.Instance;

        // Losowanie pocz¹tkowego czasu do zjedzenia
        TimerToEat = ChangeEatTimer(1);

        // Losowanie i przypisanie wygl¹du postaci
        int i = Random.Range(0, sprites.Count);
        spriteRenderer.sprite = sprites[i];
    }

    // Próba zjedzenia ulubionego posi³ku
    void EatFavoriteFood()
    {
        if (FavoriteFood != null)
        {
            // Sprawdza, czy w globalnej puli (lub ekwipunku) jest wystarczaj¹co du¿o ulubionego jedzenia
            if (FoodController.GetCurrentKgOfCurrentFood(FavoriteFood) >= AmountOfHunger)
            {
                // Zjada ulubione jedzenie, zyskuje +1 do szczêœcia i resetuje timer g³odu
                FoodController.EatTheFood(AmountOfHunger, FavoriteFood);
                ChanageHappines(1);
                TimerToEat = ChangeEatTimer(1);
            }
            else
            {
                // Jeœli nie ma ulubionego, szuka jakiegokolwiek innego
                SearchForFood();
            }
        }
        else
        {
            SearchForFood();
        }
    }

    // Wyszukiwanie jedzenia zastêpczego (gdy brak ulubionego)
    void SearchForFood()
    {
        bool HaveFood = false;

        // Faza 1: Szuka jakiegokolwiek jedzenia, którego jest wystarczaj¹co du¿o, by zaspokoiæ g³ód w pe³ni
        foreach (GameObject item in Inventory.itemsInInventory)
        {
            if (item.CompareTag("Food"))
            {
                if (FoodController.GetCurrentKgOfCurrentFood(item) >= AmountOfHunger)
                {
                    FoodController.EatTheFood(AmountOfHunger, item);
                    HaveFood = true;
                    Debug.Log("Wystarczaj¹co jedzenia");
                    break;
                }
            }
        }

        // Faza 2: Jeœli nie znalaz³ pe³nej porcji jedzenia
        if (!HaveFood)
        {
            bool HaveAnyFood = false;
            // Szuka DOS£OWNIE CZEGOKOLWIEK do zjedzenia (wyjada resztki)
            foreach (GameObject item in Inventory.itemsInInventory)
            {
                if (item.CompareTag("Food"))
                {
                    if (Inventory.GetValueOfItemInInventory(item) > 0)
                    {
                        // Wyzerowanie zapasów tego konkretnego jedzenia (zjada to co zosta³o)
                        Inventory.ChangeValueOfItemInInventoryTo(item, 0);
                        HaveAnyFood = true;
                        break;
                    }
                }
            }

            if (HaveAnyFood)
            {
                Debug.Log("nie Wystarczaj¹co jedzenia");
                // Zjad³ resztki, ale siê nie najad³. Zg³odnieje 2x szybciej i traci 2 punkty szczêœcia.
                TimerToEat = ChangeEatTimer(2);
                ChanageHappines(-2);
            }
            else
            {
                // Brak jakiegokolwiek jedzenia - œmieræ g³odowa pracownika
                Die();
            }
        }
        else
        {
            // Zjad³ inne jedzenie ni¿ ulubione, traci 1 punkt szczêœcia
            ChanageHappines(-1);
        }
    }

    void Die()
    {
        Destroy(this.gameObject); // Usuwa NPC z gry
    }

    private void Update()
    {
        // UWAGA: Jeœli Inventory.GetRandomItemFood() zwraca null (bo nie ma jedzenia), 
        // to wywo³a siê to 60 razy na sekundê. Warto to zoptymalizowaæ!
        if (FavoriteFood == null)
        {
            FavoriteFood = Inventory.GetRandomItemFood();
        }

        // Odmierzanie czasu do posi³ku
        timer += Time.deltaTime;
        if (timer >= TimerToEat)
        {
            // Losuje nowe ulubione jedzenie po ka¿dym posi³ku
            FavoriteFood = Inventory.GetRandomItemFood();
            EatFavoriteFood();
            timer = 0;
        }

        // Modyfikatory zachowania wêdrowania zale¿ne od emocji
        if (Mad)
        {
            movment.moveSpeed = 8f; // Bêd¹c wœciek³ym, porusza siê bardzo szybko
            movment.maxWaitTime = 1f; // Prawie nie odpoczywa
        }
        else
        {
            movment.moveSpeed = 4.5f; // Standardowa prêdkoœæ
            movment.maxWaitTime = 3f; // Standardowe przerwy
        }

        // SYSTEM PRACY
        if (Building != null)
        {
            // Jeœli przypisany do budynku, wy³¹cz swobodne poruszanie siê
            this.gameObject.GetComponent<MovmentNPC>().enabled = false;

            // "Przyklej" NPC do pozycji budynku (z lekkim przesuniêciem w dó³)
            transform.position = Building.transform.position + new Vector3(0, -0.6f);
            HardWorking = true;

            // Odliczanie czasu do wyprodukowania przedmiotu przez budynek
            WorkinTimer += Time.deltaTime * speed;
            if (WorkinTimer > 2f)
            {
                // Jeœli budynek jest aktywny, wyprodukuj zasób z uwzglêdnieniem bonusu od gracza
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

    // Skomplikowany system obliczania czasu do zjedzenia
    float ChangeEatTimer(float Cut)
    {
        float time = 0f;
        float speed = 3f; // Bazowy "mno¿nik przyspieszenia g³odu" (im mniejszy, tym g³odnieje szybciej)

        if (HardWorking)
        {
            speed = 1.5f; // Podczas pracy g³ód roœnie szybciej
        }
        if (Mad)
        {
            speed -= 0.5f; // Z³oœæ dodatkowo przyspiesza g³ód
        }

        if (Cut <= 1)
        {
            time = (AmountOfHunger * 1.5f) * speed;
        }
        else
        {
            time = (TimerToEat * 1.5f) / Cut;
        }
        return time; // Zwraca nowy limit czasu na timerze
    }

    // Modyfikowanie zadowolenia NPC z uwzglêdnieniem mno¿ników stanu (Praca / Z³oœæ)
    void ChanageHappines(float amount)
    {
        // Jeœli pracuje i traci szczêœcie LUB jest wœciek³y, to kary do zadowolenia s¹ PODWAJANE
        if (HardWorking && (amount < 0) || Mad)
        {
            Happines += (amount * 2);
        }
        else
        {
            if (Mad)
            {
                // Jeœli jest z³y, premie do zadowolenia (np. za dobre jedzenie) dzia³aj¹ o PO£OWÊ s³abiej
                Happines += amount / 2;
            }
            else
            {
                Happines += amount; // Standardowe dodawanie
            }
        }
        HappinesEffects(); // Aplikowanie efektów po zmianie
    }

    // Aplikowanie buffów i debuffów zwi¹zanych ze statystyk¹ zadowolenia
    void HappinesEffects()
    {
        if (Happines > 40)
        {
            if (Happines > 100) Happines = 100; // Limit maksimum

            // Szczêœliwy kot pracuje/dzia³a dwa razy szybciej (speed = 2)
            if (Happines >= 80)
            {
                speed = 2;
            }
            Mad = false;
        }
        else
        {
            // Poni¿ej 40 punktów odpala siê tryb Mad (Wœciek³y)
            Mad = true;

            if (Happines < 0) Happines = 0; // Limit minimum
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
public class MainBuilding : MonoBehaviour
{

    [Header("What item needed to upgrade")]
    public List<GameObject> ItemNeededRightNow = new List<GameObject>();
    public List<int> AmountOfItemNeeded = new List<int> ();
    public List<GameObject> ItemNeededLvL2 = new List<GameObject>();
    public List<GameObject> ItemNeededLvL3 = new List<GameObject>();
    public int AmountItemsNeeded;
    [Header("UI")]
    public GameObject Canvas;
    public Slider slider;
    public GameObject Image;
    public List<GameObject> Images = new List<GameObject>();
    public GameObject Contenrer;
    [Header("Sprites")]
    public SpriteRenderer MainSprite;
    public Sprite[] Sprites;

    [Header("Settings")]
    public int LevelOfBuilding = 1;
    public int FoodAmount = 300;
    public int People = 2;
    private void Start()
    {
        AditionalChange();
        LevelOfBuilding = 1;
    }
    private void Update()
    {
        ChangeValueSlider();
        SliderChangeColllor();
    }
    public void AditionalChange()
    {
        if (LevelOfBuilding <= 3)
        {
            FoodAmount = gameObject.GetComponent<StorageBuilding>().AditionalFoodStorage * LevelOfBuilding;
            People = gameObject.GetComponent<StorageBuilding>().MaxPeopleStorage * LevelOfBuilding;
            MainSprite.sprite = Sprites[LevelOfBuilding - 1];
            FoodController.Instance.ChangeMaxFoodAmount(FoodAmount);
            NPCController.Instance.ChangePeopleStorage(People);
            
            gameObject.GetComponent<TypeOfBuilding>().BuildingRadius = 15f * LevelOfBuilding;
            ChangeNeededItems();
        }
        else if(LevelOfBuilding == 3) 
        {
            Canvas.SetActive(false);
        }
    }
    void ChangeNeededItems()
    {
        ItemNeededRightNow.Clear();
        AmountOfItemNeeded.Clear();
        foreach(GameObject img in Images)
        {
            Destroy(img);
        }
        Images.Clear();
        if (LevelOfBuilding == 1)
        {
            foreach (GameObject item in ItemNeededLvL2)
            {
                ItemNeededRightNow.Add(item);
            }
        }
        else if(LevelOfBuilding == 2)
        {
            foreach (GameObject item in ItemNeededLvL3)
            {
                ItemNeededRightNow.Add(item);
            }
        }
        foreach (GameObject item in ItemNeededRightNow)
        {
            AmountOfItemNeeded.Add(AmountItemsNeeded * LevelOfBuilding * 2);
            GameObject img = Instantiate(Image, Contenrer.transform);
            Images.Add(img);
            img.GetComponent<Image>().sprite = item.GetComponent<SpriteRenderer>().sprite;
            img.GetComponentInChildren<TextMeshProUGUI>().text = AmountOfItemNeeded[ItemNeededRightNow.IndexOf(item)].ToString();
        }
        ChangeMaxValueSlider();
    }

    void ChangeMaxValueSlider()
    {
        float amount = 0f;
        foreach (GameObject item in ItemNeededRightNow)
        {
            amount += AmountOfItemNeeded[ItemNeededRightNow.IndexOf(item)];
        }
        slider.maxValue = amount;
    }
    void ChangeValueSlider()
    {
        float amount = 0;
        foreach (GameObject item in ItemNeededRightNow)
        {
            if(InventoryManager.Instance.GetValueOfItemInInventory(item) <= AmountOfItemNeeded[ItemNeededRightNow.IndexOf(item)])
            {
                amount += InventoryManager.Instance.GetValueOfItemInInventory(item);
            }
            else
            {
                amount += AmountOfItemNeeded[ItemNeededRightNow.IndexOf(item)];
            }

        }
        slider.value = amount;
        
    }
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
        else
        {
            slider.fillRect.GetComponent<Image>().color = Color.green;
        }
    }
    public void CheckItems()
    {
        bool CanUpgrade = true;
        foreach(GameObject item in ItemNeededRightNow)
        {
            if (AmountOfItemNeeded[ItemNeededRightNow.IndexOf(item)] > InventoryManager.Instance.GetValueOfItemInInventory(item)) 
            {
                CanUpgrade = false;
                break;
            }
        }
        if (CanUpgrade)
        {
            foreach (GameObject item in ItemNeededRightNow)
            {
                InventoryManager.Instance.ChangeValueOfItemInInventory(item, -AmountOfItemNeeded[ItemNeededRightNow.IndexOf(item)]);
            }
            LevelOfBuilding++;
            AditionalChange();
            UpgradeController.Instance.TokensForRebirth++;
        }
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UICursorController : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private Canvas canvas;
    private InventoryManager inventoryManager;
    private TypesOfBuildingMenager typesOfBuildingMenager;
    [Header("Dzwiêki")]
    public AudioClip ClickTheWoodSound;
    public AudioClip ClickTheStoneSound;
    public AudioClip ClickTheBerriesSound;
    [Header("UI Item Slots")]
    public GameObject ItemSlotContainer;
    public TextMeshProUGUI ItemQuantity;
    public TextMeshProUGUI ItemProducing;
    public Image ItemImage;
    [Header("UI Building Buttons")]
    public GameObject BuildingButtonContainer;
    public List<GameObject> RecourcesNeeded = new List<GameObject>();
    public List<TextMeshProUGUI> RecourcesNeededQuantity = new List<TextMeshProUGUI>();
    public TextMeshProUGUI DescriptionOfBuilding;
    public MouseController mouseController;
    [Header("Collision")]
    private Collider2D collisionButton;
    public float timer;
    public int ClickBonus = 0;
    float Speed = 1f;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        // Jeœli nie przypiszesz Canvasu w inspektorze, skrypt spróbuje go znaleŸæ
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
        inventoryManager = InventoryManager.Instance;
        typesOfBuildingMenager = TypesOfBuildingMenager.Instance;
    }

    void Update()
    {
        Speed = mouseController.Speed;
        ClickBonus = mouseController.ClickBonus;
        Vector2 localPoint;

        // Konwersja pozycji myszy na pozycjê relatywn¹ dla Canvasu
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out localPoint);

        rectTransform.localPosition = localPoint + new Vector2(0 , 10);

        timer += Time.deltaTime * Speed;
        if (Input.GetMouseButton(0))
        {
            if(collisionButton != null && collisionButton.gameObject.CompareTag("UiComponentButton"))
            {
                if (timer >= 1f)
                {
                    collisionButton.GetComponent<Bulding>().ProdusingItem(ClickBonus);
                    collisionButton.GetComponent<KlikaczSurowca>().KliknietoSurowiec();
                    timer = 0f;
                }
            }
        }
    }
    // Funkcja do pokazywania UI z informacjami o przedmiocie lub budynku, w zale¿noœci od tego, z czym koliduje kursor, i ukrywanie tego UI, gdy kursor przestaje kolidowaæ z danym obiektem
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("UIComponent"))
        {
            if (collision.GetComponent<Image>().enabled == true)
            {
                // Pokazywanie UI z informacjami o przedmiocie, w tym iloœci¹ tego przedmiotu w ekwipunku i obrazkiem tego przedmiotu
                ItemSlotContainer.SetActive(true);
                ItemQuantity.text = inventoryManager.GetValueOfItemInInventory(collision.gameObject).ToString();
                ItemImage.sprite = collision.GetComponentInChildren<Image>().sprite;
            }
        }

        // Szukanie budynku, z którym koliduje kursor, w menad¿erze typów budynków i pokazywanie UI z informacjami o tym budynku
        if (collision.gameObject.CompareTag("UIComponentBulding"))
        {
            GameObject Bulding = typesOfBuildingMenager.SearchForBuilding(collision.gameObject.name);
            if (Bulding != null)
            {
                // Pokazywanie UI z informacjami o budynku, w tym opisem i wymaganymi zasobami do jego budowy
                BuildingButtonContainer.SetActive(true);
                DescriptionOfBuilding.text = Bulding.GetComponent<TypeOfBuilding>().Description;
                foreach (GameObject resource in Bulding.GetComponent<TypeOfBuilding>().WhatResourcesNeedToBuild)
                {
                    RecourcesNeeded[Bulding.GetComponent<TypeOfBuilding>().WhatResourcesNeedToBuild.IndexOf(resource)].SetActive(true);
                    RecourcesNeeded[Bulding.GetComponent<TypeOfBuilding>().WhatResourcesNeedToBuild.IndexOf(resource)].GetComponent<Image>().sprite = resource.GetComponent<SpriteRenderer>().sprite;
                    RecourcesNeededQuantity[Bulding.GetComponent<TypeOfBuilding>().WhatResourcesNeedToBuild.IndexOf(resource)].text = Bulding.GetComponent<TypeOfBuilding>().CostsOfResources[Bulding.GetComponent<TypeOfBuilding>().WhatResourcesNeedToBuild.IndexOf(resource)].ToString();
                }
            }
        }
        
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("UiComponentButton"))
        {
            collisionButton = collision;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("UIComponent") || collision.gameObject.CompareTag("UIComponentBulding") || collision.gameObject.CompareTag("UiComponentButton"))
        {
            ItemSlotContainer.SetActive(false);
            BuildingButtonContainer.SetActive(false);
            foreach (GameObject resource in RecourcesNeeded)
            {
                resource.SetActive(false);
            }
            collisionButton = null;
        }
    }
}

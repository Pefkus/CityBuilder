using UnityEngine;

public class MainBuilding : MonoBehaviour
{
    public SpriteRenderer MainSprite;
    public Sprite[] Sprites;
    public int LevelOfBuilding = 1;
    public int FoodAmount = 300;
    public int People = 2;
    private void Start()
    {
        LevelOfBuilding = 1;
    }
    private void Update()
    {
        
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
        }
    }
}

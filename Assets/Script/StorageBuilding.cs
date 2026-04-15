using UnityEngine;

public class StorageBuilding : MonoBehaviour
{
    public int MaxPeopleStorage;
    public int AditionalFoodStorage;

    void Start()
    {
        FoodController.Instance.ChangeMaxFoodAmount(AditionalFoodStorage);
        FoodController.Instance.ChangePeopleStorage(MaxPeopleStorage);
    }
     void Update()
    {

    }
}

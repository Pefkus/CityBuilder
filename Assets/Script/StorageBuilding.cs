using UnityEngine;

public class StorageBuilding : MonoBehaviour
{
    public int MaxPeopleStorage;
    public int AditionalFoodStorage;

    private void Start()
    {
        if(GetComponent<TypeOfBuilding>() != null && !GetComponent<TypeOfBuilding>().MainBuilding)
        {
            FoodController.Instance.ChangeMaxFoodAmount(AditionalFoodStorage);
            NPCController.Instance.MaxPeopleStorage += MaxPeopleStorage;
        }
    }

}

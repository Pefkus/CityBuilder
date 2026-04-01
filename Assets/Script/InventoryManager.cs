using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public int wood = 0;
    public int stone = 0;
    public int berries = 0;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void ChangeValueOfItemInInventory(string name, int amount)
    {
        name = name.ToLower();
        switch (name)
        {
            case "wood":
                wood += amount;
                break;
            case "stone":
                stone += amount;
                break;
            case "berries":
                berries += amount;
                break;
            default:
                Debug.LogWarning("Nieznany przedmiot: " + name);
                break;
        }
    }
}

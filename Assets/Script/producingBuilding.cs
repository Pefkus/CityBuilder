using UnityEngine;

public class Bulding : MonoBehaviour
{
    public GameObject ProdusingItemName;
    public GameObject ItemNeeded;
    public int Cost;
    public int amoutOfItemProdusing;
    public void ProdusingItem(int boost)
    {
        if (ItemNeeded.name != "")
        {
            if (InventoryManager.Instance.GetValueOfItemInInventory(ItemNeeded) >= Cost)
            {
                InventoryManager.Instance.ChangeValueOfItemInInventory(ItemNeeded, -Cost);
                InventoryManager.Instance.ChangeValueOfItemInInventory(ProdusingItemName, amoutOfItemProdusing + boost);
            }
            else
            {
                Debug.Log("Nie masz wystarczaj¹co " + ItemNeeded);
            }
        }
        else
            InventoryManager.Instance.ChangeValueOfItemInInventory(ProdusingItemName, amoutOfItemProdusing + boost);
    }
}

using UnityEngine;

public class Bulding : MonoBehaviour
{
    public int AdaptiveBoost = 0;
    public bool isProdusingBuilding;
    public GameObject ProdusingItemName;
    public GameObject ItemNeeded;
    public int Cost;
    public int amoutOfItemProdusing;
    public GameObject PopUp;
    public void ProdusingItem(int boost)
    {
        if(isProdusingBuilding)
        {
            if (ItemNeeded != null)
            {
                if (InventoryManager.Instance.GetValueOfItemInInventory(ItemNeeded) >= Cost + boost + AdaptiveBoost)
                {
                    InventoryManager.Instance.ChangeValueOfItemInInventory(ItemNeeded, -Cost - boost - AdaptiveBoost);
                    InventoryManager.Instance.ChangeValueOfItemInInventory(ProdusingItemName, amoutOfItemProdusing + boost + AdaptiveBoost);
                    CreatePopItem();
                }
            }
            else
            {
                InventoryManager.Instance.ChangeValueOfItemInInventory(ProdusingItemName, amoutOfItemProdusing + boost + AdaptiveBoost);
                CreatePopItem();
            }
        }
    }
    void CreatePopItem()
    {
        if (PopUp != null)
        {
            PopUp.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, ProdusingItemName.GetComponent<SpriteRenderer>().sprite);
            Instantiate(PopUp, transform.position, Quaternion.identity, this.transform);
        }
    }
}

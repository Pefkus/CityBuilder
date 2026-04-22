using TMPro;
using UnityEngine;

public class Upgrader : MonoBehaviour
{
    public KociSpawner kociSpawner;
    public TextMeshProUGUI text;
    public int Cost = 100;
    public GameObject WhatItemNeed;
    InventoryManager inventoryManager;
    UpgradeController upgradeController;
    void Start()
    {
        
        inventoryManager = InventoryManager.Instance;
        upgradeController = UpgradeController.Instance;

        Cost += upgradeController.MoreCost;
        ChangeText(Cost);
    }
    bool CheckResources()
    {
        if (inventoryManager.GetValueOfItemInInventory(WhatItemNeed) >= Cost)
        {
            inventoryManager.ChangeValueOfItemInInventory(WhatItemNeed, -Cost);
            Cost *= 2;
            ChangeText(Cost);
            return true;
        }
        else
        {
            return false;
        }
    }
    public void CreateNewNPC()
    {
        if (NPCController.Instance.CanCreateNewNPC())
        {
            if (CheckResources())
            {
                NPCController.Instance.CreateNewNpc();
            }
        }
    }
    void ChangeText(int cost)
    {
        text.text = cost.ToString() + " " + WhatItemNeed.name;
        text.overflowMode = TextOverflowModes.Ellipsis;
    }

    public void MoreClickForPanel(GameObject panel)
    {
        if (CheckResources())
        {
            panel.GetComponent<Bulding>().AdaptiveBoost += 1;
        }
        
    }
    public void MoreClickForMouse()
    {
        if (CheckResources())
        {
            UpgradeController.Instance.MouseController.ClickBonus += 1;
        }
        
    }
    public void MouseSpeedBonus()
    {
        if (CheckResources())
        {
            UpgradeController.Instance.MouseController.Speed += 0.5f;
        }
       
    }
    public void CatSpeedBonus(int what)
    {
        
        if (CheckResources())
        {
            switch (what)
            {
                case 0:
                    UpgradeController.Instance.WoodSpeed += 0.5f;
                    break;
                case 1:
                    UpgradeController.Instance.StoneSpeed += 0.5f;
                    break;
                case 2:
                    UpgradeController.Instance.BerriesSpeed += 0.5f;
                    break;
            }
        }
       
        
    }
    public void MoreCatBonusForPanel(int what)
    {
        if (CheckResources())
        {
            switch (what)
            {
                case 0:
                    UpgradeController.Instance.KotDrwal += 1;
                    break;
                case 1:
                    UpgradeController.Instance.KotGórnik += 1;
                    break;
                case 2:
                    UpgradeController.Instance.KotZbieracz += 1;
                    break;
            }
            if(kociSpawner != null)
            {
                kociSpawner.DodajNowegoKota();
            }
           
        }
        
    }
}

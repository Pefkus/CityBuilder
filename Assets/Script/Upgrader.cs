using UnityEngine;

public class Upgrader : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MoreClickForPanel(GameObject panel)
    {
        panel.GetComponent<Bulding>().AdaptiveBoost += 1;
    }
    public void MoreClickForMouse()
    {
        UpgradeController.Instance.MouseController.ClickBonus += 1;
    }
    public void MouseSpeedBonus()
    {
        UpgradeController.Instance.MouseController.Speed += 0.5f;
    }
    public void CatSpeedBonus(int what)
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
    public void MoreCatBonusForPanel(int what)
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
    }
}

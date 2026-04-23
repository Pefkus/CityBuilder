using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    public static UpgradeController Instance { get; private set; }
    public MouseController MouseController;
    public UICursorController UICursorController;
    public int TokensForRebirth = 0;
    public int KotDrwal = 0;
    public int KotGórnik = 0;
    public int KotZbieracz = 0;
    public GameObject[] panels;
    float timerForStone;
    public float StoneSpeed;
    float timerForWood;
    public float WoodSpeed;

    float timerForBerries;
    public float BerriesSpeed;
    public int MoreCost = 0;
    private void Start()
    {
        Instance = this;
    }
    private void Update()
    {
        timerForStone += Time.deltaTime * StoneSpeed;
        timerForWood += Time.deltaTime * WoodSpeed;
        timerForBerries += Time.deltaTime * BerriesSpeed;

        if (timerForWood >= 1f)
        {
            if (KotDrwal > 0)
            {
                panels[0].GetComponent<Bulding>().ProdusingItem(KotDrwal);
            }
            timerForWood = 0f;
        }

        if (timerForStone >= 1f)
        {
            if (KotGórnik > 0)
            {
                panels[1].GetComponent<Bulding>().ProdusingItem(KotGórnik);
            }
            timerForStone = 0f;
        }

        if (timerForBerries >= 1f)
        {
            if (KotZbieracz > 0)
            {
                panels[2].GetComponent<Bulding>().ProdusingItem(KotZbieracz);
            }
            timerForBerries = 0f;
        }

    }
    
}

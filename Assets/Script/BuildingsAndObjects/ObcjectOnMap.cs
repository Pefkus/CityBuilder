using UnityEngine;
using UnityEngine.UI;

public class ObcjectOnMap : MonoBehaviour
{
    public int DurabilityAmount;
    int MaxAmountCanProduce;
    public GameObject ItemProdusing;
    public GameObject PopUp;
    public GameObject DestroyPS;
    public GameObject ContainerForSlider;
    public Slider slider;
    [Header("Dzwiêki")]
    public AudioClip ClickTheSound;
    void Start()
    {
        int lvl = TypesOfBuildingMenager.Instance.MainBuilding.GetComponent<MainBuilding>().LevelOfBuilding;
        DurabilityAmount = Random.Range(10 * (lvl * lvl), 25 * (lvl * lvl));
        MaxAmountCanProduce = DurabilityAmount * 2;
        slider.maxValue = DurabilityAmount;
        
    }
    private void Update()
    {
        ChangeSlider();

    }
    public void PunchingTheObject(int Clik)
    {
        ContainerForSlider.SetActive(true);
        DurabilityAmount += -Clik - 1;
        if (DurabilityAmount < 0)
        {
            GiveTheItem(Clik);
        }
    }
    void GiveTheItem(int Boost)
    {
        MapGeneretor.Instance.Limit--;
        InventoryManager.Instance.ChangeValueOfItemInInventory(ItemProdusing, MaxAmountCanProduce + Boost);
        CreatePopItem();
    }
    void CreatePopItem()
    {
        if (PopUp != null)
        {
            PopUp.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, ItemProdusing.GetComponent<SpriteRenderer>().sprite);
            Instantiate(PopUp, transform.position, Quaternion.identity);
            Instantiate(DestroyPS, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
    void ChangeSlider()
    {
        slider.value = DurabilityAmount;
        if (slider.value <= slider.maxValue * 0.25f)
        {
            slider.fillRect.GetComponent<Image>().color = Color.red;

        }
        else if (slider.value <= slider.maxValue * 0.50f)
        {
            slider.fillRect.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            slider.fillRect.GetComponent<Image>().color = Color.green;
        }
    }
    
}

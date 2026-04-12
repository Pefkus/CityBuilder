using UnityEngine;

public class Bulding : MonoBehaviour
{
    public int AdaptiveBoost = 0;
    public bool isProdusingBuilding;
    public GameObject ProdusingItemName;
    public GameObject[] ItemNeeded;
    public int Cost;
    public int amoutOfItemProdusing;
    public GameObject PopUp;
    // Funkcja do produkowania przedmiotu, która sprawdza, czy budynek jest budynkiem produkuj¹cym, czy s¹ potrzebne jakieœ przedmioty do produkcji i czy s¹ one dostêpne w ekwipunku, a nastêpnie dodaje wyprodukowany przedmiot do ekwipunku i tworzy efekt wizualny
    public void ProdusingItem(int boost)
    {
        if(isProdusingBuilding)
        {
            if (ItemNeeded != null)
            {
                bool canProdusing = true;
                // Sprawdzamy, czy mamy wystarczaj¹co przedmiotów potrzebnych do produkcji, bior¹c pod uwagê boosty
                foreach (GameObject item in ItemNeeded)
                {
                    if (InventoryManager.Instance.GetValueOfItemInInventory(item) < Cost + boost + AdaptiveBoost)
                    {
                        canProdusing = false;
                        break;
                    }
                }
                // Jeœli mamy, to odejmujemy potrzebne przedmioty z ekwipunku i dodajemy wyprodukowany przedmiot, bior¹c pod uwagê boosty
                if (canProdusing)
                {
                    foreach (GameObject item in ItemNeeded)
                    {
                        InventoryManager.Instance.ChangeValueOfItemInInventory(item, -Cost - boost - AdaptiveBoost);
                    }
                    InventoryManager.Instance.ChangeValueOfItemInInventory(ProdusingItemName, amoutOfItemProdusing + boost + AdaptiveBoost);
                    CreatePopItem();
                }
            }
            else
            {
                // Jeœli nie potrzebujemy ¿adnych przedmiotów do produkcji, to po prostu dodajemy wyprodukowany przedmiot do ekwipunku, bior¹c pod uwagê boosty
                InventoryManager.Instance.ChangeValueOfItemInInventory(ProdusingItemName, amoutOfItemProdusing + boost + AdaptiveBoost);
                CreatePopItem();
            }
        }
    }
    // Funkcja do tworzenia efektu wizualnego produkcji przedmiotu, która ustawia sprite efektu na sprite wyprodukowanego przedmiotu i tworzy go na pozycji budynku
    void CreatePopItem()
    {
        if (PopUp != null)
        {
            PopUp.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, ProdusingItemName.GetComponent<SpriteRenderer>().sprite);
            Instantiate(PopUp, transform.position, Quaternion.identity, this.transform);
        }
    }
}

using UnityEngine;

public class BoostingBuilding : MonoBehaviour
{
   void OnTriggerEnter2D(Collider2D collision)
   {
       if (collision.gameObject.CompareTag("Bulding"))
       {
           collision.gameObject.GetComponent<Bulding>().AdaptiveBoost += 1;
       }
   }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bulding"))
        {
            collision.gameObject.GetComponent<Bulding>().AdaptiveBoost -= 1;
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class Click : MonoBehaviour
{
    GameObject Collision;
    // naciœniêcie na budynek, który jest w triggerze, spowoduje wywo³anie funkcji produsing item z boostem 0 (czyli bez boosta)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bulding"))
        {
            Collision = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bulding"))
        {
            Collision = null;
        }
    }
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && Collision != null)
        {
            if(Collision.gameObject.GetComponent<Bulding>().isProdusingBuilding)
                Collision.gameObject.GetComponent<Bulding>().ProdusingItem(0);
           
            Collision.gameObject.GetComponentInChildren<Animator>().SetTrigger("Click");
        }
    }
}

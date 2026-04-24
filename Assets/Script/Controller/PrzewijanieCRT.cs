using UnityEngine;
using UnityEngine.UI;

public class PrzewijanieCRT : MonoBehaviour
{
    [Header("Prędkość zjeżdżania pasków")]
    public float szybkoscY = 0.5f;

    private RawImage obraz;
    private Rect uvRect;

    void Start()
    {
        obraz = GetComponent<RawImage>();
        uvRect = obraz.uvRect;
    }

    void Update()
    {
        uvRect.y += szybkoscY * Time.deltaTime;
        
        obraz.uvRect = uvRect;
    }
}
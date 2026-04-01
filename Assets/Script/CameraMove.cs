using UnityEngine;

public class CameraMove : MonoBehaviour
{

    [Header("Ustawienia Przesuwania")]
    public float panSpeed = 20f;

    [Header("Ustawienia Przybli¿ania (Zoom)")]
    public float zoomSpeed = 5f;
    public float minZoom = 10f;  
    public float maxZoom = 30f; 

    private Camera cam;
    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        HandlePan();
        HandleZoom();
    }

    private void HandlePan()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Vector3 move = new Vector3(-mouseX, -mouseY, 0) * panSpeed * Time.deltaTime;

            transform.Translate(move, Space.World);
        }
    }

    private void HandleZoom()
    {
        float scrollData = Input.GetAxis("Mouse ScrollWheel");

        if (scrollData != 0f)
        {
            cam.orthographicSize -= scrollData * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
     }
    }

using UnityEngine;
using System.Collections;
public class MovmentNPC : MonoBehaviour
{

    [Header("Ustawienia ruchu")]
    public float moveSpeed = 3f;
    public float MaxGridSize = 1f;

    [Header("Warstwy (Layers)")]
    public LayerMask walkableLayers;
    public LayerMask obstacleLayers;

    [Header("Czas wêdrowania")]
    public float minWaitTime = 1f;
    public float maxWaitTime = 4f;

    private bool isMoving = false;

    //  kierunki ukoœne (prawo-góra, prawo-dó³, itd.)
    private Vector2[] directions = {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right,
        new Vector2(1, 1),   // Góra-Prawo
        new Vector2(1, -1),  // Dó³-Prawo
        new Vector2(-1, 1),  // Góra-Lewo
        new Vector2(-1, -1)  // Dó³-Lewo
    };

    void Start()
    {
        StartCoroutine(WanderRoutine());
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            if (!isMoving)
            {

                float waitTime = Random.Range(minWaitTime, maxWaitTime);
                yield return new WaitForSeconds(waitTime);

                TryMove();
            }
            yield return null;
        }
    }

    private void TryMove()
    {
        float rGridSize = Random.Range(1, MaxGridSize);
        Vector2 randomDir = directions[Random.Range(0, directions.Length)];
        Vector2 targetPos = (Vector2)transform.position + (randomDir * rGridSize);

        
        // Dla ruchu prosto wyniesie ona 1 * gridSize.
        // Dla ruchu na skos wyniesie ok. 1.41 * gridSize.
        float rayDistance = randomDir.magnitude * rGridSize;

        // Puszczamy Raycasty u¿ywaj¹c nowej odleg³oœci
        RaycastHit2D obstacleHit = Physics2D.Raycast(transform.position, randomDir, rayDistance, obstacleLayers);
        RaycastHit2D walkableHit = Physics2D.Raycast(transform.position, randomDir, rayDistance, walkableLayers);

        if (obstacleHit.collider == null && walkableHit.collider != null)
        {
            StartCoroutine(MoveToGridPosition(targetPos));
        }
    }

    private IEnumerator MoveToGridPosition(Vector2 target)
    {
        isMoving = true;

        while (Vector2.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        isMoving = false;
    }
}

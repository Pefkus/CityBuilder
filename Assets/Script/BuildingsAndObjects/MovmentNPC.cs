using UnityEngine;
using System.Collections;
public class MovmentNPC : MonoBehaviour
{
    [Header("Ustawienia ruchu")]
    public float moveSpeed = 3f; // Prêdkoœæ, z jak¹ NPC przesuwa siê po mapie
    public float MaxGridSize = 1f;  // Maksymalny dystans (w jednostkach Unity), jaki NPC mo¿e przejœæ w jednym ruchu

    [Header("Warstwy (Layers)")]
    public LayerMask walkableLayers; // Warstwa oznaczaj¹ca po czym NPC MO¯E chodziæ (np. pod³oga, trawa)
    public LayerMask obstacleLayers; // Warstwa oznaczaj¹ca przeszkody, w które NPC NIE MO¯E wejœæ (np. œciany, woda)

    [Header("Czas wêdrowania")]
    public float minWaitTime = 1f;  // Minimalny czas bezruchu przed kolejnym spacerem
    public float maxWaitTime = 4f;  // Maksymalny czas bezruchu
    private bool isMoving = false; // Flaga (prze³¹cznik) blokuj¹ca losowanie nowego ruchu, gdy NPC ju¿ idzie



    // Tablica 8 mo¿liwych kierunków ruchu (4 g³ówne + 4 ukoœne)

    private Vector2[] directions = {
        Vector2.up, Vector2.down, Vector2.left, Vector2.right,
        new Vector2(1, 1),   // Góra-Prawo
        new Vector2(1, -1),  // Dó³-Prawo
        new Vector2(-1, 1),  // Góra-Lewo
        new Vector2(-1, -1)  // Dó³-Lewo
    };
    void Start()
    {
        // Uruchamia asynchroniczn¹ pêtlê (Coroutine), która bêdzie dzia³aæ w tle przez ca³e ¿ycie NPC
        StartCoroutine(WanderRoutine());
    }
    // G³ówna pêtla decyzyjna NPC
    private IEnumerator WanderRoutine()
    {
        while (true) // Dzia³a w nieskoñczonoœæ (a¿ do zniszczenia obiektu)
        {
            // Jeœli NPC aktualnie stoi w miejscu...
            if (!isMoving)
            {
                // ...losuje czas, jak d³ugo ma staæ i "odpoczywaæ"
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
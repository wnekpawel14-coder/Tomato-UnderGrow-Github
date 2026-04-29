using UnityEngine;
using Mirror;

public class PlayerTasks : NetworkBehaviour
{
    // Flagi zadań
    public bool climbedLadder = false;
    public bool collectedItem = false;
    public bool lookedAround = false;

    // Funkcja wywoływana, gdy gracz wejdzie na drabinę
    public void CompleteClimbTask()
    {
        if (!climbedLadder)
        {
            climbedLadder = true;
            Debug.Log("Zadanie: Drabina zaliczona!");
        }
    }
}
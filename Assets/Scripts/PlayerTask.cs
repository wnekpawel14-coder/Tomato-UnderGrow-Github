using UnityEngine;
using Mirror;

public class PlayerTasks : NetworkBehaviour
{
    // Flagi zadań
    public bool climbedLadder = false;
    public bool collectedItem = false;
    public bool lookedAround = false;
    
    [Header("Nowe Zadania")]
    public bool enteredBush = false; // Zadanie: Krzak
    public bool visitedBase = false;  // Zadanie: Baza

    // Funkcja wywoływana, gdy gracz wejdzie na drabinę (np. przez skrypt drabiny)
    public void CompleteClimbTask()
    {
        if (!climbedLadder)
        {
            climbedLadder = true;
            Debug.Log("Zadanie: Drabina zaliczona!");
        }
    }

    // Automatyczne wykrywanie wejścia w krzak lub bazę
    private void OnTriggerEnter(Collider other)
    {
        // Sprawdzamy postępy tylko dla lokalnego gracza
        if (!isLocalPlayer) return;

        // Obsługa zadania: Krzak
        if (other.CompareTag("Krzak") && !enteredBush)
        {
            enteredBush = true;
            Debug.Log("Zadanie wykonane: Gracz wszedł w krzak!");
        }

        // Obsługa zadania: Baza
        if (other.CompareTag("Baza") && !visitedBase)
        {
            visitedBase = true;
            Debug.Log("Zadanie wykonane: Odwiedzono Bazę!");
        }
    }
}
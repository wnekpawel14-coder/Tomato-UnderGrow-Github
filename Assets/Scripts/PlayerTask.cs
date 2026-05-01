using UnityEngine;
using Mirror;

public class PlayerTasks : NetworkBehaviour
{
    [Header("Postęp Fabuły")]
    public int currentQuestStage = 0; // 0 = brak, 1 = drabina, 2 = krzak, 3 = baza

    [Header("Flagi zadań")]
    public bool climbedLadder = false;
    public bool enteredBush = false; 
    public bool visitedBase = false;

    // Funkcja wywoływana przez Ketchupa, by podnieść etap gry
    public void SetQuestStage(int stage)
    {
        if (stage > currentQuestStage)
        {
            currentQuestStage = stage;
            Debug.Log("<color=cyan>Nowy etap zadania: " + stage + "</color>");
        }
    }

    public void CompleteClimbTask()
    {
        // Zaliczamy tylko, jeśli jesteśmy na etapie 1 (Drabina)
        if (currentQuestStage == 1 && !climbedLadder)
        {
            climbedLadder = true;
            Debug.Log("Zadanie: Drabina zaliczona!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer) return;

        // Zadanie 2: Krzak (działa tylko jeśli etap == 2)
        if (other.CompareTag("Krzak") && currentQuestStage == 2 && !enteredBush)
        {
            enteredBush = true;
            Debug.Log("Zadanie: Krzak zaliczony!");
        }

        // Zadanie 3: Baza (działa tylko jeśli etap == 3)
        if (other.CompareTag("Baza") && currentQuestStage == 3 && !visitedBase)
        {
            visitedBase = true;
            Debug.Log("Zadanie: Baza zaliczona!");
        }
    }
}
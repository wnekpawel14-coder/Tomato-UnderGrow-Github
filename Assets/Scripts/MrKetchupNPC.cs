using UnityEngine;
using Mirror;

public class MrKetchupNPC : MonoBehaviour
{
    // --- DEFINICJE ZMIENNYCH (To ich brakowało!) ---
    private bool hasIntroduced = false;
    private PlayerTasks localPlayerTasks;

    void Update()
    {
        // Sprawdzamy czy gracz jest blisko i nacisnął E
        if (NetworkClient.localPlayer != null)
        {
            float dist = Vector3.Distance(transform.position, NetworkClient.localPlayer.transform.position);
            
            if (dist < 4f && Input.GetKeyDown(KeyCode.E))
            {
                // Pobieramy skrypt zadań z lokalnego gracza, jeśli jeszcze go nie mamy
                if (localPlayerTasks == null)
                {
                    localPlayerTasks = NetworkClient.localPlayer.GetComponent<PlayerTasks>();
                }

                HandleConversation();
            }
        }
    }

    void HandleConversation()
    {
        if (DialogueManager.instance.dialoguePanel.activeSelf)
        {
            DialogueManager.instance.DisplayNextSentence();
            return;
        }

        // 1. Powitanie
        if (!hasIntroduced)
        {
            DialogueManager.instance.StartDialogue(new string[] { 
                "Siemanko! Jestem Mr. Ketchup.",
                "Zrób dla mnie parę rzeczy, a będziemy kwita." 
            });
            hasIntroduced = true;
        }
        // 2. Zadanie: DRABINA
        else if (!localPlayerTasks.climbedLadder)
        {
            DialogueManager.instance.StartDialogue(new string[] { 
                "Twoje pierwsze zadanie: Wejdź na tę drabinę!", 
                "Grawitacja nie powinna być dla Ciebie przeszkodą." 
            });
        }
        // 3. Zadanie: KRZAK
        else if (!localPlayerTasks.enteredBush)
        {
            DialogueManager.instance.StartDialogue(new string[] { 
                "Nieźle z tą drabiną! Teraz drugie zadanie.",
                "Skocz w te gęste krzaki i sprawdź, czy nic tam nie siedzi!" 
            });
        }
        // 4. Zadanie: BAZA
        else if (!localPlayerTasks.visitedBase)
        {
            DialogueManager.instance.StartDialogue(new string[] { 
                "Dobra robota! Ostatnia sprawa.",
                "Odwiedź naszą Bazę, musisz wiedzieć gdzie się schronić." 
            });
        }
        // 5. KONIEC
        else
        {
            DialogueManager.instance.StartDialogue(new string[] { 
                "Gratulacje, mordo!", 
                "Zaliczyłeś wszystko. Jesteś teraz szefem tej okolicy!" 
            });
        }
    }
}
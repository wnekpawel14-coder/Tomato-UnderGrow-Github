using UnityEngine;
using Mirror;

public class MrKetchupNPC : MonoBehaviour
{
    private bool hasIntroduced = false;
    private PlayerTasks localPlayerTasks;
    
    public float zasiegRozmowy = 4f;
    public float zasiegZamkniecia = 6f;

    void Update()
    {
        if (NetworkClient.localPlayer != null)
        {
            float dist = Vector3.Distance(transform.position, NetworkClient.localPlayer.transform.position);
            
            if (dist < zasiegRozmowy && Input.GetKeyDown(KeyCode.E))
            {
                if (localPlayerTasks == null)
                    localPlayerTasks = NetworkClient.localPlayer.GetComponent<PlayerTasks>();

                HandleConversation();
            }

            if (dist > zasiegZamkniecia && DialogueManager.instance.dialoguePanel.activeSelf)
            {
                DialogueManager.instance.dialoguePanel.SetActive(false);
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

        // 1. POWITANIE -> Aktywuje Etap 1
        if (!hasIntroduced)
        {
            DialogueManager.instance.StartDialogue(new string[] { 
                "Siemanko! Jestem Mr. Ketchup.",
                "Słuchaj, mam dla Ciebie parę spraw, ale po kolei." 
            });
            localPlayerTasks.SetQuestStage(1); // Odblokuj Drabinę
            hasIntroduced = true;
        }
        // 2. JEŚLI ZROBIŁ DRABINĘ -> Aktywuje Etap 2
        else if (localPlayerTasks.climbedLadder && localPlayerTasks.currentQuestStage == 1)
        {
            DialogueManager.instance.StartDialogue(new string[] { 
                "Świetnie! Skoro umiesz się wspinać, sprawdź te krzaki.",
                "Podejdź do nich blisko i zobacz czy są bezpieczne." 
            });
            localPlayerTasks.SetQuestStage(2); // Odblokuj Krzak
        }
        // 3. JEŚLI ZROBIŁ KRZAK -> Aktywuje Etap 3
        else if (localPlayerTasks.enteredBush && localPlayerTasks.currentQuestStage == 2)
        {
            DialogueManager.instance.StartDialogue(new string[] { 
                "W krzakach czysto? Dobra robota.",
                "Na koniec leć do Bazy zameldować, że wszystko gra!" 
            });
            localPlayerTasks.SetQuestStage(3); // Odblokuj Bazę
        }
        // 4. JEŚLI NIC NOWEGO NIE ZROBIŁ -> Przypomnienie
        else if (!localPlayerTasks.visitedBase)
        {
            string przypomnienie = "No i jak tam? Robota sama się nie zrobi!";
            if (localPlayerTasks.currentQuestStage == 1) przypomnienie = "Leć na tę drabinę!";
            if (localPlayerTasks.currentQuestStage == 2) przypomnienie = "Sprawdź te krzaki!";
            if (localPlayerTasks.currentQuestStage == 3) przypomnienie = "Baza czeka na raport!";

            DialogueManager.instance.StartDialogue(new string[] { przypomnienie });
        }
        // 5. KONIEC WSZYSTKIEGO
        else
        {
            DialogueManager.instance.StartDialogue(new string[] { "Dzięki mordo, zadania wykonane!" });
        }
    }
}
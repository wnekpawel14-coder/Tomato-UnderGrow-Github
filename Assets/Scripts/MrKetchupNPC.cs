using UnityEngine;

public class MrKetchupNPC : MonoBehaviour
{
    public float interactionRadius = 4f;
    private Transform localPlayerTransform;
    private PlayerTasks localPlayerTasks;

    // Faza rozmowy
    private bool hasIntroduced = false; 

    [Header("Dialogi - Faza 1: Powitanie")]
    public string[] introLines = { 
        "Siema, mordo! Heh, patrzysz na mnie i myślisz: 'Czemu ta kapsuła jest taka czerwona?'",
        "Ksywa Ketchup nie wzięła się znikąd, ale o tym pogadamy później.",
        "Słuchaj, zanim Cię wpuszczę na głęboką wodę, muszę sprawdzić czy ogarniasz podstawy.",
        "Widzisz tę drabinę? Wejdź na samą górę i wróć do mnie. Muszę wiedzieć, że nie masz lęku wysokości!" 
    };

    [Header("Dialogi - Faza 2: Przypomnienie")]
    public string[] reminderLines = { 
        "No i co? Drabina sama się nie przejdzie!", 
        "Dajesz, góra-dół i wracaj pogadać." 
    };

    [Header("Dialogi - Faza 3: Nagroda/Koniec")]
    public string[] successLines = { 
        "O! I to rozumiem! Widzę, że grawitacja Ci nie straszna.", 
        "Dobra, jesteś gotowy. Rozejrzyj się za fantami i baw się dobrze!" 
    };

    void Update()
    {
        // Szukanie lokalnego gracza
        if (localPlayerTransform == null)
        {
            if (Mirror.NetworkClient.localPlayer != null)
            {
                localPlayerTransform = Mirror.NetworkClient.localPlayer.transform;
                localPlayerTasks = localPlayerTransform.GetComponent<PlayerTasks>();
            }
            return;
        }

        float distance = Vector3.Distance(transform.position, localPlayerTransform.position);

        if (distance <= interactionRadius)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                HandleConversation();
            }
        }
        else
        {
            // Auto-zamykanie przy odejściu
            if (DialogueManager.instance.dialoguePanel.activeSelf)
            {
                DialogueManager.instance.dialoguePanel.SetActive(false);
            }
        }
    }

    void HandleConversation()
    {
        // 1. Jeśli panel już jest otwarty, po prostu przewijaj tekst
        if (DialogueManager.instance.dialoguePanel.activeSelf)
        {
            DialogueManager.instance.DisplayNextSentence();
            return;
        }

        // 2. Logika wyboru dialogu przy otwieraniu:
        
        // KROK 1: Przedstawienie się
        if (!hasIntroduced)
        {
            DialogueManager.instance.StartDialogue(introLines);
            hasIntroduced = true; // Zapamiętujemy, że już się poznaliśmy
        }
        // KROK 2: Sprawdzenie czy zadanie wykonane
        else if (localPlayerTasks != null && localPlayerTasks.climbedLadder)
        {
            DialogueManager.instance.StartDialogue(successLines);
        }
        // KROK 3: Przypomnienie o zadaniu
        else
        {
            DialogueManager.instance.StartDialogue(reminderLines);
        }
    }
}
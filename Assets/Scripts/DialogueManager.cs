using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI textDisplay;
    public GameObject continueHint; // Napis "[E] Dalej"

    [Header("Settings")]
    public float typingSpeed = 0.04f;

    private Queue<string> sentences;
    private bool isTyping = false;
    private string currentSentence;

    void Awake()
    {
        instance = this;
        sentences = new Queue<string>();
        if(dialoguePanel) dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string[] dialogue)
    {
        StopAllCoroutines(); // Reset pisania
        dialoguePanel.SetActive(true);
        sentences.Clear();

        foreach (string sentence in dialogue)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (isTyping)
        {
            // Przyspieszanie pisania
            StopAllCoroutines();
            textDisplay.text = currentSentence;
            isTyping = false;
            if(continueHint) continueHint.SetActive(true);
            return;
        }

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentSentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        textDisplay.text = "";
        if(continueHint) continueHint.SetActive(false);

        foreach (char letter in sentence.ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        if(continueHint) continueHint.SetActive(true);
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isTyping = false;
        dialoguePanel.SetActive(false);
    }
}
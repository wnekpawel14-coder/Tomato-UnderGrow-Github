using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System.Collections;

public partial class HealthSystem : NetworkBehaviour
{
    [Header("Statystyki")]
    public int maxHp = 100;
    [SyncVar(hook = nameof(OnHpChanged))]
    public int currentHp;

    [Header("UI - Przypisz w Inspektorze")]
    public TextMeshProUGUI hpText; // Tekst z napisem HP
    public Image redVignette;      // Czerwony obrazek na cały ekran

    void Start()
    {
        // Tylko serwer ustawia początkowe życie
        if (isServer) 
        {
            currentHp = maxHp;
        }
        UpdateHpUI();
    }

    // Funkcja do zadawania obrażeń (wywoływana na serwerze)
    public void TakeDamage(int amount)
    {
        if (!isServer) return;
        
        currentHp -= amount;
        if (currentHp < 0) currentHp = 0;
    }

    // Wykrywa zmianę HP i odpala efekty u gracza
    void OnHpChanged(int oldHp, int newHp)
    {
        UpdateHpUI();
        
        // Jeśli straciliśmy HP, błyśnij ekranem (tylko u lokalnego gracza)
        if (newHp < oldHp && isLocalPlayer)
        {
            StopAllCoroutines(); // Zatrzymaj poprzednie błyskanie, jeśli jeszcze trwa
            StartCoroutine(FlashRedScreen());
        }
    }

    void UpdateHpUI()
    {
        if (isLocalPlayer && hpText != null)
        {
            hpText.text = "HP: " + currentHp; // Wyświetla napis np. "HP: 100"
        }
    }

    IEnumerator FlashRedScreen()
    {
        if (redVignette == null) yield break;

        // Ustawienie widoczności czerwieni
        Color c = redVignette.color;
        c.a = 0.5f; // Jak mocno ma być czerwono (0.5 = 50%)
        redVignette.color = c;

        // Powolne znikanie czerwieni
        while (c.a > 0)
        {
            c.a -= Time.deltaTime * 1.0f; // Szybkość wygasania
            redVignette.color = c;
            yield return null;
        }
    }
}
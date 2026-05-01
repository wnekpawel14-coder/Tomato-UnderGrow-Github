using UnityEngine;
using TMPro;

public class PlayerStamina : MonoBehaviour
{
    [Header("Ustawienia Energii")]
    public float maxStamina = 100f;
    private float currentStamina;
    public float zuzycieSprint = 25f; 
    public float regeneracja = 20f;
    
    [Header("Interfejs")]
    public TextMeshProUGUI staminaStatus; // Napis "STAMINA" lub pasek

    private CharacterController controller;
    private bool czyBiega = false;

    // Prędkości
    public float predkoscChodu = 5f;
    public float predkoscSprintu = 11f;

    void Start()
    {
        currentStamina = maxStamina;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool chceBiec = Input.GetKey(KeyCode.LeftShift);
        bool ruszaSie = controller.velocity.magnitude > 0.1f;

        // Logika sprintu
        if (chceBiec && ruszaSie && currentStamina > 0)
        {
            czyBiega = true;
            currentStamina -= zuzycieSprint * Time.deltaTime;
        }
        else
        {
            czyBiega = false;
            if (currentStamina < maxStamina)
            {
                currentStamina += regeneracja * Time.deltaTime;
            }
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        AktualizujUI();
    }

    void AktualizujUI()
    {
        if (staminaStatus == null) return;

        // Zamiast liczb, zmieniamy przezroczystość lub kolor napisu
        // Gdy masz mało energii, napis "STAMINA" zacznie znikać lub blednąć
        float procent = currentStamina / maxStamina;
        
        if (procent > 0.99f)
        {
            staminaStatus.text = ""; // Ukryj napis, gdy stamina jest pełna (czysty ekran)
        }
        else
        {
            staminaStatus.text = "<color=yellow>BIEGNIJ!</color>";
            // Zmieniamy przezroczystość (Alfa) napisu w zależności od energii
            staminaStatus.alpha = procent; 
        }
    }

    public float PobierzPredkosc()
    {
        return czyBiega ? predkoscSprintu : predkoscChodu;
    }
}
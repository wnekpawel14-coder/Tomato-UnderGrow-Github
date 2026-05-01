using UnityEngine;
using TMPro;

public class SimpleHealth : MonoBehaviour
{
    [Header("Ustawienia HP")]
    public int zdrowie = 100;
    public int maxZdrowie = 100;
    public TextMeshProUGUI tekstHP;

    [Header("Regeneracja (Auto-Leczenie)")]
    public bool czyRegenerowac = true;
    public int punktyRegen = 2;          // Ile HP dodaje
    public float coIleSekund = 1f;       // Częstotliwość dodawania
    public float opoznieniePoHicie = 5f; // Ile sekund czekamy po dostaniu obrażeń
    
    private float licznikRegen;
    private float czasOdOstatniegoHita;

    [Header("Ustawienia Upadku")]
    public float minimalnaWysokoscUpadku = 4f; 
    public int obrazeniaZaUpadek = 10;

    private CharacterController controller;
    private float najwyzszyPunktWPowietrzu;
    private bool czyBylemWPowietrzu = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        OdswiezUI();
    }

    void Update()
    {
        ObslugaUpadku();
        ObslugaRegeneracji();
    }

    void ObslugaRegeneracji()
    {
        if (!czyRegenerowac || zdrowie >= maxZdrowie) return;

        // Liczymy czas, który upłynął od ostatniego odjęcia HP
        czasOdOstatniegoHita += Time.deltaTime;

        // Jeśli minęło już wystarczająco dużo czasu (np. 5 sekund)
        if (czasOdOstatniegoHita >= opoznieniePoHicie)
        {
            licznikRegen += Time.deltaTime;

            if (licznikRegen >= coIleSekund)
            {
                DodajHP(punktyRegen);
                licznikRegen = 0;
            }
        }
    }

    void ObslugaUpadku()
    {
        if (controller == null) return;

        if (!controller.isGrounded)
        {
            if (!czyBylemWPowietrzu)
            {
                czyBylemWPowietrzu = true;
                najwyzszyPunktWPowietrzu = transform.position.y;
            }
            else if (transform.position.y > najwyzszyPunktWPowietrzu)
            {
                najwyzszyPunktWPowietrzu = transform.position.y;
            }
        }
        else if (controller.isGrounded && czyBylemWPowietrzu)
        {
            float dystansUpadku = najwyzszyPunktWPowietrzu - transform.position.y;

            if (dystansUpadku >= minimalnaWysokoscUpadku)
            {
                ZabierzHP(obrazeniaZaUpadek);
            }
            czyBylemWPowietrzu = false;
        }
    }

    public void ZabierzHP(int ile)
    {
        zdrowie -= ile;
        if (zdrowie < 0) zdrowie = 0;
        
        // KLUCZOWY MOMENT: Resetujemy licznik czasu przy każdym otrzymaniu obrażeń
        czasOdOstatniegoHita = 0; 
        licznikRegen = 0;

        OdswiezUI();
    }

    public void DodajHP(int ile)
    {
        zdrowie += ile;
        if (zdrowie > maxZdrowie) zdrowie = maxZdrowie;
        OdswiezUI();
    }

    void OdswiezUI()
    {
        if (tekstHP != null)
        {
            tekstHP.text = "<color=red>HP:</color> " + zdrowie;
        }
    }
}
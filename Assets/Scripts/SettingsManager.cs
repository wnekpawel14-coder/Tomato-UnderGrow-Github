using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    [Header("Referencje")]
    public AudioMixer mainMixer;
    public Slider musicSlider;
    public Slider sensSlider;
    public TMP_Dropdown qualityDropdown;

    void Start()
    {
        // --- KONFIGURACJA DROPDOWN (GRAFIKA) ---
        if (qualityDropdown != null)
        {
            // Czyścimy stare opcje i dodajemy nasze trzy
            qualityDropdown.ClearOptions();
            List<string> options = new List<string> { "Low", "Medium", "Ultra" };
            qualityDropdown.AddOptions(options);

            // Wczytujemy zapisany poziom (domyślnie 1, czyli Medium)
            int savedQual = PlayerPrefs.GetInt("QualityLevel", 1);
            
            // Ustawiamy dropdown i faktyczną jakość w Unity
            qualityDropdown.value = savedQual;
            ApplyQuality(savedQual);

            qualityDropdown.onValueChanged.AddListener(SetQuality);
        }

        // --- MUZYKA ---
        if (musicSlider != null && mainMixer != null)
        {
            float vol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            musicSlider.value = vol;
            SetMusicVolume(vol);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        // --- CZUŁOŚĆ ---
        if (sensSlider != null)
        {
            sensSlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 2.0f);
            sensSlider.onValueChanged.AddListener(SetSensitivity);
        }
    }

    public void SetQuality(int index)
    {
        ApplyQuality(index);
        PlayerPrefs.SetInt("QualityLevel", index);
        PlayerPrefs.Save(); // Wymuszamy zapis
    }

    private void ApplyQuality(int index)
    {
        // Mapujemy nasz dropdown na Quality Settings w Unity:
        // index 0 -> Low (Unity Level 0)
        // index 1 -> Medium (Unity Level 2 lub 3 w zależności od projektu)
        // index 2 -> Ultra (Unity Level 5 lub najwyższy)
        
        // Najbezpieczniejsza metoda dla 3 poziomów:
        if (index == 0) QualitySettings.SetQualityLevel(0, true);      // Low
        else if (index == 1) QualitySettings.SetQualityLevel(2, true); // Medium
        else if (index == 2) QualitySettings.SetQualityLevel(5, true); // Ultra (zazwyczaj najwyższy)
    }

    public void SetMusicVolume(float value)
    {
        if (mainMixer != null)
        {
            float dB = Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20;
            mainMixer.SetFloat("MusicVol", dB);
            PlayerPrefs.SetFloat("MusicVolume", value);
        }
    }

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        
        // Opcjonalnie: Jeśli masz gracza na scenie, możesz od razu zaktualizować jego lookSpeed
        // Movement player = FindObjectOfType<Movement>();
        // if(player != null) player.lookSpeed = value;
    }
}
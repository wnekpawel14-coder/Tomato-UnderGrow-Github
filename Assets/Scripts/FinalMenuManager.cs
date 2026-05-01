using UnityEngine;
using Mirror;

public class FinalMenuManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public bool isPaused = false;

    void Awake()
    {
        isPaused = false;
        // Na starcie upewniamy się, że kursor jest zablokowany do gry
        UnpauseGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf) CloseSettings();
            else if (isPaused) UnpauseGame();
            else PauseGame();
        }

        // Jeśli klikniesz w tło, wymuszamy powrót kursora (tylko w pauzie)
        if (isPaused && Cursor.lockState != CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        NotifyPlayer(true);
    }

    public void UnpauseGame()
    {
        isPaused = false;
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        NotifyPlayer(false);
    }

    public void OpenSettings() { mainPanel.SetActive(false); settingsPanel.SetActive(true); }
    public void CloseSettings() { settingsPanel.SetActive(false); mainPanel.SetActive(true); }

    private void NotifyPlayer(bool pauseActive)
    {
        if (NetworkClient.localPlayer != null)
        {
            Movement mov = NetworkClient.localPlayer.GetComponent<Movement>();
            if (mov != null) mov.isPausedFromMenu = pauseActive;
        }
    }
}
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerToolbar : NetworkBehaviour
{
    [Header("Ekwipunek Gracza")]
    public ItemData[] inventory = new ItemData[5];
    public int[] amounts = new int[5];
    public int activeSlot = 0;

    [Header("Ustawienia UI")]
    public GameObject uiCanvas;
    public GameObject[] slotHighlights; 
    public Image[] slotIcons; 

    [Header("Widok w Ręce (Viewmodel)")]
    public Transform rekaGracza; // NOWOŚĆ: Nasz punkt w kamerze
    private GameObject aktualnyModelWReku; // Zapamiętuje, co aktualnie trzymamy

    public override void OnStartLocalPlayer()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SprawdzScene(SceneManager.GetActiveScene().name);
        AktualizujIkonyUI(); 
    }

    public override void OnStartClient()
    {
        if (!isLocalPlayer && uiCanvas != null) uiCanvas.SetActive(false);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isLocalPlayer) SprawdzScene(scene.name);
    }

    void SprawdzScene(string nazwaSceny)
    {
        if (nazwaSceny == "MainMenu") { if (uiCanvas != null) uiCanvas.SetActive(false); }
        else { if (uiCanvas != null) uiCanvas.SetActive(true); ZmienSlot(0); }
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        if (uiCanvas != null && !uiCanvas.activeSelf) return; 

        int starySlot = activeSlot;

        if (Input.GetKeyDown(KeyCode.Alpha1)) ZmienSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ZmienSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ZmienSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ZmienSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ZmienSlot(4);

        float scroll = Input.mouseScrollDelta.y;
        if (scroll > 0f) { int ns = activeSlot - 1; ZmienSlot(ns < 0 ? slotHighlights.Length - 1 : ns); }
        else if (scroll < 0f) { int ns = activeSlot + 1; ZmienSlot(ns >= slotHighlights.Length ? 0 : ns); }
    }

    void ZmienSlot(int index)
    {
        activeSlot = index;
        for (int i = 0; i < slotHighlights.Length; i++)
        {
            if (slotHighlights[i] != null) slotHighlights[i].SetActive(i == activeSlot);
        }
        
        OdswiezModelWReku(); // NOWOŚĆ: Po zmianie slota, zmień model w ręce!
    }

    // --- NOWOŚĆ: MAGIA TRZYMANIA W RĘCE ---
    public void OdswiezModelWReku()
    {
        // 1. Zniszcz to, co trzymaliśmy do tej pory (żeby przedmioty się nie nakładały)
        if (aktualnyModelWReku != null)
        {
            Destroy(aktualnyModelWReku);
        }

        // 2. Sprawdź, co teraz wybraliśmy
        ItemData aktywnyItem = GetActiveItem();

        // 3. Jeśli mamy przedmiot i ten przedmiot ma swój "model do ręki"...
        if (aktywnyItem != null && aktywnyItem.weaponPrefab != null)
        {
            // Stwórz model i przypnij go do naszej niewidzialnej Ręki Gracza
            aktualnyModelWReku = Instantiate(aktywnyItem.weaponPrefab, rekaGracza);
            
            // Wyzeruj pozycję, żeby model wskoczył idealnie w środek uchwytu
            aktualnyModelWReku.transform.localPosition = Vector3.zero;
            aktualnyModelWReku.transform.localRotation = Quaternion.identity;
        }
    }

    public bool DodajPrzedmiot(ItemData item, int ilosc)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = item;
                amounts[i] = ilosc;
                AktualizujIkonyUI(); 
                if (i == activeSlot) OdswiezModelWReku(); // Jeśli podnieśliśmy do aktywnego, pokaż w ręce
                return true; 
            }
        }
        return false; 
    }

    public void UsunAktywnyPrzedmiot()
    {
        inventory[activeSlot] = null;
        amounts[activeSlot] = 0;
        AktualizujIkonyUI(); 
        OdswiezModelWReku(); // Odśwież (czyli usuń model z ręki, bo slot jest pusty)
    }

    void AktualizujIkonyUI()
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (slotIcons[i] != null)
            {
                if (inventory[i] != null) { slotIcons[i].sprite = inventory[i].icon; slotIcons[i].enabled = true; }
                else { slotIcons[i].sprite = null; slotIcons[i].enabled = false; }
            }
        }
    }

    public ItemData GetActiveItem() { return inventory[activeSlot]; }
    void OnDestroy() { if (isLocalPlayer) SceneManager.sceneLoaded -= OnSceneLoaded; }
}
using Mirror;
using UnityEngine;

public class PlayerToolbar : NetworkBehaviour
{
    [Header("Ekwipunek Gracza")]
    // Tworzymy 5 miejsc na przedmioty i 5 miejsc na ich ilość
    public ItemData[] inventory = new ItemData[5];
    public int[] amounts = new int[5];

    [Header("Aktualny Wybór")]
    public int activeSlot = 0;

    void Update()
    {
        // Jeśli to nie jest nasz gracz, nie pozwalamy mu wciskać naszych klawiszy!
        if (!isLocalPlayer) return;

        // Zmiana slotów klawiszami 1-5
        if (Input.GetKeyDown(KeyCode.Alpha1)) ZmienSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ZmienSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ZmienSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ZmienSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ZmienSlot(4);
    }

    void ZmienSlot(int index)
    {
        activeSlot = index;
        
        // Sprawdzamy, czy w tym slocie w ogóle coś jest
        string nazwaPrzedmiotu = (inventory[index] != null) ? inventory[index].itemName : "Pusty slot";
        
        Debug.Log($"[Ekwipunek] Wybrano slot {index + 1}. Trzymasz: {nazwaPrzedmiotu}");
    }

    // Pomocnicza funkcja, którą później wykorzystamy do stawiania Growboxów i siania
    public ItemData GetActiveItem()
    {
        return inventory[activeSlot];
    }
}
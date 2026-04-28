using Mirror;
using UnityEngine;

// Zauważ, że dziedziczy z NetworkBehaviour!
public class ItemPickup : NetworkBehaviour
{
    public ItemData itemData; // Odsyła do danych na dysku
    public int ilosc = 1;
}
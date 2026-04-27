using UnityEngine;

[CreateAssetMenu(fileName = "Nowy Przedmiot", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject 
{
    public string itemName;
    public Sprite icon;             // Ikonka do paska na dole ekranu
    public GameObject weaponPrefab; // Model trzymany w dłoni gracza

    [Header("System Budowania")]
    public bool czyBudynek;         // Zaznacz, jeśli to np. Growbox lub Generator
    public GameObject prefabBudynku; // Prawdziwy, duży obiekt stawiany na ziemi
    public GameObject prefabPodgladu; // Półprzezroczysty hologram do celowania
}
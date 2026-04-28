using UnityEngine;

[CreateAssetMenu(fileName = "Nowy Przedmiot", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject 
{
    public string itemName;
    public Sprite icon;             // Ikonka do paska
    public GameObject weaponPrefab; // Opcjonalny model do łapy

    [Header("Fizyka i Świat")]
    public GameObject prefabDropu; // NOWOŚĆ: Model, który pojawia się na ziemi

    [Header("System Budowania")]
    public bool czyBudynek;         
    public GameObject prefabBudynku; 
    public GameObject prefabPodgladu; 
}
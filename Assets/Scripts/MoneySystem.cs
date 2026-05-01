using UnityEngine;
using Mirror;
using TMPro;

public class MoneySystem : NetworkBehaviour
{
    [Header("Ustawienia Portfela")]
    [SyncVar(hook = nameof(OnMoneyChanged))]
    public int currentMoney = 0;

    [Header("UI Reference")]
    [SerializeField] // To wymusza pokazanie pola nawet przy błędach
    public TextMeshProUGUI moneyText;

    void Start()
    {
        UpdateMoneyUI();
    }

    [Server]
    public void AddMoney(int amount)
    {
        currentMoney += amount;
    }

    void OnMoneyChanged(int oldAmount, int newAmount)
    {
        UpdateMoneyUI();
    }

    void UpdateMoneyUI()
    {
        if (isLocalPlayer && moneyText != null)
        {
            moneyText.text = "Kasa: $" + currentMoney;
        }
    }

    [Command]
    public void CmdAddMoney(int amount)
    {
        AddMoney(amount);
    }
}
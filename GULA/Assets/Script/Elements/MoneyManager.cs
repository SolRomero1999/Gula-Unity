using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    [Header("Money Display UI")]
    public TMP_Text moneyText;
    public GameObject panelBackgroundPanel;

    [Header("Config")]
    public int startingMoney = 150;

    private int money;
    private int totalEarnings;
    private int totalExpenses;

    public int Money => money;
    public int TotalEarnings => totalEarnings;
    public int TotalExpenses => totalExpenses;
    public int NetMoney => totalEarnings - totalExpenses;

    void Start()
    {
        money = startingMoney;
        totalEarnings = 0;
        totalExpenses = 0;
        UpdateMoneyDisplay();
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;

        money += amount;
        totalEarnings += amount;
        UpdateMoneyDisplay();
    }

    public bool SpendMoney(int amount)
    {
        if (amount <= 0 || money < amount) return false;

        money -= amount;
        totalExpenses += amount;
        UpdateMoneyDisplay();
        return true;
    }

    public void ResetMoney()
    {
        money = 0;
        totalEarnings = 0;
        totalExpenses = 0;
        UpdateMoneyDisplay();
    }

    private void UpdateMoneyDisplay()
    {
        if (moneyText != null)
            moneyText.text = $"${money}";
    }

    public void Cleanup()
    {
        if (moneyText != null) Destroy(moneyText.gameObject);
        if (panelBackgroundPanel != null) Destroy(panelBackgroundPanel);
    }
}
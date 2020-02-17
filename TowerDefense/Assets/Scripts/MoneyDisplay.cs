using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MoneyDisplay : MonoBehaviour
{
    [SerializeField] private int money = 250;
    private Text moneyBox;

    private void Start()
    {
        moneyBox = GetComponent<Text>();
        UpdateMoneyDisplay();
    }

    private void UpdateMoneyDisplay()
    {
        moneyBox.text = money.ToString();
    }

    public void AddMoney(int money)
    {
        this.money += money;
        UpdateMoneyDisplay();
    }

    public void SpendMoney(int money)
    {
        if (this.money < money) return;
        this.money -= money;
        UpdateMoneyDisplay();
    }

    public bool CanAfford(int amount)
    {
        if (amount > money) return false;
        else
        {
            SpendMoney(amount);
            return true;
        }
    }
}

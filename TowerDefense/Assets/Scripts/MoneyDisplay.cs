using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MoneyDisplay : MonoBehaviour
{
    [SerializeField] private int money = 250;
    [SerializeField] private int moneyPerSecond = 5;
    private float timer = 0f;
    private Text moneyBox;

    private void Start()
    {
        SetUpDifficulty();
        moneyBox = GetComponent<Text>();
        UpdateMoneyDisplay();
    }

    private void SetUpDifficulty()
    {
        float difficulty = PlayerPrefsController.GetDifficulty();
        difficulty = (float)Math.Round(difficulty, 2);
        money -= (int)(difficulty * money);
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
    
    private void CountTime(int moneyPerSecond)
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            timer -= 1f;
            AddMoney(moneyPerSecond);
        }
    }

    private void Update()
    {
        CountTime(moneyPerSecond);
    }
}

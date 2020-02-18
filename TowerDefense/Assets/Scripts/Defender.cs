using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Defender : MonoBehaviour
{
    [SerializeField] private int unitCost = 100;
    [SerializeField] private int moneyGrowth = 0;

    private MoneyDisplay moneyDisplay;
    private bool moneyDisplayFound = false;

    public int UnitCost { get => unitCost; }

    public void AddMoney()
    {
        if (!moneyDisplayFound)
        {
            moneyDisplay = FindObjectOfType<MoneyDisplay>();
            moneyDisplayFound = true;
        }
        moneyDisplay.AddMoney(moneyGrowth);
    }
}

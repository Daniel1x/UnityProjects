using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private GameObject deathVFX = null;
    private const string VFX_PARENT_NAME = "VFX";
    private static GameObject VFXParent = null;
    private static bool VFXParentCreated = false;
    private MoneyDisplay moneyDisplay = null;
    private bool moneyDisplayFound = false;

    public void DealDamage(float damage)
    {
        health -= damage;
        CheckIfDestroyed();
    }

    private void CheckIfDestroyed()
    {
        if (health <= 0) Die();
    }

    private void Die()
    {
        TriggerDeathVFX();
        Destroy(gameObject);
    }

    private void AddMoney()
    {
        if (CompareTag("Attacker"))
        {
            FindMoneyDisplay();
            Attacker attacker = GetComponent<Attacker>();
            moneyDisplay.AddMoney(attacker.deathIncome);
        }
    }

    private void FindMoneyDisplay()
    {
        if (!moneyDisplayFound)
        {
            moneyDisplay = FindObjectOfType<MoneyDisplay>();
            if (moneyDisplay) moneyDisplayFound = true;
        }
    }

    private void TriggerDeathVFX()
    {
        if (!deathVFX) return;
        GameObject particle = Instantiate(deathVFX, transform.position, Quaternion.identity) as GameObject;
        HookUpVFXParent(particle);
        Destroy(particle, 1f);
        AddMoney();
    }

    private void HookUpVFXParent(GameObject particle)
    {
        CreateVFXParent();
        particle.transform.parent = VFXParent.transform;
    }

    private void CreateVFXParent()
    {
        if (!VFXParentCreated || !VFXParent)
        {
            VFXParentCreated = true;
            VFXParent = new GameObject(VFX_PARENT_NAME);
        }
    }
}

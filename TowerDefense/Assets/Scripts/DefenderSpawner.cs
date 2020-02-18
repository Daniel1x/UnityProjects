using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderSpawner : MonoBehaviour
{
    [SerializeField] private Defender defenderPrefab = null;
    [SerializeField] private MoneyDisplay moneyDisplay;
    private bool spawnEnabled = false;

    private void Start()
    {
        moneyDisplay = FindObjectOfType<MoneyDisplay>();
    }

    private void SpawnDefender(Vector2 spawnPosition)
    {
        if (defenderPrefab)
        {
            Defender defender = Instantiate(defenderPrefab, spawnPosition, Quaternion.identity, transform);
        }
    }
    
    private Vector2 GetSquareClicked()
    {
        Vector2 clickPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(clickPosition);
        Vector2 spawnPosition = SnapToGrid(worldPosition);
        return spawnPosition;
    }

    private Vector2 SnapToGrid(Vector2 click)
    {
        return new Vector2(Mathf.RoundToInt(click.x), Mathf.RoundToInt(click.y));
    }

    private void OnMouseDown()
    {
        TryToSpawn();
    }

    public void SetDefenderPrefab(Defender defender)
    {
        defenderPrefab = defender;
        spawnEnabled = true;
    }

    public void TryToSpawn()
    {
        if (spawnEnabled && moneyDisplay.CanAfford(defenderPrefab.UnitCost))
        {
            SpawnDefender(GetSquareClicked());
        }
    }
}

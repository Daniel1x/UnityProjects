using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsManager : MonoBehaviour
{
    [SerializeField] private DefenderButton[] defenderButtons = null;
    [SerializeField] private DefenderSpawner defenderSpawner = null;

    private void Start()
    {
        defenderButtons = GetComponentsInChildren<DefenderButton>();
        if (!defenderSpawner) defenderSpawner = FindObjectOfType<DefenderSpawner>();
    }

    public void HideAllButtons()
    {
        foreach(DefenderButton button in defenderButtons)
        {
            button.HideButton();
        }
    }

    public void SetDefenderPrefab(Defender defender)
    {
        defenderSpawner.SetDefenderPrefab(defender);
    }
}

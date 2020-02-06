using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    private Text healthTextBox;
    private PlayerHealth playerHealth;

    private void Start()
    {
        healthTextBox = GetComponent<Text>();
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    private void Update()
    {
        healthTextBox.text = "HP: " + playerHealth.Health.ToString();
    }
}

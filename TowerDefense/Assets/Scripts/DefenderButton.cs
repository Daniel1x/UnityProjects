using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class DefenderButton : MonoBehaviour
{
    [SerializeField] private Color hiddenColor = new Color(0.25f, 0.25f, 0.25f);
    [SerializeField] private Color highlightedColor = Color.white;
    [SerializeField] private Defender defenderPrefab = null;

    private ButtonsManager buttonsManager;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        buttonsManager = GetComponentInParent<ButtonsManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        buttonsManager.HideAllButtons();
        buttonsManager.SetDefenderPrefab(defenderPrefab);
        HighlightButton();
    }

    public void HideButton()
    {
        spriteRenderer.color = hiddenColor;
    }

    public void HighlightButton()
    {
        spriteRenderer.color = highlightedColor;
    }
}

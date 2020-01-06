using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Shape manager reference.
    /// </summary>
    [SerializeField]
    private ShapeManager shapeManager;
    /// <summary>
    /// Field in which the RMS error is displayed.
    /// </summary>
    [SerializeField]
    private Text errorText;
    /// <summary>
    /// Array of created levels.
    /// </summary>
    public MeshDataContainer[] levels;
    /// <summary>
    /// Loaded level.
    /// </summary>
    public int actualLevel = 0;

    /// <summary>
    /// If the game has been started.
    /// </summary>
    private bool started = false;

    /// <summary>
    /// Loading next level from array.
    /// </summary>
    public void LoadNextLevel()
    {
        if (started) actualLevel = (actualLevel + 1) % levels.Length;
        else started = true;
        SetupShapeManager();
    }

    /// <summary>
    /// Reseting shape of current mesh.
    /// </summary>
    public void ResetThisLevel()
    {
        shapeManager.RestartMeshGenerator();
        ResetErrorText();
    }

    /// <summary>
    /// Setting new target mesh shape and restarting shape manager.
    /// </summary>
    private void SetupShapeManager()
    {
        shapeManager.targetMesh = levels[actualLevel];
        shapeManager.RestartMeshGenerator();
        ResetErrorText();
    }

    /// <summary>
    /// Reseting text of the field.
    /// </summary>
    private void ResetErrorText()
    {
        errorText.text = "RMSE";
    }
    
    private void Update()
    {
        if (started) return;
        LoadNextLevel();
        started = true;
    }
}

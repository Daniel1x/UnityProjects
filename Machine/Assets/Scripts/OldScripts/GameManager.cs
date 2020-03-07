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
    /// Field in which messages are displayed.
    /// </summary>
    [SerializeField]
    private Text messageText;
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
    /// Time between two clicks needed to close the application.
    /// </summary>
    private float quitTime = 1f;
    /// <summary>
    /// Time counter.
    /// </summary>
    private float actualTime = 10f;

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
        messageText.text = "";
    }
    
    private void Update()
    {
        TryToQuit();

        if (started) return;
        LoadNextLevel();
        started = true;
    }

    /// <summary>
    /// Closing the application after double-clicking the Escape button.
    /// </summary>
    private void TryToQuit()
    {
        actualTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (actualTime <= quitTime) Application.Quit();
            actualTime = 0;
        }
    }
}

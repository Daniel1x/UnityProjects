using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Shape manager reference.
    /// </summary>
    public ShapeManager shapeManager;
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
    /// Setting new target mesh shape and restarting shape manager.
    /// </summary>
    private void SetupShapeManager()
    {
        shapeManager.targetMesh = levels[actualLevel];
        shapeManager.RestartMeshGenerator();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.y / Screen.height >= 0.9f)
            {
                if (Input.mousePosition.x / Screen.width < 0.5f)
                {
                    shapeManager.RestartMeshGenerator();
                }
                else
                {
                    LoadNextLevel();
                }
            }
        }
    }
}

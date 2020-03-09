using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Field in which the RMS error is displayed.
    /// </summary>
    [SerializeField] private Text errorText = null;
    /// <summary>
    /// Field in which messages are displayed.
    /// </summary>
    [SerializeField] private Text messageText = null;
    /// <summary>
    /// Reference to scriptable objects that consist 
    /// </summary>
    [SerializeField] private LevelSettings[] levels = null;
    /// <summary>
    /// Current level index.
    /// </summary>
    [SerializeField] private int levelIndex = 0;
    /// <summary>
    /// Scriptable object used to save level settings.
    /// </summary>
    [SerializeField] private LevelSettings saveLevelSettingsToSO = null;
    
    /// <summary>
    /// Time between two clicks needed to close the application.
    /// </summary>
    private float quitTime = 1f;
    /// <summary>
    /// Time counter.
    /// </summary>
    private float actualTime = 10f;
    
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

    public void ResetLevel()
    {
        StartNewLevel(levels[levelIndex]);
    }

    private void StartNewLevel(LevelSettings levelSettings)
    {
        DestroyAllChilds();
    }

    private void DestroyAllChilds()
    {
        int numberOfChilds = transform.childCount;
        Transform[] childs = GetComponentsInChildren<Transform>();
        for (int index = 1; index < numberOfChilds + 1; index++)
        {
            Destroy(childs[index].gameObject);
        }
    }

    public void SaveLevelToScriptableObject()
    {
        GenericMeshInfo[] genericMeshInfo = GetComponentsInChildren<GenericMeshInfo>();
        saveLevelSettingsToSO.SetMeshInfoArray(genericMeshInfo);
    }
    

}

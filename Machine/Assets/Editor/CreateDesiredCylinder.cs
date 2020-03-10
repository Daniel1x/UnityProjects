using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CreateDesiredCylinder : ScriptableWizard
{
    public DesiredCylinderSettings lastSettingsSO = null;
    private DesiredCylinderSettings actualSettingsSO = null;
    private bool lastSettingsLoaded = false;

    public Transform parentObject = null;
    public Material material = null;
    public string gameObjectName = "GenericCylinder";
    public string gameObjectTag = "GeneratedCylinder";
    public Vector3 spawnPosition = new Vector3();
    public Vector3 spawnRotationEuler = new Vector3();
    [Range(3, 36)] public int numberOfVerticesPerLayer = 12;
    [Range(2, 1024)] public int numberOfLayers = 10;
    [Range(0.025f, 10f)] public float hightOfOneLayer = 0.1f;
    [Range(0.025f, 100f)] public float widthOfCylinder = 1f;
    [Range(0f, 10f)] public float midpointHeightDifference = 0.1f;
    [Range(1, 100)] public int numberOfCylindersToCreate = 1;

    private const string meshName = "GenericMesh";
    private Vector3[] vertices;
    private int[] triangles;
    private Mesh mesh;
    private MeshCollider meshCollider;

    [MenuItem("Cylinder Creator/Create Desired Cylinder")]
    static void CreateWizard()
    {
        DisplayWizard<CreateDesiredCylinder>("Create Cylinder", "Create New Cylinder");
    }

    private void Awake()
    {
        isValid = false;
        helpString = "Remember to choose the material!";
        material = StaticCylinderCreator.FindDefaultMaterial("Metal");

        GetLastSettings(lastSettingsSO);
    }

    private void OnWizardCreate()
    {
        for (int i = 0; i < numberOfCylindersToCreate; i++)
            CreateOneCylinder();

        SetLastSettings(lastSettingsSO);
    }

    private void CreateOneCylinder()
    {
        Info thisCylinderInfo = new Info(numberOfVerticesPerLayer, numberOfLayers, hightOfOneLayer, widthOfCylinder, 
                                midpointHeightDifference, StaticCylinderCreator.CreateMagnitudesArray(numberOfLayers, widthOfCylinder));

        GameObject cylinderGO = StaticCylinderCreator.CreateCylinder(parentObject, gameObjectName, gameObjectTag,
                                                                     material.name, thisCylinderInfo, spawnPosition.y);
    }

    private void OnWizardUpdate()
    {
        isValid = material ? true : false;
        UpdateSettingsFromScriptableObject();
    }

    private void UpdateSettingsFromScriptableObject()
    {
        if (!lastSettingsLoaded && lastSettingsSO != null)
        {
            GetLastSettings(lastSettingsSO);
            lastSettingsLoaded = true;
            actualSettingsSO = lastSettingsSO;
        }
        else if (actualSettingsSO != lastSettingsSO)
        {
            GetLastSettings(lastSettingsSO);
            actualSettingsSO = lastSettingsSO;
        }


        if (!lastSettingsLoaded && lastSettingsSO != null)
        {
            GetLastSettings(lastSettingsSO);
            lastSettingsLoaded = true;
        }
    }

    private DesiredCylinderSettings FindLastCylinderSettings(string lastCylinderSettingsFileName)
    {
        DesiredCylinderSettings lastCylinderSettings = null;
        DesiredCylinderSettings[] cylinderSettings = Resources.FindObjectsOfTypeAll<DesiredCylinderSettings>();
        for (int i = 0; i < cylinderSettings.Length; i++)
            if (cylinderSettings[i].name == lastCylinderSettingsFileName) lastCylinderSettings = cylinderSettings[i];
        return lastCylinderSettings;
    }
    
    private void SetLastSettings(DesiredCylinderSettings lastSettingsSO)
    {
        DesiredCylinderSettings lastCylinderSettings = lastSettingsSO;
        if (lastCylinderSettings == null) return;
        lastCylinderSettings.gameObjectName = gameObjectName;
        lastCylinderSettings.gameObjectTag = gameObjectTag;
        lastCylinderSettings.spawnPosition = spawnPosition;
        lastCylinderSettings.spawnRotationEuler = spawnRotationEuler;
        lastCylinderSettings.numberOfVerticesPerLayer = numberOfVerticesPerLayer;
        lastCylinderSettings.numberOfLayers = numberOfLayers;
        lastCylinderSettings.hightOfOneLayer = hightOfOneLayer;
        lastCylinderSettings.widthOfCylinder = widthOfCylinder;
        lastCylinderSettings.midpointHeightDifference = midpointHeightDifference;
    }

    private void GetLastSettings(DesiredCylinderSettings lastSettingsSO)
    {
        DesiredCylinderSettings lastCylinderSettings = lastSettingsSO;
        if (lastCylinderSettings == null) return;
        gameObjectName = lastCylinderSettings.gameObjectName;
        gameObjectTag = lastCylinderSettings.gameObjectTag;
        spawnPosition = lastCylinderSettings.spawnPosition;
        spawnRotationEuler = lastCylinderSettings.spawnRotationEuler;
        numberOfVerticesPerLayer = lastCylinderSettings.numberOfVerticesPerLayer;
        numberOfLayers = lastCylinderSettings.numberOfLayers;
        hightOfOneLayer = lastCylinderSettings.hightOfOneLayer;
        widthOfCylinder = lastCylinderSettings.widthOfCylinder;
        midpointHeightDifference = lastCylinderSettings.midpointHeightDifference;
    }

}

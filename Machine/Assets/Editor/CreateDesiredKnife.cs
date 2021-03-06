﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CreateDesiredKnife : ScriptableWizard
{
    public Material material;
    public string gameObjectName = "GenericKnife";
    public string gameObjectTag = "GeneratedKnife";
    public Vector3 spawnPosition = new Vector3();
    public Vector3 spawnRotationEuler = new Vector3();

    [Range(5, 180)] public int numberOfCuttingPoints = 18;
    [Range(0.1f, 20f)] public float rangeOfRaycast = 2f;
    [Range(0.01f, 20f)] public float sphereRadius = 0.5f;
    
    public bool markMeshAsDynamic = false;
    public bool addRigidbody = false;
    public bool isRigidbodyKinematic = true;

    private const string meshName = "GenericKnife";
    private Vector3[] vertices;
    private int[] triangles;
    private Mesh mesh;
    private MeshCollider meshCollider;

    [MenuItem("Cylinder Creator/Create Desired Knife")]
    static void CreateWizard()
    {
        DisplayWizard<CreateDesiredKnife>("Create Knife", "Create New Knife");
    }

    private void Awake()
    {
        isValid = false;
        helpString = "Remember to choose the material!";
        material = FindMaterialWithName("Metal");
    }

    private Material FindMaterialWithName(string name)
    {
        Material material = null;
        Material[] materials = Resources.FindObjectsOfTypeAll<Material>();
        for (int i = 0; i < materials.Length; i++)
            if (materials[i].name == "Metal") material = materials[i];
        return material;
    }

    private void OnWizardCreate()
    {
        GameObject knifeGO = new GameObject();
        knifeGO.name = gameObjectName;
        knifeGO.tag = gameObjectTag;

        knifeGO.transform.localScale = (2f * sphereRadius) * Vector3.one;
        knifeGO.transform.position = spawnPosition;
        knifeGO.transform.rotation = Quaternion.Euler(spawnRotationEuler);

        MeshFilter meshFilter = knifeGO.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = knifeGO.AddComponent<MeshRenderer>();
        GenericKnife knife = knifeGO.AddComponent<GenericKnife>();
        GenericKnifeControll genericKnifeControll = knifeGO.AddComponent<GenericKnifeControll>();
        
        knife.CreateKnife(numberOfCuttingPoints, rangeOfRaycast);

        mesh = GetSphereMesh();
        mesh.name = meshName;
        meshFilter.mesh = mesh;

        if (addRigidbody)
        {
            Rigidbody rigidbody = knifeGO.AddComponent<Rigidbody>();
            rigidbody.isKinematic = isRigidbodyKinematic;
            //if (!isRigidbodyKinematic) meshCollider.convex = true;
        }

        if (material) meshRenderer.material = material;
    }
    
    private Mesh GetSphereMesh()
    {
        GameObject temporaryGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Mesh sphereMesh = temporaryGO.GetComponent<MeshFilter>().sharedMesh;
        DestroyImmediate(temporaryGO);
        return sphereMesh;
    }

    private void OnWizardUpdate()
    {
        isValid = material ? true : false;
    }
}

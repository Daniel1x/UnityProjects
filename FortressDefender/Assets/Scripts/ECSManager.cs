using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;

public class ECSManager : MonoBehaviour
{
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Text textBox = null;
    [SerializeField] private int entitiesToSpawn = 250;
    [SerializeField] public static int spawnedEntities = 0;

    public static EntityManager entityManager;
    public static Entity unitEntity;
    private BlobAssetStore store;
    private const string entitiesText = "Entities: ";

    private void Start()
    {
        store = new BlobAssetStore();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, store);
        unitEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(unitPrefab, settings);
    }

    private void SpawnEntities(int entitiesToSpawn)
    {
        for(int i = 0; i < entitiesToSpawn; i++)
        {
            Entity entity = entityManager.Instantiate(unitEntity);
            float3 position = new float3(UnityEngine.Random.Range(-3f, 3f), 0f, UnityEngine.Random.Range(-3f, 3f));
            entityManager.SetComponentData(entity, new Translation { Value = position });
        }
        spawnedEntities += entitiesToSpawn;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnEntities(entitiesToSpawn);
        }
        SpawnEntities(1);

        UpdateTextBox();
    }

    private void UpdateTextBox()
    {
        textBox.text = entitiesText + spawnedEntities.ToString();
    }

    private void OnDestroy()
    {
        store.Dispose();
    }
}

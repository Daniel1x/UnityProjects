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
    /// <summary>
    /// Prefab of unit gameobject.
    /// </summary>
    [SerializeField] private GameObject unitPrefab = null;
    /// <summary>
    /// Text field to display entity count.
    /// </summary>
    [SerializeField] private Text textBox = null;
    /// <summary>
    /// Number of entities spawned at once.
    /// </summary>
    [SerializeField] private int entitiesToSpawn = 250;
    /// <summary>
    /// LayerMask of floor gameobjects.
    /// </summary>
    [SerializeField] private LayerMask floorLayerMask = 0;

    /// <summary>
    /// Static entity manager reference.
    /// </summary>
    public static EntityManager entityManager;
    /// <summary>
    /// Unit prefab as an entity.
    /// </summary>
    public static Entity unitEntity;
    /// <summary>
    /// BlobAssetStore reference.
    /// </summary>
    private BlobAssetStore store;
    /// <summary>
    /// Constant text displayed with entities count.
    /// </summary>
    private const string ENTITIES_TEXT = "Entities: ";
    /// <summary>
    /// Entity query placeholder.
    /// </summary>
    private EntityQuery entityQuery;

    private void Start()
    {
        store = new BlobAssetStore();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, store);
        unitEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(unitPrefab, settings);
    }

    /// <summary>
    /// Instantiates unit entities.
    /// </summary>
    /// <param name="entitiesToSpawn">Number of entities to spawn</param>
    private void SpawnEntities(int entitiesToSpawn)
    {
        for(int i = 0; i < entitiesToSpawn; i++)
        {
            Entity entity = entityManager.Instantiate(unitEntity);
            float3 position = new float3(UnityEngine.Random.Range(-3f, 3f), 0f, UnityEngine.Random.Range(-3f, 3f));
            entityManager.SetComponentData(entity, new Translation { Value = position });
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnEntities(entitiesToSpawn);
        }
        SpawnEntities(6);

        UpdateTextBox();
        
        // TESTING BLOCKING CHOSEN WAYPOINTS
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10f, floorLayerMask))
            {
                int2 hitOnGridPosition = WaypointsManager.Func.GetGridPositionFromWorldPosition(WaypointsManager.Func.RoundToGrid(hit.point));
                int hitOnGridIndex = WaypointsManager.Func.CalculateIndex(hitOnGridPosition, WaypointsManager.GridSize.x);
                bool waypointState = !WaypointsManager.waypoints[hitOnGridIndex].isWalkable;
                WaypointsManager.waypoints[hitOnGridIndex].isWalkable = waypointState;
                WaypointsManager.waypointHasBeenModified = true;
                string state = waypointState ? " Unlocked" : " Blocked";

                Debug.Log("Clicked at: " + hit.point + ", Hitted grid at: " + hitOnGridPosition.ToString() + " with index: " + hitOnGridIndex.ToString() + state);
            }
        }
    }

    /// <summary>
    /// Updates the display of the number of units.
    /// </summary>
    private void UpdateTextBox()
    {
        entityQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<LifetimeData>());
        int numEntities = entityQuery.CalculateEntityCount();

        textBox.text = ENTITIES_TEXT + numEntities.ToString();
    }

    /// <summary>
    /// Clears the memory.
    /// </summary>
    private void OnDestroy()
    {
        store.Dispose();
    }
}
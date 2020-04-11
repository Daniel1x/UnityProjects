using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private bool debugInEditorEnabled = false;
    private void Update()
    {
        if (!debugInEditorEnabled) return;
        Debug.Log("RESET!");
        ResetMap();
    }

    private const float GRID_TILING = 0.03125f;
    private enum Side { Front, Right, Back, Left };

    [Header("Prefabs")]
    [SerializeField] private GameObject cubePrefab = null;
    [Header("Tags")]
    [SerializeField] private string floorTag = "Floor";
    [SerializeField] private string wallTag = "Wall";
    [Header("Materials")]
    [SerializeField] private Material floorMaterial = null;
    [SerializeField] private Material wallMaterial = null;
    [Header("Sizes")]
    [SerializeField] [Range(2, 100)] private int width = 3;
    [SerializeField] [Range(2, 100)] private int hight = 3;
    [SerializeField] [Range(2, 100)] private int length = 3;
    [SerializeField] private Vector3 spawnOffset = new Vector3();
    [Header("Entrances")]
    [SerializeField] bool hasFrontEntrance = false;
    [SerializeField] bool hasBackEntrance = false;
    [SerializeField] bool hasRightEntrance = false;
    [SerializeField] bool hasLeftEntrance = false;
    [SerializeField] [Range(0f, 10f)] private float entranceSize = 1f;

    private void CreateMap(float width, float hight, float length, Vector3 position, bool hasFrontEntrance, bool hasRightEntrance, bool hasBackEntrance, bool hasLeftEntrance, float entranceSize)
    {
        GameObject mapFragment = new GameObject("MapFragment") as GameObject;
        mapFragment.transform.parent = this.transform;

        CreateFloor(width, length, position, mapFragment.transform);
        CreateWall(Side.Front, width, hight, length, position, mapFragment.transform, hasFrontEntrance, entranceSize);
        CreateWall(Side.Right, width, hight, length, position, mapFragment.transform, hasRightEntrance, entranceSize);
        CreateWall(Side.Back, width, hight, length, position, mapFragment.transform, hasBackEntrance, entranceSize);
        CreateWall(Side.Left, width, hight, length, position, mapFragment.transform, hasLeftEntrance, entranceSize);
    }

    private void CreateWall(Side side, float width, float hight, float length, Vector3 floorPosition, Transform parent, bool hasEntrance = false, float entranceSize = 1f)
    {
        Vector3 wallPosition = CalculateWallPosition(side, width, hight, length, floorPosition);
        Quaternion wallRotation = CalculateWallRotation(side);

        GameObject wall = new GameObject();
        wall.transform.position = wallPosition;
        wall.transform.rotation = wallRotation;
        wall.transform.parent = parent;
        wall.name = side + "Wall";

        CreateWallGrid(wall, side, width, hight, length, hasEntrance, entranceSize);
    }

    private void CreateWallGrid(GameObject wallGO, Side side, float width, float hight, float length, bool hasEntrance = false, float entranceSize = 1f)
    {
        if (hasEntrance)
        {
            float halfEntranceSize = entranceSize / 2f;
            float halfWidth = width / 2f;
            float halfLength = length / 2f;
            float halfWallWidth = halfWidth - halfEntranceSize;
            float halfWallLength = halfLength - halfEntranceSize;

            GameObject wallRight;
            try
            {
                wallRight = Instantiate(cubePrefab, wallGO.transform);
            }
            catch
            {
                Debug.LogError("Cube prefab is NULL, find it!");
                return;
            }
            wallRight.name = "RightWall";
            wallRight.tag = wallTag;

            Vector3 wallRightLocalScale = CalculateWallScale(side, halfWallWidth, hight, halfWallLength);
            wallRightLocalScale = ClampVector(wallRightLocalScale, 0.25f, float.MaxValue);
            wallRight.transform.localScale = wallRightLocalScale;

            Vector3 wall1Position = Vector3.right * ((wallRightLocalScale.x / 2f) + halfEntranceSize);
            wallRight.transform.localPosition = wall1Position;
            
            GameObject wallLeft = Instantiate(cubePrefab, wallGO.transform);

            wallLeft.name = "LeftWall";
            wallLeft.tag = wallTag;

            Vector3 wallLeftLocalScale = CalculateWallScale(side, halfWallWidth, hight, halfWallLength);
            wallLeftLocalScale = ClampVector(wallLeftLocalScale, 0.25f, float.MaxValue);
            wallLeft.transform.localScale = wallLeftLocalScale;

            Vector3 wall2Position = Vector3.left * ((wallLeftLocalScale.x / 2f) + halfEntranceSize);
            
            wallLeft.transform.localPosition = wall2Position;
            wallLeft.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            Material wallMaterial;
            try
            {
                wallMaterial = new Material(this.wallMaterial);
            }
            catch
            {
                Debug.LogError("Wall Material is NULL, find it!");
                return;
            }
            wallMaterial.mainTextureScale = CalculateWallTextureScale(side, halfWallWidth, hight, halfWallLength);
            wallRight.GetComponent<MeshRenderer>().material = wallMaterial;
            wallLeft.GetComponent<MeshRenderer>().material = wallMaterial;
        }
        else
        {
            GameObject wall;
            try
            {
                wall = Instantiate(cubePrefab, wallGO.transform);
            }
            catch
            {
                Debug.LogError("Cube prefab is NULL, fint it!");
                return;
            }
            wall.name = side + "Wall";
            wall.transform.localScale = CalculateWallScale(side, width, hight, length);
            
            Material wallMaterial;
            try
            {
                wallMaterial = new Material(this.wallMaterial);
            }
            catch
            {
                Debug.LogError("Wall Material is NULL, find it!");
                return;
            }
            wallMaterial.mainTextureScale = CalculateWallTextureScale(side, width, hight, length);
            wall.GetComponent<MeshRenderer>().material = wallMaterial;
        }
    }

    private Vector3 ClampVector(Vector3 value, float minValue, float maxValue, bool clampX = true, bool clampY = false, bool clampZ = false)
    {
        Vector3 v = value;
        if (clampX) v.x = Mathf.Clamp(v.x, minValue, maxValue);
        if (clampY) v.y = Mathf.Clamp(v.y, minValue, maxValue);
        if (clampZ) v.z = Mathf.Clamp(v.z, minValue, maxValue);
        return v;
    }

    private Quaternion CalculateWallRotation(Side side)
    {
        return ChoseBySide(side, Quaternion.Euler(0f, 0f, 0f),
                                 Quaternion.Euler(0f, 90f, 0f),
                                 Quaternion.Euler(0f, 0f, 0f),
                                 Quaternion.Euler(0f, 90f, 0f));
    }

    private Vector2 CalculateWallTextureScale(Side side, float width, float hight, float length)
    {
        return ChoseBySide(side, new Vector2(width, hight) * GRID_TILING,
                                 new Vector2(length, hight) * GRID_TILING,
                                 new Vector2(width, hight) * GRID_TILING,
                                 new Vector2(length, hight) * GRID_TILING);
    }

    private Vector3 CalculateWallScale(Side side, float width, float hight, float length)
    {
        return ChoseBySide(side, new Vector3(width, hight, 1),
                                 new Vector3(length, hight, 1),
                                 new Vector3(width, hight, 1),
                                 new Vector3(length, hight, 1));
    }

    private Vector3 CalculateWallPosition(Side side, float width, float hight, float length, Vector3 floorPosition)
    {
        return ChoseBySide(side, floorPosition + new Vector3(0f, hight / 2f, -0.5f - (length / 2f)),
                                 floorPosition + new Vector3(0.5f + (width / 2f), hight / 2f, 0f),
                                 floorPosition + new Vector3(0f, hight / 2f, 0.5f + (length / 2f)),
                                 floorPosition + new Vector3(-0.5f - (width / 2f), hight / 2f, 0f));
    }

    private void CreateFloor(float floorWidth, float floorLength, Vector3 position, Transform parent)
    {
        float width = floorWidth + 2;
        float length = floorLength + 2;
        GameObject floor;
        try
        {
            floor = Instantiate(cubePrefab, position - (Vector3.up / 2f), Quaternion.identity, parent) as GameObject;
        }
        catch
        {
            Debug.LogError("Cube prefab is NULL, find it!");
            return;
        }
        floor.name = "Floor";
        floor.tag = floorTag;
        floor.transform.localScale = new Vector3(width, 1, length);
        Material floorMaterial;
        try
        {
            floorMaterial = new Material(this.floorMaterial);
        }
        catch
        {
            Debug.LogError("Floor Material is NULL, find it!");
            return;
        }
        Material material = floor.GetComponent<MeshRenderer>().material = floorMaterial;
        Vector2 tiling = material.mainTextureScale;
        material.mainTextureScale = new Vector2(width, length) * GRID_TILING;
    }
    
    public void CreateMap()
    {
        CreateMap(width, hight, length, transform.position + spawnOffset, hasFrontEntrance, hasRightEntrance, hasBackEntrance, hasLeftEntrance, entranceSize);
    }

    public void CreateRandomMapFragment()
    {
        CreateMap(UnityEngine.Random.Range(4, 50),
                  UnityEngine.Random.Range(1, 10),
                  UnityEngine.Random.Range(4, 50),
                  new Vector3(UnityEngine.Random.Range(-50, 50), 0f, UnityEngine.Random.Range(-50, 50)),
                  UnityEngine.Random.Range(0f, 1f) > 0.5f,
                  UnityEngine.Random.Range(0f, 1f) > 0.5f,
                  UnityEngine.Random.Range(0f, 1f) > 0.5f,
                  UnityEngine.Random.Range(0f, 1f) > 0.5f,
                  UnityEngine.Random.Range(0.5f, 3f));
    }

    private void DestroyAllChildrens(Transform parent, bool inEditorMode = false)
    {
        while (parent.childCount > 0)
        {
            foreach (Transform child in parent)
            {
                DestroyAllChildrens(child, inEditorMode);
                if (inEditorMode)
                {
                    DestroyImmediate(child.gameObject);
                }
                else
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    public void DestroyMap()
    {
        DestroyAllChildrens(transform, true);
    }

    public void ResetMap()
    {
        DestroyAllChildrens(transform, true);
        CreateMap(width, hight, length, transform.position + spawnOffset, hasFrontEntrance, hasRightEntrance, hasBackEntrance, hasLeftEntrance, entranceSize);
    }

    /// <summary>
    /// Return values based on chosen side.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="side">Chosen side</param>
    /// <param name="front">Returned on front side</param>
    /// <param name="right">Returned on front right</param>
    /// <param name="back">Returned on front back</param>
    /// <param name="left">Returned on front left</param>
    /// <returns></returns>
    private T ChoseBySide<T>(Side side, T front, T right, T back, T left)
    {
        T t;
        switch (side)
        {
            case Side.Front:
                t = front;
                break;
            case Side.Right:
                t = right;
                break;
            case Side.Back:
                t = back;
                break;
            case Side.Left:
                t = left;
                break;
            default:
                t = front;
                break;
        }
        return t;
    }
}

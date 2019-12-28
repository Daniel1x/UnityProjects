using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] List<GameObject> blocks = null;
    [SerializeField] int numberOfBlocks = 10;
    [SerializeField] float timeToNextGeneration = 0.25f;

    float minX = 1f;
    float maxX = 32f;
    float minY = 6.0f;
    float maxY = 24f;
    float timer = 0;
    
    private void StartGeneration()
    {
        for(int i = 0; i < numberOfBlocks; i++)
        {
            float x, y;
            x = (int)Random.Range(minX, maxX);
            y = (int)Random.Range(minY, maxY);
            Vector2 pos = new Vector2(x/2, y/2);
            int block = (int)Random.Range(0.0f, blocks.Capacity);
            GameObject clone = Instantiate(blocks[block], pos, Quaternion.identity, transform);
            clone.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
        }
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            if (Input.GetKey(KeyCode.G))
            {
                StartGeneration();
                timer = timeToNextGeneration;
            }
        }
    }
}

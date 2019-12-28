using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Block : MonoBehaviour
{
    [SerializeField] AudioClip audioClip = null;
    [SerializeField] GameObject explosionVFX = null;
    [SerializeField] int maxHits = 1;
    [SerializeField] Sprite[] hitSprites = null;

    int timesHit = 0;
    Level level = null;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        DestroyBreakableBlock();
    }

    private void DestroyBreakableBlock()
    {
        if (tag == "Breakable")
        {
            timesHit++;
            AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position);
            GameObject spark = Instantiate(explosionVFX, transform.position, transform.rotation);
            Destroy(spark, 2.0f);
            level.AddPoints();
            if(timesHit >= maxHits)
            {
                level.SubtractOneBlock();
                Destroy(gameObject);
            }
            else
            {
                ShowNextHitSprite();
            }
        }
    }

    private void ShowNextHitSprite()
    {
        int spriteIndex = 0;
        if (timesHit >= (spriteIndex + 1) * maxHits / hitSprites.Length) 
        {
            spriteIndex++;
            if (hitSprites[spriteIndex] != null)
            {
                if (timesHit == maxHits - 1)
                {
                    GetComponent<SpriteRenderer>().sprite = hitSprites[hitSprites.Length - 1];
                }
                else
                {
                    GetComponent<SpriteRenderer>().sprite = hitSprites[spriteIndex];
                }
            }
            else
            {
                Debug.Log("Sprite is missing! " + gameObject.name);
            }
        }
    }

    private void Start()
    {
        CountBreakableBlocks();
    }

    private void CountBreakableBlocks()
    {
        level = FindObjectOfType<Level>();
        if (tag == "Breakable")
        {
            level.CountBreakableBlocks();
        }
    }
}

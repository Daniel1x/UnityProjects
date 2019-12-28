using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Level : MonoBehaviour
{
    [SerializeField] public int breakableBlocks = 0;
    [SerializeField] public int playableBalls = 0;
    [Range(0.1f,10.0f)][SerializeField] public float timeSpeed = 1f;
    [SerializeField] public int playerScore = 0;
    [SerializeField] TextMeshProUGUI scoreBox = null;
    [SerializeField] bool isAutoPlayEnabled = false;

    float timer = 0;
    int pointsPerBlock = 10;
    Level level = null;

    private void Awake()
    {
        int gameStatusCount = FindObjectsOfType<Level>().Length;
        if (gameStatusCount > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        level = FindObjectOfType<Level>();
    }

    public void CountBreakableBlocks()
    {
        breakableBlocks++;
    }

    public void SubtractOneBlock()
    {
        breakableBlocks--;
        if (breakableBlocks <= 0)
        {
            CheckForWin();
        }
    }

    public void CountPlayableBalls()
    {
        playableBalls++;
    }

    public void SubtractOneBall()
    {
        playableBalls--;
    }

    public void AddPoints()
    {
        playerScore += pointsPerBlock;
        scoreBox.text = playerScore.ToString();
    }

    public void ClearPoints()
    {
        playerScore = 0;
        scoreBox.text = 0.ToString();
    }

    public void ClearCounting()
    {
        playableBalls = 0;
        breakableBlocks = 0;
    }

    private void Update()
    {
        Time.timeScale = timeSpeed;

        timer += Time.deltaTime;
        if (timer > 1f)
        {
            timer = 0;
            CheckForWin();
        }
    }

    public bool IsAutoPlayEnabled()
    {
        return isAutoPlayEnabled;
    }

    public void ChangeAutoPlayStatus()
    {
        isAutoPlayEnabled = !isAutoPlayEnabled;
    }

    public void SetTimeSpeed(float tSpeed)
    {
        timeSpeed = tSpeed;
    }

    public void CheckForWin()
    {
        int startSceneID = 0;
        int sceneID = SceneManager.GetActiveScene().buildIndex;
        int scenesInBuild = SceneManager.sceneCountInBuildSettings;
        if (sceneID != startSceneID && sceneID != scenesInBuild && sceneID != scenesInBuild - 1)
        {
            if (breakableBlocks <= 0 && CheckForBlocks())
            {
                playableBalls = 0;
                if (breakableBlocks <= 0 && sceneID == scenesInBuild - 3)
                {
                    SceneManager.LoadScene(scenesInBuild - 1);
                }
                else
                {
                    SceneManager.LoadScene(sceneID + 1);
                }
            }
        }
    }

    private bool CheckForBlocks()
    {
        Block[] blocks = FindObjectsOfType<Block>();
        foreach (Block block in blocks)
        {
            if(block.tag == "Breakable")
            {
                return false;
            }
        }
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    public GameObject botPrefab;
    public int populationSize = 50;
    List<GameObject> population = new List<GameObject>();
    public static float elapsed = 0;
    public float trialTime = 5f;
    [Range(0.5f,10f)] public float timeScale = 5f;
    int generation = 1;
    public bool loadFromFile = true;

    GUIStyle guiStyle = new GUIStyle();
    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats ", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}",elapsed), guiStyle);
        GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
        GUI.EndGroup();
    }

    private void Start()
    {
        Time.timeScale = timeScale;

        for(int i = 0; i < populationSize; i++)
        {
            Vector3 startingPos = new Vector3(transform.position.x /*+ Random.Range(-3, 3)*/,
                                              transform.position.y,
                                              transform.position.z /*+ Random.Range(-3, 3)*/);

            GameObject b = Instantiate(botPrefab, startingPos, transform.rotation);
            b.GetComponent<Brain>().Init();
            if (loadFromFile) b.GetComponent<Brain>().dna.LoadGenesFromFile("1");
            population.Add(b);
        }
    }

    GameObject Recreate(GameObject parent1, GameObject parent2, bool randomly = false)
    {
        Vector3 startingPos = new Vector3(transform.position.x /*+ Random.Range(-3, 3)*/,
                                          transform.position.y,
                                          transform.position.z /*+ Random.Range(-3, 3)*/);
        GameObject child = Instantiate(botPrefab, startingPos, transform.rotation);
        Brain b = child.GetComponent<Brain>();
        if (Random.Range(0f, 100f) < 25f) //mutate 25 in 100
        {
            if (Random.Range(0f, 100f) < 5f)
            {
                b.Init();
                b.dna.MutateRandom();
            }
            else
            {
                b.Init();
                b.dna.MutateOne();
            }
        }
        else
        {
            b.Init();
            if (randomly)
            {
                b.dna.CombineRandomly(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);
            }
            else
            {
                b.dna.Combine(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);
            }
        }
        return child;
    }

    void RecreateNewPopulation()
    {
        //List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<Brain>().timeAlive).ToList();
        List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<Brain>().distanceTravelled).ToList();
        sortedList[sortedList.Count - 1].GetComponent<Brain>().dna.SaveGenesToFile("1");
        population.Clear();
        /*for (int i = (int)(sortedList.Count / 2.0f) - 1; i < sortedList.Count - 1; i++)
        {
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i + 1], sortedList[i]));
        }*/

        for (int i = (int)(sortedList.Count * 0.9) - 1; i < sortedList.Count - 1; i++)
        {
            population.Add(Recreate(sortedList[i], sortedList[i + 1]));
            population.Add(Recreate(sortedList[i + 1], sortedList[i]));
            population.Add(Recreate(sortedList[i], sortedList[i + 1], true));
            population.Add(Recreate(sortedList[i + 1], sortedList[i], true));
            population.Add(Recreate(sortedList[i + 1], sortedList[i], true));
            population.Add(Recreate(sortedList[i], sortedList[i + 1]));
            population.Add(Recreate(sortedList[i + 1], sortedList[i]));
            population.Add(Recreate(sortedList[i], sortedList[i + 1], true));
            population.Add(Recreate(sortedList[i + 1], sortedList[i], true));
            population.Add(Recreate(sortedList[i + 1], sortedList[i], true));
        }

        for(int i = 0; i < sortedList.Count; i++)
        {
            Destroy(sortedList[i]);
        }
        generation++;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= trialTime)
        {
            RecreateNewPopulation();
            elapsed = 0;
        }
        
        Time.timeScale = timeScale;
    }

}

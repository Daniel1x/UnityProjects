using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DNA
{
    public List<int> genes = new List<int>();
    private int dnaLength = 0;
    private int maxValues = 0;
    
    public DNA(int l,int v)
    {
        dnaLength = l;
        maxValues = v;
        SetRandom();
    }
    
    public void SetRandom()
    {
        genes.Clear();
        for(int i = 0; i < dnaLength; i++)
        {
            genes.Add(Random.Range(0, maxValues));
        }
    }

    public void SetInt(int pos,int value)
    {
        genes[pos] = value;
    }

    public void Combine(DNA d1, DNA d2)
    {
        for (int i = 0; i < dnaLength; i++)
        {
            if (i < dnaLength / 2f)
            {
                genes[i] = d1.genes[i];
            }
            else
            {
                genes[i] = d2.genes[i];
            }
        }
    }

    public void CombineRandomly(DNA d1, DNA d2)
    {
        for (int i = 0; i < dnaLength; i++)
        {
            if (Random.Range(0, 2) == 0)
            {
                genes[i] = d1.genes[i];
            }
            else
            {
                genes[i] = d2.genes[i];
            }
        }
    }

    public void MutateOne()
    {
        genes[Random.Range(0, dnaLength)] = Random.Range(0, maxValues);
    }

    public void MutateRandom()
    {
        for (int i = 0; i < Random.Range(0, dnaLength); i++)
        {
            genes[Random.Range(0, dnaLength)] = Random.Range(0, maxValues);
        }
    }

    public int GetGene(int pos)
    {
        return genes[pos];
    }

    private string GenesToString()
    {
        string data = "" + genes[0].ToString();
        for(int i = 1; i < dnaLength; i++)
        {
            data += ";" + genes[i].ToString();
        }
        return data;
    }

    private void StringToGenes(string data)
    {
        if (data == "") return;
        string[] values = data.Split(';');
        for(int i = 0; i < dnaLength; i++)
        {
            genes[i] = int.Parse(values[i]);
        }
    }

    public void SaveGenesToFile(string filename = "")
    {
        string path = Application.dataPath + "/genes" + filename + ".txt";
        StreamWriter wf = File.CreateText(path);
        wf.WriteLine(GenesToString());
        wf.Close();
    }

    public void LoadGenesFromFile(string filename = "")
    {
        string path = Application.dataPath + "/genes" + filename + ".txt";
        StreamReader wf = File.OpenText(path);

        if (File.Exists(path))
        {
            string line = wf.ReadLine();
            StringToGenes(line);
        }
    }
}

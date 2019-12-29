using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using System.Linq;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class Brain : MonoBehaviour
{
    public int DNALength = 243;
    public float timeAlive;
    public float distanceTravelled;
    public float points = 0;
    Vector3 startPosition;
    public DNA dna;
    public List<int> genes = new List<int>();

    private ThirdPersonCharacter charakter;
    private Vector3 moveV;
    public bool alive = true;
    bool jump;
    bool crouch;
    float h;
    float v;

    public GameObject eyes;
    private enum visibleObject { map, obstacle, dead };
    private visibleObject lookForward;
    private visibleObject lookRigth;
    private visibleObject lookLeft;
    private visibleObject lookBack;
    private visibleObject lookDown;
    Vector3 look = new Vector3(0f, -1f, 4f);
    int[] dataSet = new int[5];
    const int posibilities = 243;
    List<Combination> combinations = new List<Combination>();
    public int activeGene = 0;
    public SkinnedMeshRenderer body;

    List<GameObject> bonuses = new List<GameObject>();

    private void Start()
    {
        SetUpCombinations();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "dead")
        {
            alive = false;
            body.enabled = false;
        }
        if (collision.gameObject.tag == "bonus")
        {
            bool exist = false;
            if (bonuses != null)
            {
                foreach (GameObject go in bonuses)
                {
                    if (go == collision.gameObject)
                    {
                        exist = true;
                    }
                }
            }
            if (!exist)
            {
                points += 1;
                bonuses.Add(collision.gameObject);
            }
        }
    }

    public void Init()
    {
        //initialise DNA
        //0 forward
        //1 back
        //2 left
        //3 rigth
        //4 jump
        //5 crouch
        dna = new DNA(DNALength, 5);
        charakter = GetComponent<ThirdPersonCharacter>();
        timeAlive = 0;
        alive = true;
        startPosition = transform.position;
        //Added
        genes = dna.genes;
        SetUpCombinations();
    }

    private void FixedUpdate()
    {
        if (!alive) return;
        h = 0;
        v = 0;
        crouch = false;

        SetUpSight();
        DecideWhatToDo();

        moveV = v * Vector3.forward + h * Vector3.right;
        charakter.Move(moveV, crouch, jump);
        jump = false;
        if (alive)
        {
            timeAlive += Time.deltaTime;
            distanceTravelled = Vector3.Distance(transform.position, startPosition);
        }
    }
    
    private void DecideWhatToDo()
    {
        Combination dataSet = new Combination();
        dataSet.states.Clear();
        dataSet.states.Add((int)lookForward);
        dataSet.states.Add((int)lookRigth);
        dataSet.states.Add((int)lookLeft);
        dataSet.states.Add((int)lookBack);
        dataSet.states.Add((int)lookDown);

        activeGene = FindIndexOfCombination(dataSet);

        DoAction(activeGene);
    }

    private void DoAction(int geneID)
    {
        if (dna.GetGene(geneID) == 0) v = 1;
        else if (dna.GetGene(geneID) == 1) v = -1;
        else if (dna.GetGene(geneID) == 2) h = -1;
        else if (dna.GetGene(geneID) == 3) h = 1;
        else if (dna.GetGene(geneID) == 4) jump = true;
        else if (dna.GetGene(geneID) == 5) v = 1;
        //else if (dna.GetGene(geneID) == 5) crouch = true;
    }

    private void SetUpCombinations()
    {
        int combinationID = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    for (int l = 0; l < 3; l++)
                    {
                        for (int m = 0; m < 3; m++)
                        {
                            Combination cb = new Combination();
                            cb.states.Add(i);
                            cb.states.Add(j);
                            cb.states.Add(k);
                            cb.states.Add(l);
                            cb.states.Add(m);
                            combinationID++;
                            if (!combinations.Contains(cb))
                                combinations.Add(cb);
                        }
                    }
                }
            }
        }

    }

    private void SetUpSight()
    {
        look =  /*transform.rotation*/ (Quaternion.Euler(68f * Vector3.right) * Vector3.forward * 50f);

        RaycastHit hit;
        if (Physics.Raycast(eyes.transform.position, Quaternion.Euler(0f, 0f, 0f) * look, out hit))
        {
            lookForward = Hitted(hit);
        }
        if (Physics.Raycast(eyes.transform.position, Quaternion.Euler(0f, 90f, 0f) * look, out hit))
        {
            lookRigth = Hitted(hit);
        }
        if (Physics.Raycast(eyes.transform.position, Quaternion.Euler(0f, -90f, 0f) * look, out hit))
        {
            lookLeft = Hitted(hit);
        }
        if (Physics.Raycast(eyes.transform.position, Quaternion.Euler(0f, 180f, 0f) * look, out hit))
        {
            lookBack = Hitted(hit);
        }
        if (Physics.Raycast(eyes.transform.position, -transform.up * 50f, out hit)) 
        {
            lookDown = Hitted(hit);
        }
    }

    private visibleObject Hitted(RaycastHit hit)
    {
        if (hit.collider.gameObject.tag == "map")
        {
            return visibleObject.map;
        }
        else if (hit.collider.gameObject.tag == "obsticle")
        {
            return visibleObject.obstacle;
        }
        else if (hit.collider.gameObject.tag == "dead")
        {
            return visibleObject.dead;
        }
        else if (hit.collider.gameObject.tag == "bonus")
        {
            return visibleObject.map;
        }
        else
        {
            Debug.Log("Wrong combination!");
            return visibleObject.dead;
        }
    }

    private int FindIndexOfCombination(Combination dataSet)
    {
        int id = 0;
        while (combinations[id].states[0] != dataSet.states[0] || 
               combinations[id].states[1] != dataSet.states[1] || 
               combinations[id].states[2] != dataSet.states[2] || 
               combinations[id].states[3] != dataSet.states[3] || 
               combinations[id].states[4] != dataSet.states[4])
        {
            id++;
        }
        return id;
    }
}

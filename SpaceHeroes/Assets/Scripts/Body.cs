using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Rigidbody))]

public class Body : MonoBehaviour
{
    private readonly float Gravity = 2000.0f;   //Sila grawitacji
    public bool PullOrPush = true;              //Przyciagaj lub odpychaj
    public static List<Body> Bodies;            //Lista obiektow
    public Rigidbody ThisRb { get; set; }           //Zmienna Rigidbody
    public bool Playing = false;
    public GameObject Scraps;
    public GameObject Bonus;
    public float health = 50.0f;
    private float BonusPropability = 0.5f;

    void Start()
    {
        if (Bodies == null)                     //Sprawdzanie czy jest lista obiektow
            Bodies = new List<Body>();          //Gdy nie to tworzenie nowej listy obiektow

        Bodies.Add(this);

        ThisRb = GetComponent<Rigidbody>();

        foreach (Body body in Bodies)           //Dla każdego obiektu podatnego na przyciaganie
        {
            if (body == this)                   //Przypisanie aktualnego obiektu do zmiennej
                continue;
                                                //Dodanie sily do obiektow o losowych kierunkach i mocy
            Vector3 forceKick =ThisRb.mass* UnityEngine.Random.value* new Vector3(body.transform.position.x * ((2 * UnityEngine.Random.value) - 1), body.transform.position.y * ((2 * UnityEngine.Random.value) - 1), body.transform.position.z * ((2 * UnityEngine.Random.value) - 1));
            if (PullOrPush) ThisRb.AddForce(forceKick); //Gdy przyciaga dodaje sile dodatnia
            else ThisRb.AddForce(-forceKick);           //Gdy odpycha dodaje sile ujemna
        }
        Playing = true;
    }
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) PullOrPush = !PullOrPush;  //Zmiana kierunku sily grawitacji klawiszem "O"
        if (Playing==false)
        {
            RestartTheGame();
            Playing = true;
        }
        foreach (Body body in Bodies)           //Dla każdego obiektu podatnego na przyciaganie
        {
            if (body == null)
                break;
            if (body == this)                   //Przypisanie aktualnego obiektu do zmiennej
            {
                continue;
            }

            float mass1 = ThisRb.mass;              //Przypisanie mas obiektow
            float mass2 = body.ThisRb.mass;         

            float distance = Vector3.Distance(transform.position, body.transform.position);    //Obliczanie odleglosci miedzy obiektami

            float Force_amp = (mass1 * mass2) / Mathf.Pow(distance, 2);    //Obliczanie sily oddzialywania na siebie dwoch cial
            Force_amp *= Gravity;

            Vector3 direction = Vector3.Normalize(body.transform.position - transform.position);  //Obliczanie kierunku oddzialywania

            Vector3 F = (direction * Force_amp) * Time.fixedDeltaTime;     //Skalowanie sily z uwzglednieniem czasu
            
            if (PullOrPush) ThisRb.AddForce(F);     //Dodawanie sily w kierunku ustawionym klawiszem "O"
            else ThisRb.AddForce(-F);
            if (Input.GetKey(KeyCode.P))        //Gdy wcisniety klawisz "P"
            {
                Explode();                      //Dodawanie sily wybuchu
            }
        }

        Quit();
    }

    private void Quit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Bodies.Remove(this);
        }
    }

    private void Explode()      //Funkcja wybuchu
    {
        float power = 50.0f;    //Wartosc sily
                                //Tworzenie wektora o losowych kierunku skalowanego za pomoca watosci sily
        Vector3 kick = new Vector3(((2 * UnityEngine.Random.value) - 1), ((2 * UnityEngine.Random.value) - 1), ((2 * UnityEngine.Random.value) - 1)) * power;
                                //Dodawanie sily wybuchu o losowej wartosci mocy oraz promieniu i pozycji obiektu
        ThisRb.AddExplosionForce(1000.0f * ((2 * UnityEngine.Random.value) - 1), ThisRb.transform.position, 200.0f * UnityEngine.Random.value);
        ThisRb.AddTorque(kick);
        ThisRb.AddForce(kick,ForceMode.Impulse);
    }
    
    private void RestartTheGame()
    {
        if (Bodies == null)                     //Sprawdzanie czy jest lista obiektow
            Bodies = new List<Body>();          //Gdy nie to tworzenie nowej listy obiektow

        Bodies.Add(this);

        ThisRb = GetComponent<Rigidbody>();

        foreach (Body body in Bodies)           //Dla każdego obiektu podatnego na przyciaganie
        {
            if (body == this)                   //Przypisanie aktualnego obiektu do zmiennej
                continue;
            //Dodanie sily do obiektow o losowych kierunkach i mocy
            Vector3 forceKick = ThisRb.mass * UnityEngine.Random.value * new Vector3(body.transform.position.x * ((2 * UnityEngine.Random.value) - 1), body.transform.position.y * ((2 * UnityEngine.Random.value) - 1), body.transform.position.z * ((2 * UnityEngine.Random.value) - 1));
            if (PullOrPush) ThisRb.AddForce(forceKick); //Gdy przyciaga dodaje sile dodatnia
            else ThisRb.AddForce(-forceKick);           //Gdy odpycha dodaje sile ujemna
        }
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Instantiate(Scraps, transform.position, transform.rotation);
        if (UnityEngine.Random.value < BonusPropability)
        {
            Instantiate(Bonus, transform.position, transform.rotation);
        }
        Bodies.Remove(this);
        Destroy(gameObject);
    }

}

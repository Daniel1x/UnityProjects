using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarState : MonoBehaviour
{
    // Zmienna informująca o stanie pojazdu.
    public bool crashed = false;
    // Funkcja uruchamiana przez silnik fizyczny w momencie wykrycia kolizji.
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.tag);
        // Zmiana stanu auta w momencie kolizji i obiektem oznaczonym jako plansza.
        if (collision.gameObject.tag == "Map")
        {
            crashed = true;
        }
    }
}

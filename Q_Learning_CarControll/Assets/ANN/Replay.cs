using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Struktura danych do przechowywania "wspomnień", czyli listy stanów wejść oraz związanej z nimi nagrody.
public class Replay
{
    // Lista stanów.
    public List<float> states;
    // Wartość nagrody.
    public float reward;

    // Konstruktor, zawierający dane wejściowe oraz wartość nagrody.
    public Replay(float xRot, float ballZ, float ballVX, float ballY, float r)
    {
        states = new List<float>();
        states.Add(xRot);
        states.Add(ballZ);
        states.Add(ballVX);
        states.Add(ballY);
        reward = r;
    }

    // Konstruktor, zawierający dane wejściowe oraz wartość nagrody.
    public Replay(float xRot, float zRot, float xBallPos, float yBallPos, float zBallPos, float xBallVel, float yBallVel, float zBallVel, float r)
    {
        states = new List<float>();
        states.Add(xRot);
        states.Add(zRot);
        states.Add(xBallPos);
        states.Add(yBallPos);
        states.Add(zBallPos);
        states.Add(xBallVel);
        states.Add(yBallVel);
        states.Add(zBallVel);
        reward = r;
    }

    // Konstruktor, zawierający dane wejściowe oraz wartość nagrody. Dla auta.
    public Replay(float forward, float right, float right45, float left, float left45, float r)
    {
        states = new List<float>();
        states.Add(forward);
        states.Add(right);
        states.Add(right45);
        states.Add(left);
        states.Add(left45);
        reward = r;
    }
}

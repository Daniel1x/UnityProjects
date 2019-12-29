using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Struktura danych do przechowywania "wspomnień", czyli listy stanów wejść oraz związanej z nimi nagrody.
public class Replay
{
    // Lista stanów.
    public List<double> states;
    // Wartość nagrody.
    public double reward;

    // Konstruktor, zawierający dane wejściowe oraz wartość nagrody.
    public Replay(double xRot, double ballZ, double ballVX, double ballY, double r)
    {
        states = new List<double>();
        states.Add(xRot);
        states.Add(ballZ);
        states.Add(ballVX);
        states.Add(ballY);
        reward = r;
    }

    // Konstruktor, zawierający dane wejściowe oraz wartość nagrody.
    public Replay(double xRot, double zRot, double xBallPos, double yBallPos, double zBallPos, double xBallVel, double yBallVel, double zBallVel, double r)
    {
        states = new List<double>();
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
}

using UnityEngine;
using System;

public class ActivationFunction
{
    // Funkcja zaokrąglająca wartość do danej liczby miejsc po przecinku.
    public static double RoundValue(double value, int decimalNumbers)
    {
        double scaleFactor = Mathf.Pow(10f, decimalNumbers);
        return ((int)(value * scaleFactor)) / scaleFactor;
    }

    // Funkcja skokowa.
    public static double Step(double value)
    {
        return (value < 0) ? 0 : 1;
    }

    // Funkcja sigmoidalna.
    public static double Sigmoid(double value)
    {
        double val = Math.Exp(value);
        return val / (1f + val);
    }

    // Funkcja Tanh.
    public static double TanH(double value)
    {
        return (2 * Sigmoid(value) - 1);
    }

    //Funkcja ReLu, dla dodatnich liniowa a dla reszty równa 0.
    public static double ReLu(double value)
    {
        return (value > 0) ? value : 0;
    }

    //Funkcja LeakyReLu, dla ujemnych spłaszczona 100 krotnie natomiast dla dodatnich liniowa.
    public static double LeakyReLu(double value)
    {
        return (value < 0) ? 0.01 * value : value;
    }

    //Funkcja sinusoidalna.
    public static double Sinusoid(double value)
    {
        return Mathf.Sin((float)value);
    }

    //Funkcja arcus tangens.
    public static double ArcTan(double value)
    {
        return Mathf.Atan((float)value);
    }

    //Funkcja SoftSign.
    public static double SoftSign(double value)
    {
        return value / (1 + Mathf.Abs((float)value));
    }
}

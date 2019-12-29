using UnityEngine;
using System;
using System.Linq;

public static class ActivationFunction
{
    // Funkcja zaokrąglająca wartość do danej liczby miejsc po przecinku.
    public static float RoundValue(float value, int decimalNumbers)
    {
        float scaleFactor = Mathf.Pow(10f, decimalNumbers);
        return ((int)(value * scaleFactor)) / scaleFactor;
    }

    // Funkcja zaokrąglająca.
    public static float Round(float x)
    {
        return (float)Math.Round(x, MidpointRounding.AwayFromZero) / 2.0f;
    }

    // Funkcja skokowa.
    public static float Step(float value)
    {
        return (value < 0) ? 0 : 1;
    }

    // Funkcja sigmoidalna.
    public static float Sigmoid(float value)
    {
        float val = (float)Math.Exp(value);
        return val / (1f + val);
    }

    // Pochodna funkcji sigmoidalnej
    public static float SigmoidDerivative(float value)
    {
        float exp = Mathf.Exp(-value);
        return exp / Mathf.Pow(exp + 1, 2);
    }

    // Funkcja Tanh.
    public static float TanH(float value)
    {
        return (2 * Sigmoid(value) - 1);
    }

    //Funkcja ReLu, dla dodatnich liniowa a dla reszty równa 0.
    public static float ReLu(float value)
    {
        return (value > 0) ? value : 0;
    }

    //Funkcja LeakyReLu, dla ujemnych spłaszczona 100 krotnie natomiast dla dodatnich liniowa.
    public static float LeakyReLu(float value)
    {
        return (value < 0) ? 0.01f * value : value;
    }

    //Funkcja sinusoidalna.
    public static float Sinusoid(float value)
    {
        return Mathf.Sin(value);
    }

    //Funkcja arcus tangens.
    public static float ArcTan(float value)
    {
        return Mathf.Atan(value);
    }

    //Funkcja SoftSign.
    public static float SoftSign(float value)
    {
        return value / (1 + Mathf.Abs(value));
    }

    //Funkcja Mapująca zakres zmiennej
    public static float Map(float newfrom, float newto, float origfrom, float origto, float value)
    {
        if (value <= origfrom)
            return newfrom;
        else if (value >= origto)
            return newto;
        return (newto - newfrom) * ((value - origfrom) / (origto - origfrom)) + newfrom;
    }
}

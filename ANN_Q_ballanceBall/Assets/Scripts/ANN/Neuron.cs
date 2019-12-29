using System.Collections.Generic;

public class Neuron
{
    // Liczba wejść neuronu.
    public int numberOfInputs;

    // Liczba dodawana do wyjścia neuronu (offset) wyrównująca straty.
    public double bias;

    // Wartość wyjściowa neuronu.
    public double output;

    // Gradient błędu wartości wyjściowej.
    public double errorGradient;
    
    // Lista wag dla danych wejściowych, tworzona na podstawie ich ilości.
    public List<double> weights = new List<double>();

    // Lista danych wejściowych.
    public List<double> inputs = new List<double>();

    // Konstruktor neuronu.
    public Neuron(int numOfInputs)
    {
        // Przypisywanie losowej wartości z przedziału <-1, 1>.
        bias = UnityEngine.Random.Range(-1f, 1f);

        // Przypisanie liczby danych wejściowych.
        numberOfInputs = numOfInputs;

        // Przypisywanie losowej wartości wag z przedziału <-1, 1>, dla każdego z wejść.
        for (int i = 0; i < numOfInputs; i++)
            weights.Add(UnityEngine.Random.Range(-1f, 1f));
    }
}

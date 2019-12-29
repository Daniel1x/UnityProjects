using System.Collections.Generic;

public class Layer
{
    // Liczba neuronów w warstwie.
    public int numberOfNeurons;

    // Lista utworzonych neuronów.
    public List<Neuron> neurons = new List<Neuron>();

    // Konstruktor warstwy neuronów.
    public Layer(int numOfNeurons,int numOfNeuronInputs)
    {
        // Przypisywanie liczby neuronów w warstwie.
        numberOfNeurons = numOfNeurons;

        // Tworzenie warstwy neuronów.
        for (int i = 0; i < numOfNeurons; i++)
            neurons.Add(new Neuron(numOfNeuronInputs));
    }
}

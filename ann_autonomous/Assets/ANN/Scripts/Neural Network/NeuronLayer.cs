using System.Collections.Generic;

//Contains a list of neurons: represents a layer
class NeuronLayer
{
    /// <summary>
    /// Number of neurons in layer
    /// </summary>
    public int NumNeurons
    {
        get; private set;
    }

    /// <summary>
    /// List of neurons in layer
    /// </summary>
    public List<Neuron> Neurons
    {
        get; private set;
    }

    public NeuronLayer(int numOfNeurons, int numOfInput)
    {
        this.NumNeurons = numOfNeurons;
        Neurons = new List<Neuron>();

        //Adds neurons to neuron list
        for (int i = 0; i < numOfNeurons; ++i)
        {
            Neurons.Add(new Neuron(numOfInput));
        }
    }
}


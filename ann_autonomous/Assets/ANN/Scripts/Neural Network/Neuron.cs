using System.Collections.Generic;

/// <summary>
/// Basic structure of ANN: the building blocks.
/// </summary>
struct Neuron
{
    /// <summary>
    /// Number of inputs into the neural network
    /// </summary>
    public int NumInputs
    {
        get; private set;
    }

    /// <summary>
    /// Weight of each input: determines activity of network
    /// </summary>
    public List<float> Weight
    {
        get; private set;
    }

    public Neuron(int numOfInput)
    {
        //Extra input for threshold - so it can be evolved with GA
        this.NumInputs = numOfInput + 1;

        Weight = new List<float>();

        //Initialise random weights for each input
        for (int i = 0; i < this.NumInputs; i++)
        {
            Weight.Add(Utilities.instance.RandomMinMax(-1, 1));
        }
    }
}



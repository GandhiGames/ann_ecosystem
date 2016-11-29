using System;
using System.Collections.Generic;

/// <summary>
/// Contains a list of layers: represents the whole neural net
/// </summary>
public class NeuralNet
{
    private int _numOfInput; //Number of inputs for each neuron
    public int numOfInput { get { return _numOfInput; } }

    private int _numOfOutput; //Number of outputs of each neuron
    public int numOfOutput { get { return _numOfOutput; } }

    private int _numHiddenLayers; //Number of hidden layers
    public int numOfHiddenLayers { get { return _numHiddenLayers; } }

    private int _numOfNeuronsPerHiddenLayer; //Number of neurons per hidden layer
    public int numOfNeuronsPerHiddenLayer { get { return _numOfNeuronsPerHiddenLayer; } }

    List<NeuronLayer> layers; //List containing layers

    public NeuralNet(int numOfInput, int numOfOutput, int numOfHiddenLayers, int numOfNeuronsPerHiddenLayer)
    {
        _numOfInput = numOfInput;
        _numOfOutput = numOfOutput;
        _numHiddenLayers = numOfHiddenLayers;
        _numOfNeuronsPerHiddenLayer = numOfNeuronsPerHiddenLayer;

        layers = new List<NeuronLayer>();

        CreateNeuralNet();

    }

    public void CreateNeuralNet()
    {
        //If there are hidden layers
        if (_numHiddenLayers > 0)
        {
            //Create first layer
            layers.Add(new NeuronLayer(_numOfNeuronsPerHiddenLayer, _numOfInput));

            //Create any other subsequent hidden layers
            for (int i = 0; i < _numHiddenLayers - 1; i++)
            {
                //Input from first hidden layer
                layers.Add(new NeuronLayer(_numOfNeuronsPerHiddenLayer,
                    _numOfNeuronsPerHiddenLayer));
            }

            //Output layer
            //Input from subsequent or first hidden layer
            layers.Add(new NeuronLayer(_numOfOutput, _numOfNeuronsPerHiddenLayer));
        }
        else
        { //If no hidden layers
          //Input layer
            layers.Add(new NeuronLayer(_numOfOutput, _numOfInput));
        }


    }

    //Receives input and returns output: performs caluclations for neural net
    public List<float> Update(params float[] inputs)
    {
        List<float> inputList = new List<float>();
        inputList.AddRange(inputs);

        //Output from each layer
        List<float> outputs = new List<float>();

        int weightCount = 0;

        //Return empty if not corrent number of inputs
        if (inputList.Count != _numOfInput)
        {
            Console.WriteLine("NeuralNet|Update|Size of inputs list not equal number of inputs");
            return outputs;
        }

        //Each layer
        for (int i = 0; i < _numHiddenLayers + 1; i++)
        {
            if (i > 0)
            {
                //Clear input and add output from previous layer
                inputList.Clear();
                inputList.AddRange(outputs);
            }

            outputs.Clear();

            weightCount = 0;

            for (int j = 0; j < layers[i].NumNeurons; ++j)
            {
                float netInput = 0.0f;

                int NumInputs = layers[i].Neurons[j].NumInputs;

                //Each weight
                for (int k = 0; k < NumInputs - 1; ++k)
                {
                    //Sum the weights x inputs
                    netInput += layers[i].Neurons[j].Weight[k] *
                        inputList[weightCount++];
                }

                //Add in the bias
                netInput += layers[i].Neurons[j].Weight[NumInputs - 1] *
                    Utilities.instance.bias;

                //Store result in output
                outputs.Add(Sigmoid(netInput));

                weightCount = 0;
            }
        }

        return outputs;
    }

    //Gets weights from network
    public List<float> GetWeights()
    {
        //Temporarily store wights
        List<float> weights = new List<float>();

        //Each layer
        for (int i = 0; i < _numHiddenLayers + 1; ++i)
        {
            //Each neuron
            for (int j = 0; j < layers[i].NumNeurons; ++j)
            {
                //Each weight
                for (int k = 0; k < layers[i].Neurons[j].NumInputs; ++k)
                {
                    weights.Add(layers[i].Neurons[j].Weight[k]);
                }
            }
        }

        return weights;
    }

    //Gets number of weights
    public int GetNumberOfWeights()
    {
        int weights = 0;

        //Each layer
        for (int i = 0; i < _numHiddenLayers + 1; ++i)
        {
            //Eeach neuron
            for (int j = 0; j < layers[i].NumNeurons; ++j)
            {
                //Each weight
                for (int k = 0; k < layers[i].Neurons[j].NumInputs; ++k)
                    weights++;
            }
        }

        return weights;
    }

    //Sets weights for network (initially set to random values)
    public void SetWeights(ref List<float> weights)
    {
        //Used to cycle through received weights
        int weightCount = 0;

        //Each layer
        for (int i = 0; i < _numHiddenLayers + 1; ++i)
        {
            //Each neuron
            for (int j = 0; j < layers[i].NumNeurons; ++j)
            {
                //Each weight
                for (int k = 0; k < layers[i].Neurons[j].NumInputs; ++k)
                {
                    layers[i].Neurons[j].Weight[k] = weights[weightCount++];
                }
            }
        }
    }

    //S shaped output
    public float Sigmoid(float netInput)
    {
        return (float)(1 / (1 + Math.Exp(-netInput / Utilities.instance.activationResponse)));
    }

    
    public override bool Equals(object obj)
    {
        if(!(obj is NeuralNet))
        {
            return false;
        }

        if(obj == this)
        {
            return true;
        }

        NeuralNet other = (NeuralNet)obj;

        return (other._numOfInput == this._numOfInput && other._numOfOutput == this._numOfOutput
         && other._numHiddenLayers == this._numHiddenLayers
         && other._numOfNeuronsPerHiddenLayer == this._numOfNeuronsPerHiddenLayer);
    }

    public override int GetHashCode()
    {
        return new HashCodeBuilder().
                        Add(numOfInput).
                        Add(numOfOutput).
                        Add(numOfHiddenLayers).
                        Add(numOfNeuronsPerHiddenLayer).
                        GetHashCode();
    }

    private int GetHash(int value)
    {
        return 31 + value; 
    }
}

public sealed class HashCodeBuilder
{
    private int hash = 17;

    public HashCodeBuilder Add(int value)
    {
        unchecked
        {
            hash = hash * 31 + value; 
        }
        return this;
    }

    public HashCodeBuilder Add(object value)
    {
        return Add(value != null ? value.GetHashCode() : 0);
    }

    public HashCodeBuilder Add(float value)
    {
        return Add(value.GetHashCode());
    }

    public HashCodeBuilder Add(double value)
    {
        return Add(value.GetHashCode());
    }

    public override int GetHashCode()
    {
        return hash;
    }
}


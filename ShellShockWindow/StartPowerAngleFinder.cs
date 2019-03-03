using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xaml.Schema;
using TWTCMachineLearning;

namespace ShellShockWindow
{
    public class StartPowerAngleFinder
    {
        private const string Location = @"C:\Users\Roopal\Documents\Aashish\Shellshock";
        private const string Name = @"ArtificialShellshock_Final";
        private static readonly int[] Layers = new[] { 19, 25, 25, 2 };
        private const double LearningRate = 0.03;

        private NeuralNetwork neuralNetwork;

        public StartPowerAngleFinder()
        {
            NetworkStartInfo startInfo = new NetworkStartInfo(Name, Layers, LearningRate, Location, 0);
            neuralNetwork = new NeuralNetwork(startInfo);
        }

        public int[] FindStartingPowerAngle(double[] inputs)
        {
            double[] inputDoubles = neuralNetwork.FindOutput(inputs);
            int[] inputInts = new int[inputDoubles.Length];

            inputInts[0] = Convert.ToInt32((inputDoubles[0]+1)*50);
            inputInts[1] = Convert.ToInt32((inputDoubles[1]+1)*90);

            return inputInts;
        }
    }
}
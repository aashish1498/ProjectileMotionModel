using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellShockWindow
{
    public class NeuralNetworkParameters
    {
        const int numberOfInputs = 19;
        public double[] MyTankPosition = new double[2];
        public double[] EnemyTankPosition = new double[2];
        public double[] LinearBumper1 = new double[2];
        public double[] LinearBumper2 = new double[2];
        public double[] CircularBumper1 = new double[2];
        public double[] CircularBumper2 = new double[2];
        public double[] CircularBumper3 = new double[2];
        public double[] Portal1 = new double[2];
        public double[] Portal2 = new double[2];

        public double Wind;

        public double[] GetInputData()
        {
            double[] inputData = new double[numberOfInputs];
            NormalizeXY(MyTankPosition).CopyTo(inputData, 0);
            NormalizeXY(EnemyTankPosition).CopyTo(inputData, 2);
            NormalizeXY(LinearBumper1).CopyTo(inputData, 4);
            NormalizeXY(LinearBumper2).CopyTo(inputData, 6);
            NormalizeCircularBumpers(CircularBumper1).CopyTo(inputData, 8);
            NormalizeCircularBumpers(CircularBumper2).CopyTo(inputData, 10);
            NormalizeCircularBumpers(CircularBumper3).CopyTo(inputData, 12);
            NormalizeXY(Portal1).CopyTo(inputData, 14);
            NormalizeXY(Portal2).CopyTo(inputData, 16);
            inputData[numberOfInputs - 1] = NormalizeWind(Wind);

            return inputData;
        }

        private double[] NormalizeXY(double[] unnormalized)
        {
            double[] normalizedDoubles = new double[unnormalized.Length];
            normalizedDoubles[0] = Normalize(unnormalized[0], 0.0, 571, -1.0);
            normalizedDoubles[1] = Normalize(unnormalized[1], 0.0, 377.5, -1.0);
            return normalizedDoubles;
        }

        private double[] NormalizeCircularBumpers(double[] unnormalized)
        {
            double[] normalizedDoubles = new double[unnormalized.Length];
            normalizedDoubles[0] = Normalize(unnormalized[0], 360.0, 931, -1.0);
            normalizedDoubles[1] = Normalize(unnormalized[1], 360.0, 737.5, -1.0);
            return normalizedDoubles;
        }

        private double NormalizeWind(double wind)
        {
            return Normalize(wind, 0.0, 100, 0.0);
        }

        //private double[] NormalizeCircularBumpers()

        private double Normalize(double unnormalized, double addition, double divider, double bias)
        {
            double normalized = (unnormalized + addition) / divider + bias;
            if (normalized < -1)
                return -1;
            if (normalized > 1)
                return 1;
            return normalized;
        }
    }
}

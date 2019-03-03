using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShellShockWindow;

namespace ShellShockAI
{
    class Program
    {
        private const string _filePath = @"C:\Users\Roopal\Documents\Aashish\Shellshock\TrainingData.csv";
        static void Main(string[] args)
        {
            //Initialise everything
            World newWorld = new World();
            Portal newPortal = new Portal(10, 35, 5, 100, 30);
            Bumper newBumper = new Bumper();
            SuvatSolver newSuvatSolver = new SuvatSolver(newWorld.g, newWorld.WindConstant);
            ODESolution newOde = new ODESolution(newWorld.g, newWorld.WindConstant);
            NeuralNetworkParameters networkParameters = new NeuralNetworkParameters();
            BruteForceMethods newBruteForceMethods = new BruteForceMethods(newOde, newBumper, networkParameters);

            for (int i = 1; i < 1000; i++)
            {
                Console.WriteLine("Starting simulation " + i + " of this batch");
                // Generate the random values and save to sheet
                RandomPositionGenerator newGenerator = new RandomPositionGenerator();
                SaveMethods saveMethods = new SaveMethods(_filePath);
                newGenerator.GenerateRandomValues();
                saveMethods.SaveInputs(newGenerator);
                // Save to file

                //Create a 'fake' ShellshockWindow app or something
                var keyCollection = newGenerator.AllVariables.Keys;
                var valueCollection = newGenerator.AllVariables.Values;

                double[] xVals = new double[keyCollection.Count];
                double[] yVals = new double[valueCollection.Count];

                keyCollection.CopyTo(xVals, 0);
                valueCollection.CopyTo(yVals, 0);

                //Wind
                newWorld.Wind = (int) yVals[9];

                // Tanks
                var myNewXPosition = xVals[0];
                var myNewYPosition = yVals[0];
                var enemyNewXPosition = xVals[1];
                var enemyNewYPosition = yVals[1];

                // Portals
                newPortal.BlueLeft = xVals[7];
                newPortal.BlueTop = yVals[7];
                newPortal.OrangeLeft = xVals[8];
                newPortal.OrangeTop = yVals[8];

                // Linear bumpers
                newBumper.LinearBumper1LeftPosition = xVals[2];
                newBumper.LinearBumper1TopPosition = yVals[2];
                newBumper.LinearBumper2LeftPosition = xVals[3];
                newBumper.LinearBumper2TopPosition = yVals[3];

                // Circular bumpers
                newBumper.CircularBumper1Left = xVals[4];
                newBumper.CircularBumper1Top = yVals[4];
                newBumper.CircularBumper2Left = xVals[5];
                newBumper.CircularBumper2Top = yVals[5];
                newBumper.CircularBumper3Left = xVals[6];
                newBumper.CircularBumper3Top = yVals[6];


                //Run methods to generate wind and power
                newBumper.isRebound = true;
                var isMirrored = myNewXPosition > enemyNewXPosition ? -1 : 1;

                double horizontalPixelDistance = Math.Abs(enemyNewXPosition - myNewXPosition);
                double horizontalMmDistance = horizontalPixelDistance * newWorld.ScreenWidthRatio * World.PixelToMm;
                newWorld.X = horizontalMmDistance;

                double verticalPixelDistance = myNewYPosition - enemyNewYPosition;
                double verticalMmDistance = verticalPixelDistance * newWorld.ScreenWidthRatio * World.PixelToMm;
                newWorld.Y = verticalMmDistance;

                // Now update portal positions
                newOde.BluePosition = newPortal.BluePosition(myNewXPosition, myNewYPosition,
                    newWorld.ScreenWidthRatio, isMirrored);
                newOde.OrangePosition = newPortal.OrangePosition(myNewXPosition, myNewYPosition,
                    newWorld.ScreenWidthRatio, isMirrored);

                newBumper.FindAllRelativePositions(myNewXPosition, myNewYPosition, newWorld.ScreenWidthRatio,
                    isMirrored);
                newOde.NewBumper = newBumper;
                newOde.IsMirrored = isMirrored;
                Console.WriteLine("Finding power and angle ...");
                double[] powerAngleGuess = BruteForceAsync(newBruteForceMethods, newWorld).Result;
                string powerGuess = powerAngleGuess[0].ToString("0");
                string angleGuess = powerAngleGuess[1].ToString("0");
                saveMethods.SaveOutputs(powerGuess, angleGuess);
                Console.WriteLine("Completed simulation " + i + " of this batch");
            }

        }
        private static async Task<double[]> BruteForceAsync(BruteForceMethods newBruteForceMethods, World newWorld)
        {
            double[] powerAngleGuess = await newBruteForceMethods.BruteForce(newWorld.X, newWorld.Y, newWorld.Wind);
            return powerAngleGuess;
        }
    }
}

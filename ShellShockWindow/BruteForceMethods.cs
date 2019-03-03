using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using TWTCMachineLearning;
using static System.Math;

namespace ShellShockWindow
{
    public class BruteForceMethods
    {
        public BruteForceMethods(ODESolution newOdeSolution, Bumper newBumper, NeuralNetworkParameters newNetworkParameters)
        {
            this._newOdeSolution = newOdeSolution;
            this._newBumper = newBumper;
            this._newNetworkParameters = newNetworkParameters;
            _newPowerAngleFinder = new StartPowerAngleFinder();
        }
        private ODESolution _newOdeSolution;
        private Bumper _newBumper;
        private NeuralNetworkParameters _newNetworkParameters;
        private StartPowerAngleFinder _newPowerAngleFinder;

        private bool _isOddPositiveCompleted;
        private bool _isEvenPositiveCompleted;
        private bool _isOddNegativeCompleted;
        private bool _isEvenNegativeCompleted;
        private double[] _powerAndAngleResult;

        private int startingPower;

        #region Start of Async methods

        public double[] BruteForceEvenPositive(double x, double y, double wind, int startingAngleEven)
        {
            _isEvenPositiveCompleted = false;
            double[] currentWinner = new double[3] { -1, -1, 10000 }; //Start current winner (Power, Angle, Minimum distance) 
            // for (int thetaDegreesIndex = 90; thetaDegreesIndex > 0; thetaDegreesIndex -= 2)
            for (int a = startingAngleEven; a < startingAngleEven + 90; a += 2)
            {
                if (_isOddPositiveCompleted || _isOddNegativeCompleted || _isEvenNegativeCompleted)
                {
                    return currentWinner;
                }

                int thetaDegreesIndex;
                if (a > 180)
                {
                    thetaDegreesIndex = a - 182;
                }
                else
                {
                    thetaDegreesIndex = a;
                }

                double thetaDegrees = thetaDegreesIndex;
                double thetaRadians = thetaDegrees * PI / 180;

                ShooterResult newShooterResultEvPos = _newOdeSolution.Shooter(x, y, thetaRadians, wind, startingPower);
                double newPower = newShooterResultEvPos.Power;

                IVPSolverResult newIvpSolverResultEvPos = _newOdeSolution.IVPSolver(x, y, thetaRadians, wind, newPower);
                double newMinimumDistance = newIvpSolverResultEvPos.MinimumDistance;

                if (_newBumper.isRebound && !newIvpSolverResultEvPos.DidIHitABumper)
                {
                    continue;
                }
                if (Abs(newMinimumDistance) < Abs(currentWinner[2]))
                {
                    currentWinner[0] = newPower;
                    currentWinner[1] = thetaDegrees;
                    currentWinner[2] = newMinimumDistance;
                }
                if (!newShooterResultEvPos.AMinimumDistanceWasFound) continue;
                _isEvenPositiveCompleted = true;
                return currentWinner;
            }
            return currentWinner;
        }

        public double[] BruteForceEvenNegative(double x, double y, double wind, int startingAngleEven)
        {
            _isEvenNegativeCompleted = false;
            double[] currentWinner = new double[3] { -1, -1, 10000 };
            //for (int thetaDegreesIndex = 90; thetaDegreesIndex < 180; thetaDegreesIndex += 2)
            for (int a = startingAngleEven - 2; a > startingAngleEven - 92; a -= 2)
            {
                if (_isOddPositiveCompleted || _isEvenPositiveCompleted || _isOddNegativeCompleted)
                {
                    return currentWinner;
                }

                int thetaDegreesIndex;
                if (a < 0)
                {
                    thetaDegreesIndex = a + 182;
                }
                else
                {
                    thetaDegreesIndex = a;
                }

                double thetaDegrees = thetaDegreesIndex;
                double thetaRadians = thetaDegrees * PI / 180;

                ShooterResult newShooterResultEvNeg = _newOdeSolution.Shooter(x, y, thetaRadians, wind, startingPower);
                double newPower = newShooterResultEvNeg.Power;

                IVPSolverResult newIvpSolverResultEvNeg = _newOdeSolution.IVPSolver(x, y, thetaRadians, wind, newPower);
                double newMinimumDistance = newIvpSolverResultEvNeg.MinimumDistance;

                if (_newBumper.isRebound && !newIvpSolverResultEvNeg.DidIHitABumper)
                {
                    continue;
                }
                if (Abs(newMinimumDistance) < Abs(currentWinner[2]))
                {
                    currentWinner[0] = newPower;
                    currentWinner[1] = thetaDegrees;
                    currentWinner[2] = newMinimumDistance;
                }
                if (!newShooterResultEvNeg.AMinimumDistanceWasFound) continue;
                _isEvenNegativeCompleted = true;
                return currentWinner;
            }
            return currentWinner;
        }

        public double[] BruteForceOddPositive(double x, double y, double wind, int startingAngleOdd)
        {
            _isOddPositiveCompleted = false;
            double[] currentWinner = new double[3] { -1, -1, 10000 };
            //for (int thetaDegreesIndex = 89; thetaDegreesIndex > 0; thetaDegreesIndex -= 2)
            for (int a = startingAngleOdd; a < startingAngleOdd + 90; a += 2)
            {
                if (_isEvenPositiveCompleted || _isOddNegativeCompleted || _isEvenNegativeCompleted)
                {
                    return currentWinner;
                }

                int thetaDegreesIndex;
                if (a > 180)
                {
                    thetaDegreesIndex = a - 182;
                }
                else
                {
                    thetaDegreesIndex = a;
                }
                double thetaDegrees = thetaDegreesIndex;
                double thetaRadians = thetaDegrees * PI / 180;
                ShooterResult newShooterResultOddPos = _newOdeSolution.Shooter(x, y, thetaRadians, wind, startingPower);
                double newPower = newShooterResultOddPos.Power;

                IVPSolverResult newIvpSolverResultOddPos = _newOdeSolution.IVPSolver(x, y, thetaRadians, wind, newPower);
                double newMinimumDistance = newIvpSolverResultOddPos.MinimumDistance;
                if (_newBumper.isRebound && !newIvpSolverResultOddPos.DidIHitABumper)
                {
                    continue;
                }
                if (Abs(newMinimumDistance) < Abs(currentWinner[2]))
                {
                    currentWinner[0] = newPower;
                    currentWinner[1] = thetaDegrees;
                    currentWinner[2] = newMinimumDistance;
                }
                if (!newShooterResultOddPos.AMinimumDistanceWasFound) continue;
                _isOddPositiveCompleted = true;
                return currentWinner;
            }
            return currentWinner;
        }

        public double[] BruteForceOddNegative(double x, double y, double wind, int startingAngleOdd)
        {
            _isOddNegativeCompleted = false;
            double[] currentWinner = new double[3] { -1, -1, 10000 };
            for (int a = startingAngleOdd - 2; a > startingAngleOdd - 92; a -= 2)
            {
                if (_isOddPositiveCompleted || _isEvenPositiveCompleted || _isEvenNegativeCompleted)
                {
                    return currentWinner;
                }
                int thetaDegreesIndex;
                if (a < 0)
                {
                    thetaDegreesIndex = a + 182;
                }
                else
                {
                    thetaDegreesIndex = a;
                }
                double thetaDegrees = thetaDegreesIndex;
                double thetaRadians = thetaDegrees * PI / 180;

                ShooterResult newShooterResultOddNeg = _newOdeSolution.Shooter(x, y, thetaRadians, wind, startingPower);
                double newPower = newShooterResultOddNeg.Power;

                IVPSolverResult newIvpSolverResultOddNeg = _newOdeSolution.IVPSolver(x, y, thetaRadians, wind, newPower);
                double newMinimumDistance = newIvpSolverResultOddNeg.MinimumDistance;

                if (_newBumper.isRebound && !newIvpSolverResultOddNeg.DidIHitABumper)
                {
                    continue;
                }
                if (Abs(newMinimumDistance) < Abs(currentWinner[2]))
                {
                    currentWinner[0] = newPower;
                    currentWinner[1] = thetaDegrees;
                    currentWinner[2] = newMinimumDistance;
                }
                if (!newShooterResultOddNeg.AMinimumDistanceWasFound) continue;
                _isOddNegativeCompleted = true;
                return currentWinner;
            }
            return currentWinner;
        }

        #endregion

        public async Task<double[]> BruteForce(double x, double y, double wind)
        {
            double[] inputData = _newNetworkParameters.GetInputData();
            int[] startPowerAngle = _newPowerAngleFinder.FindStartingPowerAngle(inputData);
            startingPower = startPowerAngle[0];
            if (startingPower < 21)
            {
                startingPower = 21;
            }

            int startingAngleEven;
            if (startPowerAngle[1].IsEven())
            {
                startingAngleEven = startPowerAngle[1];
            }
            else
            {
                startingAngleEven = startPowerAngle[1] + 1;
            }
            var startingAngleOdd = startingAngleEven - 1;
            
            List<Task<double[]>> tasks = new List<Task<double[]>>();
            Task<double[]> bruteEvenPositiveResult = Task.Run(() => BruteForceEvenPositive(x, y, wind, startingAngleEven));
            Task<double[]> bruteOddPositiveResult = Task.Run(() => BruteForceOddPositive(x, y, wind, startingAngleOdd));
            Task<double[]> bruteEvenNegativeResult = Task.Run(() => BruteForceEvenNegative(x, y, wind, startingAngleEven));
            Task<double[]> bruteOddNegativeResult = Task.Run(() => BruteForceOddNegative(x, y, wind, startingAngleOdd));

            tasks.Add(bruteEvenPositiveResult);
            tasks.Add(bruteOddPositiveResult);
            tasks.Add(bruteEvenNegativeResult);
            tasks.Add(bruteOddNegativeResult);

            var results = await Task.WhenAll(tasks);

            if (_isEvenPositiveCompleted)
            {
                _powerAndAngleResult = new[] { results[0][0], results[0][1] };
            }
            else if (_isOddPositiveCompleted)
            {
                _powerAndAngleResult = new[] { results[1][0], results[1][1] };
            }
            else if (_isEvenNegativeCompleted)
            {
                _powerAndAngleResult = new[] { results[2][0], results[2][1] };
            }
            else if (_isOddNegativeCompleted)
            {
                _powerAndAngleResult = new[] { results[3][0], results[3][1] };
            }
            else
            {
                int lowestDistanceKey = 0;
                double lowestDistanceSoFar = 10000.0;
                for (int i = 0; i < 4; i++)
                {
                    if (results[i][2] < lowestDistanceSoFar)
                    {
                        lowestDistanceKey = i;
                        lowestDistanceSoFar = results[i][2];
                    }
                }

                _powerAndAngleResult = new[] { results[lowestDistanceKey][0], results[lowestDistanceKey][1] };
            }
            // Update current z
            _newOdeSolution.IVPSolver(x, y, _powerAndAngleResult[1] * PI / 180, wind, _powerAndAngleResult[0]);
            return _powerAndAngleResult;
        }

    }
}

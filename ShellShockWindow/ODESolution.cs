using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathNet.Numerics;

namespace ShellShockWindow
{
    public class ODESolution
    {
        public ODESolution(double g, double windConstant)
        {
            _g = g;
            _windConstant = windConstant;
        }

        public int IsMirrored;
        public Bumper NewBumper;
        private double _theta;
        private double _wind;
        public double _g;
        public double _windConstant;
        //private readonly double _windConstant;
        public List<double[]> CurrentZ;
        public double PortalRadius = 7;

        private const double dt = 0.005;
        //private bool _aPortalHasJustBeenHit;
        //private bool _aBumperHasJustBeenHit;
        //private bool _aCircularBumperHasJustBeenHit;

        //public bool _aMinimumDistanceWasFound;
        //public bool _didIHitABumper;

        public double[] OrangePosition { get; set; }
        public double[] BluePosition { get; set; }

        public ShooterResult Shooter(double x, double y, double theta, double wind, int startingPower = 51)
        {
            //Iterate through all the powers
            bool aMinimumDistanceWasFound = false;

            List<double> distanceArray = new List<double>();

            for (int p = startingPower; p < startingPower + 80; p++)
            {
                int powerIndex = GetPowerFromIndex(p);
                double newDistance = Math.Abs(IVPSolver(x, y, theta, wind, powerIndex).MinimumDistance);
                distanceArray.Add(Math.Abs(newDistance));
                if (Math.Abs(newDistance) < 2)
                {
                    aMinimumDistanceWasFound = true;
                    //Update current Z
                    IVPSolver(x, y, theta, wind, powerIndex);
                    return new ShooterResult(powerIndex, aMinimumDistanceWasFound);
                }
            }
            int correctPower = GetPowerFromIndex(distanceArray.IndexOf(distanceArray.Min()) + startingPower);

            //for (int powerIndex = 21; powerIndex < 101; powerIndex++)
            //{
            //    double newDistance = Math.Abs(IVPSolver(x, y, theta, wind, powerIndex).MinimumDistance);
            //    distanceArray.Add(newDistance);
            //    if ( Math.Abs(newDistance) < 1)
            //    {
            //        aMinimumDistanceWasFound = true;
            //        //Update current Z
            //        IVPSolver(x, y, theta, wind, powerIndex);
            //        return new ShooterResult(powerIndex, aMinimumDistanceWasFound);
            //    }
            //}
            //int correctPower = distanceArray.IndexOf(distanceArray.Min()) + 21;


            //Update current Z
            IVPSolver(x, y, theta, wind, correctPower);
            return new ShooterResult(correctPower, aMinimumDistanceWasFound);
        }

        public IVPSolverResult IVPSolver(double x, double y, double theta, double wind, double currentPower)
        {
            _theta = theta;
            _wind = wind;
            // Create new array lists for the state vector z, and time t
            List<double[]> z = new List<double[]>();
            List<double> t = new List<double>();
            List<double> distanceArray = new List<double>();
            double currentTime = 0;
            t.Add(currentTime);
            int n = 0;
            int finalValuePosition = 0;
            // Initial vector in the form x, y, x(dot), y(dot)
            double[] z0 = { 0, 0, currentPower * Math.Cos(_theta), currentPower * Math.Sin(_theta) };
            z.Add(z0);

            bool shotHasReachedEnemy = false;
            bool powerShouldIncrease = false;
            bool didIHitABumper = false;
            bool aBumperHasJustBeenHit = false;
            bool aCircularBumperHasJustBeenHit = false;
            bool aPortalHasJustBeenHit = false;

            int noOfLinearHits = 0;
            int noOfCircularHits = 0;

            while (!(z[n][1] < -200 && z[n][3] < 0)) //While the shot is not moving downwards beyond the screen
            {
               
                //Step through time
                currentTime += dt;
                t.Add(currentTime);
                if (n > 8000)
                {
                    CurrentZ = z;
                    return new IVPSolverResult(double.NaN, false);
                }

                double[] currentRow = z[n];
                double[] newRow = StepRungeKutta(currentRow);

                switch (PortalHit(newRow, ref aPortalHasJustBeenHit) )
                {
                    case 0:
                        break;
                    case 1:
                        newRow = FindTransportedPosition(newRow, 1);
                        break;
                    case 2:
                        newRow = FindTransportedPosition(newRow, 2);
                        break;
                    case -1:
                        break;
                }

                if (ACircularBumperIsHit(newRow, ref aCircularBumperHasJustBeenHit, ref noOfCircularHits))
                {
                    noOfCircularHits++;
                    didIHitABumper = true;
                }
                else if (ABumperIsHit(newRow, ref aBumperHasJustBeenHit, ref noOfLinearHits))
                {
                    noOfLinearHits++;
                    didIHitABumper = true;
                }

                z.Add(newRow);
                distanceArray.Add(FindDistance(z[n][0], z[n][1], x, y));

                if ((z[n][1].AlmostEqual(y, 0) && z[n][3] < 0))
                {
                    finalValuePosition = n;
                    shotHasReachedEnemy = true;
                }
                if ((z[n][1].AlmostEqual(y, 0) && z[n][3] > 0)) // When it reaches the tank and is traveling upwards
                {
                    // If the shot is to the right of the tank (and its final position is past the tank), the power should increase
                    powerShouldIncrease = z[n][0] > x;
                }
                n++;
            }

            //aPortalHasJustBeenHit = false;
            CurrentZ = z;
            double minimumDistance = Math.Abs(distanceArray.Min());
            if (shotHasReachedEnemy)
            {
                z.RemoveRange(finalValuePosition + 1, z.Count - finalValuePosition - 1);
                n = finalValuePosition;

                if (z[n][0] < x)
                {
                    return new IVPSolverResult(minimumDistance, didIHitABumper);
                }

                if (powerShouldIncrease)
                {
                    return new IVPSolverResult(minimumDistance, didIHitABumper);
                }
                return new IVPSolverResult(-minimumDistance, didIHitABumper);
            }
            // If the shot has not reached the enemy

            return new IVPSolverResult( Math.Abs(minimumDistance), didIHitABumper);
        }

        private double[] StateDeriv(double[] z)
        {
            double dz1 = z[2];
            double dz2 = z[3];
            double dz3 = _wind * _windConstant; ;
            double dz4 = -_g;
            double[] dz = { dz1, dz2, dz3, dz4 };

            return dz;
        }

        // StepRungeKutta will find the next z vector to be used
        private double[] StepRungeKutta(double[] z)
        {
            double[] a = MultiplyArrayByConstant(StateDeriv(z), dt);
            double[] b = MultiplyArrayByConstant(StateDeriv(AddTwoArraysTogether(z, MultiplyArrayByConstant(a, 0.5))), dt);
            double[] c = MultiplyArrayByConstant(StateDeriv(AddTwoArraysTogether(z, MultiplyArrayByConstant(b, 0.5))), dt);
            double[] d = MultiplyArrayByConstant(StateDeriv(AddTwoArraysTogether(z, c)), dt);

            double[] aPlusD = AddTwoArraysTogether(a, d);
            double[] twoBplusTwoC = AddTwoArraysTogether(MultiplyArrayByConstant(b, 2), MultiplyArrayByConstant(c, 2));
            //double dz = (A + (2 * B) + (2 * C) + D) / 6;
            double[] dz = MultiplyArrayByConstant(AddTwoArraysTogether(aPlusD, twoBplusTwoC), 1 / (double)6);

            double[] zNext = AddTwoArraysTogether(z, dz);
            return zNext;
        }

        #region Helper Methods

        private bool ABumperIsHit(double[] newRow, ref bool aBumperHasJustBeenHit, ref int noOfLinearHits)
        {
            if (noOfLinearHits > 4)
            {
                return false;
            }
            double xPosition = newRow[0];
            double yPosition = newRow[1];
            double xVelocity = newRow[2];
            double yVelocity = newRow[3];

            NewBumper.FindLeftAndRightBumperPositions();

            if (xPosition < NewBumper.LeftBumperX || xPosition > NewBumper.RightBumperX)
            {
                aBumperHasJustBeenHit = false;
                return false;
            }

            double m = (NewBumper.RightBumperY - NewBumper.LeftBumperY) /
                       (NewBumper.RightBumperX - NewBumper.LeftBumperX);
            double c = (NewBumper.RightBumperY - m * NewBumper.RightBumperX);
            double yBumper = m * xPosition + c;

            //If the horizontal distance between the bumpers are small, assume that if the shot is in vertical range, it hits the bumper

            if (Math.Abs(NewBumper.LeftBumperX - NewBumper.RightBumperX) < 6 && 
                yPosition > Math.Min(NewBumper.LeftBumperY, NewBumper.RightBumperY) &&
                yPosition < Math.Max(NewBumper.LeftBumperY, NewBumper.RightBumperY))
            {
                yBumper = yPosition;
            }

            if (yBumper.AlmostEqual(yPosition, (double)1))
            {
                if (!aBumperHasJustBeenHit)
                {
                    double velocityMagnitude = Math.Sqrt(Math.Pow(xVelocity, 2) + Math.Pow(yVelocity, 2));
                    double theta = Math.Atan2(yVelocity, xVelocity);
                    double newTheta = 2 * (IsMirrored * NewBumper.CalculateAlpha()) - theta;
                    newRow[2] = velocityMagnitude * Math.Cos(newTheta);
                    newRow[3] = velocityMagnitude * Math.Sin(newTheta);
                    aBumperHasJustBeenHit = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            aBumperHasJustBeenHit = false;
            return false;
        }

        private bool ACircularBumperIsHit(double[] newRow, ref bool aCircularBumperHasJustBeenHit, ref int noOfCircularHits)
        {
            if (noOfCircularHits > 4)
            {
                return false;
            }
            
            double xPosition = newRow[0];
            double yPosition = newRow[1];
            double xVelocity = newRow[2];
            double yVelocity = newRow[3];

            if (Math.Abs((Math.Pow((xPosition - NewBumper.CircleCentre[0]), 2) +
                          Math.Pow((yPosition - NewBumper.CircleCentre[1]), 2)) - Math.Pow(NewBumper.CircleRadius, 2)) < 9)
            {
                if (!aCircularBumperHasJustBeenHit)
                {
                    double velocityMagnitude = Math.Sqrt(Math.Pow(xVelocity, 2) + Math.Pow(yVelocity, 2));
                    double theta = Math.Atan2(yVelocity, xVelocity);
                    double newTheta = 2 * NewBumper.CalculateCircularAlpha(xPosition, yPosition) - theta;

                    newRow[2] = velocityMagnitude * Math.Cos(newTheta);
                    newRow[3] = velocityMagnitude * Math.Sin(newTheta);
                    aCircularBumperHasJustBeenHit = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            aCircularBumperHasJustBeenHit = false;
            return false;
        }

        private int PortalHit(double[] newRow, ref bool aPortalHasJustBeenHit)
        {
            double horizontalCheck = newRow[0];
            double verticalCheck = newRow[1];
            if ((Math.Pow(horizontalCheck - OrangePosition[0], 2) + Math.Pow(verticalCheck - OrangePosition[1], 2)) <
                Math.Pow(PortalRadius, 2))
            {
                if (aPortalHasJustBeenHit)
                {
                    return -1;
                }
                aPortalHasJustBeenHit = true;
                return 1;
            }
            if ((Math.Pow(horizontalCheck - BluePosition[0], 2) + Math.Pow(verticalCheck - BluePosition[1], 2)) <
                Math.Pow(PortalRadius, 2))
            {
                if (aPortalHasJustBeenHit)
                {
                    return -1;
                }
                aPortalHasJustBeenHit = true;
                return 2;
            }
            aPortalHasJustBeenHit = false;
            return 0;
        }

        private double[] FindTransportedPosition(double[] newRow, int identifier)
        {
            double[] transportedRow = newRow;
            switch (identifier)
            {
                case 1: // The orange portal has been hit
                    transportedRow[0] = (BluePosition[0] + (newRow[0] - OrangePosition[0]));
                    transportedRow[1] = (BluePosition[1] + (newRow[1] - OrangePosition[1]));
                    break;
                case 2: // The orange portal has been hit
                    transportedRow[0] = (OrangePosition[0] + (newRow[0] - BluePosition[0]));
                    transportedRow[1] = (OrangePosition[1] + (newRow[1] - BluePosition[1]));
                    break;
            }

            return transportedRow;
        }

        private double[] MultiplyArrayByConstant(double[] arrayName, double multiplier)
        {
            double[] newArray = new double[arrayName.Length];
            int i = 0;
            foreach (double item in arrayName)
            {
                newArray[i] = item * multiplier;
                i++;
            }
            return newArray;
        }

        private double[] AddTwoArraysTogether(double[] array1, double[] array2)
        {
            double[] newArray = new double[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                newArray[i] = array1[i] + array2[i];
            }
            return newArray;
        }

        private double FindDistance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));

        }

        private int GetPowerFromIndex(int index)
        {
            if (index > 100)
            {
                return index - 81;
            }

            return index;
        }
        #endregion
    }
}

using System;

namespace ShellShockWindow
{
    public class Bumper
    {
        public double LinearBumper1LeftPosition = 126;
        public double LinearBumper1TopPosition = 54;
        public double LinearBumper2LeftPosition = 126;
        public double LinearBumper2TopPosition = 35;

        public double CircularBumper1Left = 234;
        public double CircularBumper1Top = 25;
        public double CircularBumper2Left = 248;
        public double CircularBumper2Top = 45;
        public double CircularBumper3Left = 263;
        public double CircularBumper3Top = 30;

        public double LeftBumperX;
        public double LeftBumperY;
        public double RightBumperX;
        public double RightBumperY;

        public double[] RelativePosition1;
        public double[] RelativePosition2;
        public double[] RelativeCircularPosition1;
        public double[] RelativeCircularPosition2;
        public double[] RelativeCircularPosition3;

        public double[] CircleCentre = new double[2];
        public double CircleRadius;

        public bool isRebound;

        public void FindLeftAndRightBumperPositions()
        {
            var relativeXPosition1 = RelativePosition1[0];
            var relativeYPosition1 = RelativePosition1[1];

            var relativeXPosition2 = RelativePosition2[0];
            var relativeYPosition2 = RelativePosition2[1];

            if (relativeXPosition1 < relativeXPosition2)
            {
                LeftBumperX = relativeXPosition1;
                LeftBumperY = relativeYPosition1;
                RightBumperX = relativeXPosition2;
                RightBumperY = relativeYPosition2;
            }
            else
            {
                LeftBumperX = relativeXPosition2;
                LeftBumperY = relativeYPosition2;
                RightBumperX = relativeXPosition1;
                RightBumperY = relativeYPosition1;
            }
        }

        public double CalculateAlpha()
        {
            // The y distances are swapped because origin is top-left
            double alpha = Math.Atan((LinearBumper2TopPosition - LinearBumper1TopPosition) /
                                     (LinearBumper1LeftPosition - LinearBumper2LeftPosition));

            return alpha;
        }

        public double CalculateCircularAlpha(double shotXPosition, double shotYPosition)
        {
            double zeta = Math.Atan2(shotYPosition - CircleCentre[1], 
                shotXPosition - CircleCentre[0]);
            if ((shotYPosition - CircleCentre[1]) < 0)
            {
                return zeta + Math.PI / 2;
            }
            return zeta - Math.PI/2;
        }

        public void FindAllRelativePositions(double tankLeft, double tankTop, double screenWidthRatio, int isMirrored)
        {
            RelativePosition1 = FindRelativePosition(tankLeft, tankTop, screenWidthRatio, isMirrored, LinearBumper1LeftPosition, LinearBumper1TopPosition);
            RelativePosition2 = FindRelativePosition(tankLeft, tankTop, screenWidthRatio, isMirrored, LinearBumper2LeftPosition, LinearBumper2TopPosition);
            RelativeCircularPosition1 = FindRelativePosition(tankLeft, tankTop, screenWidthRatio, isMirrored, CircularBumper1Left, CircularBumper1Top);
            RelativeCircularPosition2 = FindRelativePosition(tankLeft, tankTop, screenWidthRatio, isMirrored, CircularBumper2Left, CircularBumper2Top);
            RelativeCircularPosition3 = FindRelativePosition(tankLeft, tankTop, screenWidthRatio, isMirrored, CircularBumper3Left, CircularBumper3Top);
            SetCircleParameters(RelativeCircularPosition1[0], RelativeCircularPosition1[1], RelativeCircularPosition2[0],
                RelativeCircularPosition2[1], RelativeCircularPosition3[0], RelativeCircularPosition3[1]);
        }

        private double[] FindRelativePosition(double tankLeft, double tankTop, double screenWidthRatio, int isMirrored, double leftPosition, double topPosition)
        {
            double leftRelativePosition = leftPosition - tankLeft;
            double topRelativePosition = tankTop - topPosition;

            double leftMm = leftRelativePosition * screenWidthRatio * World.PixelToMm * isMirrored;
            double topMm = topRelativePosition * screenWidthRatio * World.PixelToMm;
            return new double[2] { leftMm, topMm };
        }

        private void SetCircleParameters(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            double C12 = CalculateConstantC(x1, y1, x2, y2);
            double C23 = CalculateConstantC(x2, y2, x3, y3);
            double D12 = CalculateConstantD(x1, y1, x2, y2);
            double D23 = CalculateConstantD(x2, y2, x3, y3);

            double b = (C23 - C12) / (D23 - D12);
            double a = C12 - b * D12;
            double r = Math.Sqrt(Math.Pow((x1 - a), 2) + Math.Pow((y1 - b), 2));

            CircleCentre[0] = a;
            CircleCentre[1] = b;
            CircleRadius = r;
        }

        public static double CalculateConstantD(double x1, double y1, double x2, double y2)
        {
            return (y1 - y2) / (x1 - x2);
        }

        public static double CalculateConstantC(double x1, double y1, double x2, double y2)
        {
            double numerator = Math.Pow(x1, 2) - Math.Pow(x2, 2) + Math.Pow(y1, 2) - Math.Pow(y2, 2);
            double denomiator = 2 * (x1 - x2);
            return numerator / denomiator;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellShockAI
{
    class RandomPositionGenerator
    {
        Random masterRandom = new Random((int)DateTime.Now.Ticks);

        double x1;
        double y1;
        double x2;
        double y2;
        double x3;
        double y3;

        public OrderedDictionary AllVariables = new OrderedDictionary();
        public void GenerateRandomValues()
        {
            FindCirclePositions();

            AllVariables.Clear();
            AllVariables.Add(RandomNumber(0, 1142), RandomNumber(0, 755)); // My tank
            AllVariables.Add(RandomNumber(0, 1142), RandomNumber(0, 755)); // Enemy tank
            AllVariables.Add(RandomNumber(0, 1142), RandomNumber(0, 755)); // Left bumper 1
            AllVariables.Add(RandomNumber(0, 1142), RandomNumber(0, 755)); // Left bumper 2
            AllVariables.Add(x1,y1);                                       // Circular bumper 1
            AllVariables.Add(x2,y2);                                       // Circular bumper 2
            AllVariables.Add(x3,y3);                                       // Circular bumper 3
            AllVariables.Add(RandomNumber(0, 1142), RandomNumber(0, 755)); // Portal 1
            AllVariables.Add(RandomNumber(0, 1142), RandomNumber(0, 755)); // Portal 2
            AllVariables.Add(-1.001, masterRandom.Next(-100, 100));        // Wind
        }

        public double RandomThetaGenerator()
        {
            return RandomNumber(0, 2 * Math.PI);
        }

        private double RandomNumber(double min, double max)
        {
            return min + masterRandom.NextDouble() * (max - min);
        }

        private void FindCirclePositions()
        {
            var circleX = RandomNumber(0, 1142);
            var circleY = RandomNumber(0, 755);
            var circleRadius = RandomNumber(30, 360);

            x1 = circleX + circleRadius * Math.Cos(RandomThetaGenerator());
            y1 = circleY + circleRadius + Math.Sin(RandomThetaGenerator());
            x2 = circleX + circleRadius * Math.Cos(RandomThetaGenerator());
            y2 = circleY + circleRadius + Math.Sin(RandomThetaGenerator());
            x3 = circleX + circleRadius * Math.Cos(RandomThetaGenerator());
            y3 = circleY + circleRadius + Math.Sin(RandomThetaGenerator());
        }
    }
}

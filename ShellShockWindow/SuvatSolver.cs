using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellShockWindow
{
    public class SuvatSolver
    {
        public SuvatSolver(double g, double windConstant)
        {
            this._g = g;
            this._windConstant = windConstant;
        }

        //private double _x;
        //private double _y;
        //private double _theta;
        //private readonly int _wind;
        private readonly double _g;
        public double _windConstant;
        //private readonly double _windConstant;

        public double CalculatePower(double x, double y, double theta, double wind)
        {
            double w = wind * _windConstant;
            double timeSquared = 2 * ((x * Math.Sin(theta)) - (y * Math.Cos(theta))) /
                                 ((_g * Math.Cos(theta)) + (w * Math.Sin(theta)));

            double power = ((2 * x) - (w * timeSquared)) / (2 * Math.Sqrt(timeSquared) * Math.Cos(theta));
            return power;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellShockWindow
{
    public class World
    {
        public double X;
        public double Y;
        public double WindConstant = 0.028925; //0.028925
        public int Wind = 0;
        public double Theta;
        public double g = 22.1; //Used to be 22.1
        public double ScreenWidthRatio = 1;
        public static double PixelToMm = 0.252;
    }
}

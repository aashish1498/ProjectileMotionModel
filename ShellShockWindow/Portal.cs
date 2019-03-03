using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellShockWindow
{
    /// <summary>
    /// Contains the positions and dimensions of the portal
    /// </summary>
    public class Portal
    {
        public Portal(double blueLeft, double blueTop, double orangeLeft, double orangeTop, double portalRadius)
        {
            this.BlueLeft = blueLeft;
            this.BlueTop = blueTop;
            this.OrangeLeft = orangeLeft;
            this.OrangeTop = orangeTop;
            this.PortalRadius = portalRadius;
        }

        public double BlueLeft { get; set; }
        public double OrangeLeft { get; set; }
        public double BlueTop { get; set; }
        public double OrangeTop { get; set; }
        public double PortalRadius { get; set; }

        public double[] BluePosition(double tankLeft, double tankTop, double screenWidthRatio, int isMirrored)
        {
            double blueLeftRelativePosition = (BlueLeft + PortalRadius) - tankLeft;
            double blueTopRelativePosition = tankTop - (BlueTop + PortalRadius);

            double blueLeftMm = blueLeftRelativePosition * screenWidthRatio * World.PixelToMm * isMirrored;
            double blueTopMm = blueTopRelativePosition * screenWidthRatio * World.PixelToMm;
            return new double[2] {blueLeftMm, blueTopMm};
        }

        public double[] OrangePosition(double tankLeft, double tankTop, double screenWidthRatio, int isMirrored)
        {
            double orangeLeftRelativePosition = (OrangeLeft + PortalRadius) - tankLeft;
            double orangeTopRelativePosition = tankTop - (OrangeTop + PortalRadius);

            double orangeLeftMm = orangeLeftRelativePosition * screenWidthRatio * World.PixelToMm * isMirrored;
            double orangeTopMm = orangeTopRelativePosition * screenWidthRatio * World.PixelToMm;
            return new double[2] {orangeLeftMm, orangeTopMm};
        }
    }
}

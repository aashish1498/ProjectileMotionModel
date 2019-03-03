using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellShockWindow
{
    public class ShooterResult
    {
        public ShooterResult(double power, bool aMinimumDistanceWasFound)
        {
            this.AMinimumDistanceWasFound = aMinimumDistanceWasFound;
            this.Power = power;
        }

        public double Power;
        public bool AMinimumDistanceWasFound;
    }
}

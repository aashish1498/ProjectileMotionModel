using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellShockWindow
{
    public class IVPSolverResult
    {
        public IVPSolverResult(double minimumDistance, bool didIHitABumper)
        {
            this.DidIHitABumper = didIHitABumper;
            this.MinimumDistance = minimumDistance;
        }

        public double MinimumDistance;
        public bool DidIHitABumper;
    }
}

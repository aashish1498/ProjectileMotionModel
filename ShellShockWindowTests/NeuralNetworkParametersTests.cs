using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShellShockWindow;

namespace ShellShockWindowTests
{
    [TestClass()]
    public class NeuralNetworkParametersTests
    {
        [TestMethod()]
        public void GetInputDataTest()
        {
            NeuralNetworkParameters myNetworkParameters = new NeuralNetworkParameters();
            myNetworkParameters.MyTankPosition = new[] {1.0, 2.0};
            myNetworkParameters.EnemyTankPosition = new[] {3.0, 4.0};
            myNetworkParameters.LinearBumper1 = new[] {5.0, 6.0};
            myNetworkParameters.LinearBumper2 = new[] {7.0, 8.0};
            myNetworkParameters.CircularBumper1 = new[] {9.0, 10.0};
            myNetworkParameters.CircularBumper2 = new[] {11.0, 12.0};
            myNetworkParameters.CircularBumper3 = new[] {13.0, 14.0};
            myNetworkParameters.Portal1 = new[] {15.0, 16.0};
            myNetworkParameters.Portal2 = new[] {17.0, 18.0};
            myNetworkParameters.Wind = 19;

            double[] inputArray = myNetworkParameters.GetInputData();
            double[] expectedArray = new[] {1.0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19};
            Assert.AreEqual(expectedArray, inputArray);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShellShockWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Initialising variables
        // Instantiate a class with world variables, and calculation method
        public static World NewWorld = new World();
        public static Portal NewPortal = new Portal(10, 35, 5, 100, 30);
        private Bumper _newBumper = new Bumper();
        private readonly SuvatSolver _newSuvatSolver = new SuvatSolver(NewWorld.g, NewWorld.WindConstant);
        private readonly ODESolution _newOde = new ODESolution(NewWorld.g, NewWorld.WindConstant);
        private BruteForceMethods _newBruteForceMethods;
        public NeuralNetworkParameters MyNetworkParameters = new NeuralNetworkParameters();

        private bool _thereIsDistance = true;
        private bool _thereIsAngle = true;
        private bool _thereIsWind = true;
        private bool _waitedLongEnough;

        private int _isMirrored = 1;

        //Initiate variables to hold new inputs
        private double _newPower;
        private int _newAngleInDegrees;
        private int _newWind;
        private int _newPowerGuess;
        private double gravityConstant;
        private double windConstant;

        //Positions of my, and the enemy's tank
        private double _myNewXPosition = 205;
        private double _myNewYPosition = 239;
        private double _enemyNewXPosition = 607;
        private double _enemyNewYPosition = 239;

        //Portal positions
        private double _bluePortalLeftPosition;
        private double _bluePortalTopPosition;
        private double _orangePortalLeftPosition;
        private double _orangePortalTopPosition;

        //Bumper positions
        private double _linearBumper1LeftPosition;
        private double _linearBumper1TopPosition;
        private double _linearBumper2LeftPosition;
        private double _linearBumper2TopPosition;

        private double _circularBumper1Left = 234;
        private double _circularBumper1Top = 25;
        private double _circularBumper2Left = 248;
        private double _circularBumper2Top = 45;
        private double _circularBumper3Left = 263;
        private double _circularBumper3Top = 30;

        //constants
        private readonly int _screenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        private readonly int _screenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

        private readonly double _tankWidthHalf;

        private int _recordingLinear = -1;
        private int _recordingCircular = -1;

        public MainWindow()
        {
            InitializeComponent();
            UpdatePositions();
            _tankWidthHalf = myTankThumb.Width / 2.0;
            _newBruteForceMethods = new BruteForceMethods(_newOde, _newBumper, MyNetworkParameters);
        }

        #endregion

        #region UI Changes

        private void Angle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(Angle.Text, out _newAngleInDegrees))
            {
                _thereIsAngle = true;
                NewWorld.Theta = (_newAngleInDegrees * Math.PI / 180);
                UpdateSuvatPowerTextbox();
            }
            else
            {
                _thereIsAngle = false;
            }
        }

        private void Wind_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(Wind.Text, out _newWind))
            {
                _thereIsWind = true;
                NewWorld.Wind = _newWind;
                UpdateSuvatPowerTextbox();
            }
            else
            {
                _thereIsWind = false;
            }
        }

        private void ThinkWithPortals_Click(object sender, RoutedEventArgs e)
        {
            double powerGuess = _newOde.Shooter(NewWorld.X, NewWorld.Y, NewWorld.Theta, NewWorld.Wind).Power;
            Debug.Text = powerGuess.ToString("0.00");
            if (!(double.IsInfinity(powerGuess) || double.IsNaN(powerGuess)))
                PlotGraph();
        }

        private void MyThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            _myNewXPosition = Canvas.GetLeft(myTankThumb) + e.HorizontalChange + _tankWidthHalf;
            _myNewYPosition = Canvas.GetTop(myTankThumb) + e.VerticalChange + _tankWidthHalf;

            MyNetworkParameters.MyTankPosition = new[] {_myNewXPosition, _myNewYPosition};

            if ((_myNewXPosition > _tankWidthHalf) && (_myNewXPosition < _screenWidth))
            {
                Canvas.SetLeft(myTankThumb, _myNewXPosition - _tankWidthHalf);
            }

            if ((_myNewYPosition > _tankWidthHalf) && (_myNewYPosition < (_screenHeight - _tankWidthHalf)))
            {
                Canvas.SetTop(myTankThumb, _myNewYPosition - _tankWidthHalf);
            }
            UpdatePositions();

        }

        private void EnemyThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            _enemyNewXPosition = Canvas.GetLeft(enemyTankThumb) + e.HorizontalChange + _tankWidthHalf;
            _enemyNewYPosition = Canvas.GetTop(enemyTankThumb) + e.VerticalChange + _tankWidthHalf;

            MyNetworkParameters.EnemyTankPosition = new[] { _enemyNewXPosition, _enemyNewYPosition };

            if ((_enemyNewXPosition > _tankWidthHalf) && (_enemyNewXPosition < _screenWidth))
            {
                Canvas.SetLeft(enemyTankThumb, _enemyNewXPosition - _tankWidthHalf);
            }

            if ((_enemyNewYPosition > _tankWidthHalf) && (_enemyNewYPosition < (_screenHeight - _tankWidthHalf)))
            {
                Canvas.SetTop(enemyTankThumb, _enemyNewYPosition - _tankWidthHalf);
            }

            UpdatePositions();
        }

        private void LinearBumper1_OnDragDelta_DragDelta(object sender, DragDeltaEventArgs e)
        {
            _linearBumper1LeftPosition = Canvas.GetLeft(LinearBumper1) + e.HorizontalChange;
            _linearBumper1TopPosition = Canvas.GetTop(LinearBumper1) + e.VerticalChange;

            MyNetworkParameters.LinearBumper1 = new[] { _linearBumper1LeftPosition, _linearBumper1TopPosition};

            if ((_linearBumper1LeftPosition > 0) && (_linearBumper1LeftPosition < _screenWidth))
            {
                Canvas.SetLeft(LinearBumper1, _linearBumper1LeftPosition);
            }

            if ((_linearBumper1TopPosition > 0) && (_linearBumper1TopPosition < (0.95 * _screenHeight)))
            {
                Canvas.SetTop(LinearBumper1, _linearBumper1TopPosition);
            }

            // Find distances relative to my tank position
            _newBumper.LinearBumper1LeftPosition = _linearBumper1LeftPosition + 5;
            _newBumper.LinearBumper1TopPosition = _linearBumper1TopPosition + 5;
            UpdatePositions();
            UpdateBumperPlot();
        }
        private void LinearBumper2_OnDragDelta_DragDelta(object sender, DragDeltaEventArgs e)
        {
            _linearBumper2LeftPosition = Canvas.GetLeft(LinearBumper2) + e.HorizontalChange;
            _linearBumper2TopPosition = Canvas.GetTop(LinearBumper2) + e.VerticalChange;

            MyNetworkParameters.LinearBumper2 = new[] { _linearBumper2LeftPosition, _linearBumper2TopPosition };

            if ((_linearBumper2LeftPosition > 0) && (_linearBumper2LeftPosition < _screenWidth))
            {
                Canvas.SetLeft(LinearBumper2, _linearBumper2LeftPosition);
            }

            if ((_linearBumper2TopPosition > 0) && (_linearBumper2TopPosition < (0.95 * _screenHeight)))
            {
                Canvas.SetTop(LinearBumper2, _linearBumper2TopPosition);
            }

            // Find distances relative to my tank position
            _newBumper.LinearBumper2LeftPosition = _linearBumper2LeftPosition + 5;
            _newBumper.LinearBumper2TopPosition = _linearBumper2TopPosition + 5;
            UpdatePositions();
            UpdateBumperPlot();
        }

        private void CircularBumper1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            _circularBumper1Left = Canvas.GetLeft(CircularBumper1) + e.HorizontalChange;
            _circularBumper1Top = Canvas.GetTop(CircularBumper1) + e.VerticalChange;

            MyNetworkParameters.CircularBumper1 = new[] { _circularBumper1Left, _circularBumper1Top};

            if ((_circularBumper1Left > 0) && (_circularBumper1Left < _screenWidth))
            {
                Canvas.SetLeft(CircularBumper1, _circularBumper1Left);
            }

            if ((_circularBumper1Top > 0) && (_circularBumper1Top < (0.95 * _screenHeight)))
            {
                Canvas.SetTop(CircularBumper1, _circularBumper1Top);
            }

            // Find distances relative to my tank position
            _newBumper.CircularBumper1Left = _circularBumper1Left + 7.5;
            _newBumper.CircularBumper1Top = _circularBumper1Top + 7.5;
            UpdatePositions();
            UpdateCircularBumperPlot();
        }
        private void CircularBumper2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            _circularBumper2Left = Canvas.GetLeft(CircularBumper2) + e.HorizontalChange;
            _circularBumper2Top = Canvas.GetTop(CircularBumper2) + e.VerticalChange;

            MyNetworkParameters.CircularBumper2 = new[] { _circularBumper2Left, _circularBumper2Top };

            if ((_circularBumper2Left > 0) && (_circularBumper2Left < _screenWidth))
            {
                Canvas.SetLeft(CircularBumper2, _circularBumper2Left);
            }

            if ((_circularBumper2Top > 0) && (_circularBumper2Top < (0.95 * _screenHeight)))
            {
                Canvas.SetTop(CircularBumper2, _circularBumper2Top);
            }

            // Find distances relative to my tank position
            _newBumper.CircularBumper2Left = _circularBumper2Left + 7.5;
            _newBumper.CircularBumper2Top = _circularBumper2Top + 7.5;
            UpdatePositions();
            UpdateCircularBumperPlot();
        }
        private void CircularBumper3_DragDelta(object sender, DragDeltaEventArgs e)
        {
            _circularBumper3Left = Canvas.GetLeft(CircularBumper3) + e.HorizontalChange;
            _circularBumper3Top = Canvas.GetTop(CircularBumper3) + e.VerticalChange;

            MyNetworkParameters.CircularBumper3 = new[] { _circularBumper3Left, _circularBumper3Top };

            if ((_circularBumper3Left > 0) && (_circularBumper3Left < _screenWidth))
            {
                Canvas.SetLeft(CircularBumper3, _circularBumper3Left);
            }

            if ((_circularBumper3Top > 0) && (_circularBumper3Top < (0.95 * _screenHeight)))
            {
                Canvas.SetTop(CircularBumper3, _circularBumper3Top);
            }

            // Find distances relative to my tank position
            _newBumper.CircularBumper3Left = _circularBumper3Left + 7.5;
            _newBumper.CircularBumper3Top = _circularBumper3Top + 7.5;
            UpdatePositions();
            UpdateCircularBumperPlot();
        }

        private void BluePortal_DragDelta(object sender, DragDeltaEventArgs e)
        {
            _bluePortalLeftPosition = Canvas.GetLeft(BluePortal) + e.HorizontalChange;
            _bluePortalTopPosition = Canvas.GetTop(BluePortal) + e.VerticalChange;

            MyNetworkParameters.Portal1 = new[] { _bluePortalLeftPosition, _bluePortalTopPosition };

            if ((_bluePortalLeftPosition > 0) && (_bluePortalLeftPosition < 0.95 * _screenWidth))
            {
                Canvas.SetLeft(BluePortal, _bluePortalLeftPosition);
            }

            if ((_bluePortalTopPosition > 0) && (_bluePortalTopPosition < (0.95 * _screenHeight)))
            {
                Canvas.SetTop(BluePortal, _bluePortalTopPosition);
            }

            // Find distances relative to my tank position
            NewPortal.BlueLeft = _bluePortalLeftPosition;
            NewPortal.BlueTop = _bluePortalTopPosition;
            UpdatePositions();
        }

        private void OrangePortal_DragDelta(object sender, DragDeltaEventArgs e)
        {
            _orangePortalLeftPosition = Canvas.GetLeft(OrangePortal) + e.HorizontalChange;
            _orangePortalTopPosition = Canvas.GetTop(OrangePortal) + e.VerticalChange;

            MyNetworkParameters.Portal2 = new[] { _orangePortalLeftPosition, _orangePortalTopPosition };

            if ((_orangePortalLeftPosition > 0) && (_orangePortalLeftPosition < 0.95 * _screenWidth))
            {
                Canvas.SetLeft(OrangePortal, _orangePortalLeftPosition);
            }

            if ((_orangePortalTopPosition > 0) && (_orangePortalTopPosition < (0.95 * _screenHeight)))
            {
                Canvas.SetTop(OrangePortal, _orangePortalTopPosition);
            }

            // Find distances relative to my tank position
            NewPortal.OrangeLeft = _orangePortalLeftPosition;
            NewPortal.OrangeTop = _orangePortalTopPosition;
            UpdatePositions();
        }

        private void Calibrate_Click(object sender, RoutedEventArgs e)
        {
            double screenWidthInMm = NewWorld.X / NewWorld.ScreenWidthRatio;
            NewWorld.ScreenWidthRatio = 342 / screenWidthInMm;
            UpdatePositions();
            UpdateSuvatPowerTextbox();
        }

        private void ClearScreen_Click(object sender, RoutedEventArgs e)
        {
            PlotCanvas.Children.Clear();
        }

        private void PowerGuessButton_Click(object sender, RoutedEventArgs e)
        {
            _newOde.IVPSolver(NewWorld.X, NewWorld.Y, NewWorld.Theta, NewWorld.Wind, _newPowerGuess);
            PlotGraph();
            _waitedLongEnough = true;
        }

        private void PowerGuessTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(PowerGuessTextbox.Text, out _newPowerGuess))
            {
            }
        }

        private void PowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PowerGuessTextbox.Text = PowerSlider.Value.ToString("0");
            //_newPowerGuess = (int) PowerSlider.Value;
            //PowerGuessButton_Click(null,null);
            if (_waitedLongEnough)
            {
                _newOde.IVPSolver(NewWorld.X, NewWorld.Y, NewWorld.Theta, NewWorld.Wind, _newPowerGuess);
                PlotGraph();
            }
        }
        #endregion
        #region Helper Methods

        private void UpdatePositions()
        {
            // First update tank positions

            // Distances in terms of pixels, and in terms of on screen mm
            _isMirrored = _myNewXPosition > _enemyNewXPosition ? -1 : 1;

            double horizontalPixelDistance = Math.Abs(_enemyNewXPosition - _myNewXPosition);
            double horizontalMmDistance = horizontalPixelDistance * NewWorld.ScreenWidthRatio * World.PixelToMm;
            NewWorld.X = horizontalMmDistance;
            Distance.Text = (NewWorld.X / NewWorld.ScreenWidthRatio).ToString("0.0");

            double verticalPixelDistance = _myNewYPosition - _enemyNewYPosition;
            double verticalMmDistance = verticalPixelDistance * NewWorld.ScreenWidthRatio * World.PixelToMm;
            NewWorld.Y = verticalMmDistance;

            UpdateSuvatPowerTextbox();

            // Now update portal positions
            _newOde.BluePosition = NewPortal.BluePosition(_myNewXPosition, _myNewYPosition,
                NewWorld.ScreenWidthRatio, _isMirrored);
            _newOde.OrangePosition = NewPortal.OrangePosition(_myNewXPosition, _myNewYPosition,
                NewWorld.ScreenWidthRatio, _isMirrored);

            _newBumper.FindAllRelativePositions(_myNewXPosition, _myNewYPosition, NewWorld.ScreenWidthRatio, _isMirrored);
            _newOde.NewBumper = _newBumper;
            _newOde.IsMirrored = _isMirrored;
            //double alpha = _newBumper.CalculateAlpha();
        }

        private void PlotGraph()
        {
            PlotCanvas.Children.Clear();
            Polyline pltemp = new Polyline
            {
                Stroke = Brushes.White,
                StrokeThickness = 2
            };

            List<double[]> currentZ = _newOde.CurrentZ;
            foreach (var stateVector in currentZ)
            {
                double xPoint = _myNewXPosition + _isMirrored * (stateVector[0] / World.PixelToMm / NewWorld.ScreenWidthRatio);
                double yPoint = _myNewYPosition - (stateVector[1] / World.PixelToMm / NewWorld.ScreenWidthRatio);

                Point newPoint = new Point(xPoint, yPoint);
                pltemp.Points.Add(newPoint);
                PlotCanvas.Children.Clear();
                PlotCanvas.Children.Add(pltemp);

            }

            //PlotCanvas.Children.Add(pltemp);
        }

        private void UpdateCircularBumperPlot()
        {
            double x1 = _newBumper.CircularBumper1Left;
            double y1 = _newBumper.CircularBumper1Top;
            double x2 = _newBumper.CircularBumper2Left;
            double y2 = _newBumper.CircularBumper2Top;
            double x3 = _newBumper.CircularBumper3Left;
            double y3 = _newBumper.CircularBumper3Top;

            double C12 = Bumper.CalculateConstantC(x1, y1, x2, y2);
            double C23 = Bumper.CalculateConstantC(x2, y2, x3, y3);
            double D12 = Bumper.CalculateConstantD(x1, y1, x2, y2);
            double D23 = Bumper.CalculateConstantD(x2, y2, x3, y3);

            double b = (C23 - C12) / (D23 - D12);
            double a = C12 - b * D12;
            double r = Math.Sqrt(Math.Pow((x1 - a), 2) + Math.Pow((y1 - b), 2));

            if (!(double.IsNaN(a) || double.IsNaN(b) || double.IsNaN(r) || 
                double.IsInfinity(a) || double.IsInfinity(b) || double.IsInfinity(r)))
            {
                FullCircularBumper.Margin = new Thickness(a - r, b - r, 0, 0);
                FullCircularBumper.Height = 2*r;
                FullCircularBumper.Width = 2*r;
            }
        }

        private void UpdateBumperPlot()
        {
            BumperPlotCanvas.Children.Clear();
            Polyline BumperPlot1 = new Polyline
            {
                Stroke = Brushes.DeepPink,
                StrokeThickness = 2
            };

            BumperPlot1.Points.Add(new Point(_newBumper.LinearBumper1LeftPosition, _newBumper.LinearBumper1TopPosition));
            BumperPlot1.Points.Add(new Point(_newBumper.LinearBumper2LeftPosition, _newBumper.LinearBumper2TopPosition));

            BumperPlotCanvas.Children.Add(BumperPlot1);
        }

        private void UpdateSuvatPowerTextbox()
        {
            if (!_thereIsAngle || !_thereIsDistance || !_thereIsWind) return;
            _newPower = _newSuvatSolver.CalculatePower(NewWorld.X, NewWorld.Y, NewWorld.Theta, NewWorld.Wind);
            Power.Text = _newPower.ToString("0.00");
        }

        #endregion
        private async void BruteForce_Click(object sender, RoutedEventArgs e)
        {
            BruteForce.Visibility = Visibility.Hidden;
            double[] powerAngleGuess = await _newBruteForceMethods.BruteForce(NewWorld.X, NewWorld.Y, NewWorld.Wind);
            BruteForce.Visibility = Visibility.Visible;
            string powerGuess = powerAngleGuess[0].ToString("0");
            string angleGuess = powerAngleGuess[1].ToString("0");

            string over90 = powerAngleGuess[1] > 90 ? " (" + (180 - powerAngleGuess[1]) + ") " : " ";
            BruteForceTextbox.Text = " Power: " + powerGuess + "   Angle: " + angleGuess + over90;
            if (!(double.IsInfinity(powerAngleGuess[0]) || double.IsNaN(powerAngleGuess[0])))
                PlotGraph();
        }

        private void ReboundCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            _newBumper.isRebound = true;
        }

        private void ReboundCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            _newBumper.isRebound = false;
        }

        private void DragCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (_recordingLinear == 0)
            {
                Point p = e.GetPosition(this);
                SetLinearBumper1Position(p.X, p.Y);
                _recordingLinear = 1;
            }
            else if (_recordingLinear == 1)
            {
                Point p = e.GetPosition(this);
                SetLinearBumper2Position(p.X, p.Y);
                _recordingLinear = -1;
                DragCanvas.Background = Brushes.Transparent;
                DragCanvas.Opacity = 1;
            }
            else if (_recordingCircular == 0)
            {
                Point p = e.GetPosition(this);
                SetCircularBumper1Position(p.X, p.Y);
                _recordingCircular = 1;
            }
            else if (_recordingCircular == 1)
            {
                Point p = e.GetPosition(this);
                SetCircularBumper2Position(p.X, p.Y);
                _recordingCircular = 2;
            }
            else if (_recordingCircular == 2)
            {
                Point p = e.GetPosition(this);
                SetCircularBumper3Position(p.X, p.Y);
                _recordingCircular = -1;
                DragCanvas.Background = Brushes.Transparent;
                DragCanvas.Opacity = 1;
            }
            else
            {
                DragCanvas.Background = Brushes.Transparent;
                DragCanvas.Opacity = 1;
            }
        }

        private void SetLinear_Click(object sender, RoutedEventArgs e)
        {
            DragCanvas.Background = Brushes.Black;
            DragCanvas.Opacity = 0.1;
            _recordingLinear = 0;
        }

        private void SetCircular_Click(object sender, RoutedEventArgs e)
        {
            DragCanvas.Background = Brushes.Black;
            DragCanvas.Opacity = 0.1;
            _recordingCircular = 0;
        }

        private void SetLinearBumper1Position(double leftPosition, double topPosition)
        {
            _linearBumper1LeftPosition = leftPosition - 5;
            _linearBumper1TopPosition = topPosition - 5;

            MyNetworkParameters.LinearBumper1 = new[] { _linearBumper1LeftPosition, _linearBumper1TopPosition };

            if ((_linearBumper1LeftPosition > 0) && (_linearBumper1LeftPosition < _screenWidth))
            {
                Canvas.SetLeft(LinearBumper1, _linearBumper1LeftPosition);
            }

            if ((_linearBumper1TopPosition > 0) && (_linearBumper1TopPosition < (0.95 * _screenHeight)))
            {
                Canvas.SetTop(LinearBumper1, _linearBumper1TopPosition);
            }

            // Find distances relative to my tank position
            _newBumper.LinearBumper1LeftPosition = _linearBumper1LeftPosition + 5;
            _newBumper.LinearBumper1TopPosition = _linearBumper1TopPosition + 5;
            UpdatePositions();
            UpdateBumperPlot();
        }
        private void SetLinearBumper2Position(double leftPosition, double topPosition)
        {
            _linearBumper2LeftPosition = leftPosition - 5;
            _linearBumper2TopPosition = topPosition - 5;

            MyNetworkParameters.LinearBumper2 = new[] { _linearBumper2LeftPosition, _linearBumper2TopPosition };

            if ((_linearBumper2LeftPosition > 0) && (_linearBumper2LeftPosition < _screenWidth))
            {
                Canvas.SetLeft(LinearBumper2, _linearBumper2LeftPosition);
            }

            if ((_linearBumper2TopPosition > 0) && (_linearBumper2TopPosition < (0.95 * _screenHeight)))
            {
                Canvas.SetTop(LinearBumper2, _linearBumper2TopPosition);
            }

            // Find distances relative to my tank position
            _newBumper.LinearBumper2LeftPosition = _linearBumper2LeftPosition + 5;
            _newBumper.LinearBumper2TopPosition = _linearBumper2TopPosition + 5;
            UpdatePositions();
            UpdateBumperPlot();
        }

        private void SetCircularBumper1Position(double leftPosition, double topPosition)
        {
            _circularBumper1Left = leftPosition - 7.5;
            _circularBumper1Top = topPosition - 7.5;

            MyNetworkParameters.CircularBumper1 = new[] { _circularBumper1Left, _circularBumper1Top };

            if ((_circularBumper1Left > 0) && (_circularBumper1Left < _screenWidth))
            {
                Canvas.SetLeft(CircularBumper1, _circularBumper1Left);
            }

            if ((_circularBumper1Top > 0) && (_circularBumper1Top < (0.95 * _screenHeight)))
            {
                Canvas.SetTop(CircularBumper1, _circularBumper1Top);
            }

            // Find distances relative to my tank position
            _newBumper.CircularBumper1Left = _circularBumper1Left + 7.5;
            _newBumper.CircularBumper1Top = _circularBumper1Top + 7.5;
            UpdatePositions();
            UpdateCircularBumperPlot();
        }
        private void SetCircularBumper2Position(double leftPosition, double topPosition)
        {
            _circularBumper2Left = leftPosition - 7.5;
            _circularBumper2Top = topPosition - 7.5;

            MyNetworkParameters.CircularBumper2 = new[] { _circularBumper2Left, _circularBumper2Top };

            if ((_circularBumper2Left > 0) && (_circularBumper2Left < _screenWidth))
            {
                Canvas.SetLeft(CircularBumper2, _circularBumper2Left);
            }

            if ((_circularBumper2Top > 0) && (_circularBumper2Top < (0.95 * _screenHeight)))
            {
                Canvas.SetTop(CircularBumper2, _circularBumper2Top);
            }

            // Find distances relative to my tank position
            _newBumper.CircularBumper2Left = _circularBumper2Left + 7.5;
            _newBumper.CircularBumper2Top = _circularBumper2Top + 7.5;
            UpdatePositions();
            UpdateCircularBumperPlot();
        }
        private void SetCircularBumper3Position(double leftPosition, double topPosition)
        {
            _circularBumper3Left = leftPosition - 7.5;
            _circularBumper3Top = topPosition - 7.5;

            MyNetworkParameters.CircularBumper3 = new[] { _circularBumper3Left, _circularBumper3Top };

            if ((_circularBumper3Left > 0) && (_circularBumper3Left < _screenWidth))
            {
                Canvas.SetLeft(CircularBumper3, _circularBumper3Left);
            }

            if ((_circularBumper3Top > 0) && (_circularBumper3Top < (0.95 * _screenHeight)))
            {
                Canvas.SetTop(CircularBumper3, _circularBumper3Top);
            }

            // Find distances relative to my tank position
            _newBumper.CircularBumper3Left = _circularBumper3Left + 7.5;
            _newBumper.CircularBumper3Top = _circularBumper3Top + 7.5;
            UpdatePositions();
            UpdateCircularBumperPlot();
        }

        private void ClearBumpers_Click(object sender, RoutedEventArgs e)
        {
            // First set positions
            SetLinearBumper1Position(10, 10);
            SetLinearBumper2Position(10, 10);
            SetCircularBumper1Position(10, 10);
            SetCircularBumper2Position(20, 10);
            SetCircularBumper3Position(15, 20);

            // Now set unrealistic positions
            SetLinearBumper1Position(-1000, -1000);
            SetLinearBumper2Position(-1000, -1000);
            SetCircularBumper1Position(-1000, -1000);
            SetCircularBumper2Position(-2000, -1000);
            SetCircularBumper3Position(-1500, -2000);
        }

        //private void Gravity_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (double.TryParse(Gravity.Text, out gravityConstant))
        //    {
        //        _newOde._g = (gravityConstant);
        //        UpdateSuvatPowerTextbox();
        //    }
        //}
        //private void WindConstant_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (double.TryParse(WindConstant.Text, out windConstant))
        //    {
        //        _newOde._windConstant = (windConstant);
        //        UpdateSuvatPowerTextbox();
        //    }
        //}
    }
}

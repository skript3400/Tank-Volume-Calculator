using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Math;


namespace Tank_Volume_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    

    public partial class MainWindow : Window
    {
        private static  readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private InputVariable _diameter;
        private InputVariable _length;
        private InputVariable _maxVolume;
        private InputVariable _volume;
        private InputVariable _height;
        private InputVariable _volumeRatio;


        public MainWindow()
        {
            InitializeComponent();
            _diameter = new InputVariable(this.DiameterInput1, 0.01);
            _length = new InputVariable(this.LengthInput1, 0.01);
            _maxVolume = new InputVariable(this.MaxVolumeInput, 0.001);
            _volume = new InputVariable(this.VolumeInput, 0.001);
            _height = new InputVariable(this.WaterHeightInput, 0.01);
            _volumeRatio = new InputVariable(this.VolumeRatioInput, 0.01);
        }



        //left calculate button click handler
        private void DimensionsCalc(object sender, RoutedEventArgs e)
        {
            if (_diameter.isValid && _length.isValid)
            {
                getMaxVol();
                return;
            }
            if (_length.isValid && _maxVolume.isValid)
            {
                getDiameter();
                return;
            }
            if (_diameter.isValid && _maxVolume.isValid)
            {
                getLength();
                return;
            }
        }

        //right calculate button click handler
        private void VolumeHeightCalc(object sender, RoutedEventArgs e)
        {
            
            if(_diameter.isValid && _length.isValid && _height.isValid)
            {
                //calculate Volume from height (analytical solution)

                double l = _length.Value;
                double h = _height.Value;
                double r = _diameter.Value / 2;
                _volume.Value = CalcTankVolume(r, h, l);
                getMaxVol();
                _volumeRatio.Value = _volume.Value / _maxVolume.Value;
                return;
            }

            if (_diameter.isValid && _length.isValid && _volume.isValid)
            {
                //calculate Height from Volume (numerical aprox.)

                double l = _length.Value;
                double v = _volume.Value;
                double d = _diameter.Value;
                _height.Value = SolveWaterHight(d,l,v);
                getMaxVol();
                _volumeRatio.Value = v/_maxVolume.Value;
                return;
            }
            if(_diameter.isValid && _length.isValid && _volumeRatio.isValid)
            {
                double d = _diameter.Value;
                double l = _length.Value;
                getMaxVol();
                double v = _maxVolume.Value * _volumeRatio.Value;
                _height.Value = SolveWaterHight(d, l, v);
                _volume.Value = v;
            }

        }



        //---Simple Cylinder Calculations

        //cylinder volume
        private void getMaxVol()
        {
            double r = _diameter.Value / 2;
            double l = _length.Value;

            _maxVolume.Value = Pow(r, 2) * PI * l;
        }
        //cylinder diameter
        private void getDiameter()
        {
            double v = _maxVolume.Value;
            double l = _length.Value;

            _diameter.Value = 2 * Sqrt(v / (l * PI));
        }
        //cylinder length
        private void getLength()
        {
            double v = _maxVolume.Value;
            double r = _diameter.Value / 2;

            _length.Value = v / (Pow(r, 2) * PI);
        }

        //Lying Cylinder Height Volume Caclulations

        //calculate volume from height
        private double CalcTankVolume(double radius, double fillHeight, double length)
        {
            double segmentArea = radius * radius * Acos((radius - fillHeight) / radius) - (radius - fillHeight) * Sqrt(2 * radius * fillHeight - fillHeight * fillHeight);
            double volume = segmentArea * length;

            return volume;
        }
        //volume volume from fill ratio ratio 

        

        //numerical aprox. height for given volume
        private double SolveWaterHight(double diameter, double length, double volume)
        {
            double radius = diameter / 2.0;
            double epsilon = 1e-9;
            double hMin = 0.0;
            double hMax = diameter;

            while (hMax - hMin > epsilon)
            {
                double hMid = (hMin + hMax) / 2.0;
                double calculatedVolume = CalcTankVolume(radius, hMid, length);

                if (Abs(calculatedVolume - volume) <= epsilon)
                {
                    return hMid;
                }
                else if (calculatedVolume < volume)
                {
                    hMin = hMid;
                }
                else
                {
                    hMax = hMid;
                }
            }

            return (hMin + hMax) / 2.0;
        }

      
    }
}

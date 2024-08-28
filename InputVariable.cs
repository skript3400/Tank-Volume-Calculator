using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Tank_Volume_Calculator
{   

    internal class InputVariable
    {
        private TextBox _textBox;
        private double _siConversion;
        private double _value = 0;
        public bool isValid
        {
            get { return Double.TryParse(_textBox.Text, out _value); }
        }

        public double Value { 
            get { 
                Double.TryParse(_textBox.Text, out _value);
                return _value * _siConversion; 
            } 
            set {
                _value = value / _siConversion;
                _textBox.Text = Math.Round(_value,5).ToString();
            } 
        }
        public InputVariable(TextBox textBox, double siConversion)
        {
            _siConversion = siConversion;
            _textBox = textBox;
        }
    }
}

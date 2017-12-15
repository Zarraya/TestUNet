using UnityEngine;
using System;
using System.Globalization;


namespace Noesis.Samples
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class CapsConverter : Noesis.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ViewModel : NotifyPropertyChangedBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Input { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private string _output = string.Empty;
        public string Output
        {
            get { return _output; }
            set
            {
                if (_output != value)
                {
                    _output = value;
                    OnPropertyChanged("Output");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Noesis.Samples.DelegateCommand SayHelloCommand { get; private set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ViewModel()
        {
            SayHelloCommand = new Noesis.Samples.DelegateCommand(SayHello);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SayHello(object parameter)
        {
            string param = (string)parameter;
            Output = System.String.Format("Hello, {0} ({1})", Input, param);
        }
    }
}

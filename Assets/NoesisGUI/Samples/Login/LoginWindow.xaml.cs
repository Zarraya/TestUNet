#if UNITY_5 || UNITY_5_3_OR_NEWER
#define NOESIS
using Noesis;
#else
using System;
using System.Windows;
using System.Windows.Controls;
#endif

namespace Noesis.Samples
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
#if NOESIS
    [UserControlSource("Assets/NoesisGUI/Samples/Login/LoginWindow.xaml")]
#endif
    public partial class LoginWindow : UserControl
    {
        public LoginWindow()
        {
            this.Initialized += OnInitialized;
            this.InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
            this.DataContext = new LoginViewModel();
        }
    }
}

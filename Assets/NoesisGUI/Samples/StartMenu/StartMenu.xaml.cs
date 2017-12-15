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
    /// Interaction logic for StartMenu.xaml
    /// </summary>
#if NOESIS
    [UserControlSource("Assets/NoesisGUI/Samples/StartMenu/StartMenu.xaml")]
#endif
    public partial class StartMenu : UserControl
    {
        public StartMenu()
        {
            this.Initialized += OnInitialized;
            this.InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
            this.DataContext = new StartMenuViewModel();
        }
    }
}

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
    /// Interaction logic for QuestLogWindow.xaml
    /// </summary>
#if NOESIS
    [UserControlSource("Assets/NoesisGUI/Samples/QuestLog/QuestLogWindow.xaml")]
#endif
    public partial class QuestLogWindow : UserControl
    {
        public QuestLogWindow()
        {
            this.Initialized += OnInitialized;
            this.InitializeComponent();
        }

        private void OnInitialized(object sender, EventArgs args)
        {
            this.DataContext = new QuestLogViewModel((ResourceDictionary)Resources.MergedDictionaries[0]);
        }
    }
}

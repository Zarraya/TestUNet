using UnityEngine;
using System.Collections;

public class ControlGalleryLogic : MonoBehaviour
{
    Noesis.ComboBox _styleSelector;
    Noesis.ResourceDictionary _noesisStyleResources;
    Noesis.ResourceDictionary _simpleStyleResources;
    Noesis.ResourceDictionary _windowsStyleResources;

    Noesis.Grid _selector;
    Noesis.TreeView _sampleSelector;
    string _lastSample;

    Noesis.Viewbox _sampleContainer;
    Noesis.Border _sampleContainer1;
    Noesis.Border _sampleContainer2;
    Noesis.Grid _sampleOverlay;

    Noesis.Storyboard _showContainer1;
    Noesis.Storyboard _showContainer2;

    Noesis.Border _selectorTopContainer;
    Noesis.StackPanel _selectorTop;
    Noesis.ToggleButton _selectorTopExpand;

    Noesis.Border _selectorLeftContainer;
    Noesis.StackPanel _selectorLeft;
    Noesis.ToggleButton _selectorLeftExpand;

    Noesis.Decorator _itemHeight;

    void Start()
    {
        var gui = GetComponent<NoesisGUIPanel>();
        var content = gui.GetContent();
        content.SizeChanged += OnSizeChanged;

        _styleSelector = (Noesis.ComboBox)content.FindName("StyleSelector");
        _styleSelector.SelectionChanged += OnStyleSelectionChanged;

        _noesisStyleResources = (Noesis.ResourceDictionary)NoesisGUISystem.LoadXaml("Assets/NoesisGUI/Themes/NoesisStyle.xaml");
        _simpleStyleResources = (Noesis.ResourceDictionary)NoesisGUISystem.LoadXaml("Assets/NoesisGUI/Themes/SimpleStyle.xaml");
        _windowsStyleResources = (Noesis.ResourceDictionary)NoesisGUISystem.LoadXaml("Assets/NoesisGUI/Themes/WindowsStyle.xaml");

        _selector = (Noesis.Grid)content.FindName("Selector");
        _sampleSelector = (Noesis.TreeView)content.FindName("SampleSelector");
        _sampleSelector.SelectedItemChanged += OnSamplesSelectionChanged;

        _sampleContainer = (Noesis.Viewbox)content.FindName("SampleContainer");
        _sampleContainer1 = (Noesis.Border)content.FindName("SampleContainer1");
        _sampleContainer2 = (Noesis.Border)content.FindName("SampleContainer2");

        _sampleOverlay = (Noesis.Grid)content.FindName("SampleOverlay");
        _sampleOverlay.MouseDown += OnSampleOverlayMouseDown;

        _showContainer1 = (Noesis.Storyboard)content.Resources["ShowContainer1"];
        _showContainer2 = (Noesis.Storyboard)content.Resources["ShowContainer2"];

        _selectorTopContainer = (Noesis.Border)content.FindName("SelectorTopContainer");
        _selectorTop = (Noesis.StackPanel)content.FindName("SelectorTop");
        _selectorTopExpand = (Noesis.ToggleButton)content.FindName("SelectorTopExpand");

        _selectorLeftContainer = (Noesis.Border)content.FindName("SelectorLeftContainer");
        _selectorLeft = (Noesis.StackPanel)content.FindName("SelectorLeft");
        _selectorLeftExpand = (Noesis.ToggleButton)content.FindName("SelectorLeftExpand");

        _itemHeight = (Noesis.Decorator)content.FindName("ItemHeight");
    }

    void OnStyleSelectionChanged(object sender, Noesis.SelectionChangedEventArgs args)
    {
        switch (_styleSelector.SelectedIndex)
        {
            case 0:
                _sampleContainer.Resources = _noesisStyleResources;
                break;

            case 1:
                _sampleContainer.Resources = _simpleStyleResources;
                break;

            case 2:
                _sampleContainer.Resources = _windowsStyleResources;
                break;
        }

        args.Handled = true;
    }

    void OnSamplesSelectionChanged(object oldValue, object newValue)
    {
        Noesis.TreeViewItem tvi = (Noesis.TreeViewItem)newValue;
        if (tvi != null && !tvi.HasItems)
        {
            string sampleName = (string)tvi.Tag;
            if (_lastSample != sampleName)
            {
                LoadSample(sampleName);
                _lastSample = sampleName;
            }
        }
    }

    void LoadSample(string sampleName)
    {
        _sampleSelector.IsEnabled = false;

        Noesis.UIElement sample = (Noesis.UIElement)NoesisGUISystem.LoadXaml(sampleName);

        if (_sampleContainer1.Child == null)
        {
            // Show container 1
            _sampleContainer1.Child = sample;
            _showContainer1.Begin();
            _showContainer1.Completed += OnShowSampleCompleted;
        }
        else
        {
            // Show container 2
            _sampleContainer2.Child = sample;
            _showContainer2.Begin();
            _showContainer2.Completed += OnShowSampleCompleted;
        }
    }

    void OnShowSampleCompleted(object sender, Noesis.TimelineEventArgs e)
    {
        if ((Noesis.Storyboard)sender == _showContainer1)
        {
            // Container 1 shown
            _showContainer1.Completed -= OnShowSampleCompleted;
            _sampleContainer2.Child = null;
        }
        else
        {
            // Container 2 shown
            _showContainer2.Completed -= OnShowSampleCompleted;
            _sampleContainer1.Child = null;
        }

        _sampleSelector.IsEnabled = true;
    }

    void OnSizeChanged(object sender, Noesis.SizeChangedEventArgs e)
    {
        Noesis.Size newSize = e.NewSize;
        if (newSize.Width > newSize.Height)
        {
            // Landscape
            _selectorTopContainer.Child = null;
            _selectorLeftContainer.Child = _selector;
            _selectorTop.Visibility = Noesis.Visibility.Collapsed;
            _selectorLeft.Visibility = Noesis.Visibility.Visible;
            _selectorTopExpand.IsChecked = false;
            _itemHeight.Height = newSize.Width * 0.05f;
        }
        else
        {
            // Portrait
            _selectorLeftContainer.Child = null;
            _selectorTopContainer.Child = _selector;
            _selectorLeft.Visibility = Noesis.Visibility.Collapsed;
            _selectorTop.Visibility = Noesis.Visibility.Visible;
            _selectorLeftExpand.IsChecked = false;
            _itemHeight.Height = newSize.Height * 0.05f;
        }
    }

    void OnSampleOverlayMouseDown(object sender, Noesis.MouseButtonEventArgs e)
    {
        _selectorLeftExpand.IsChecked = false;
        _selectorTopExpand.IsChecked = false;
    }
}

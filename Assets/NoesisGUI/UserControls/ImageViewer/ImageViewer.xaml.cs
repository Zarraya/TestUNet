#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_IPHONE || UNITY_METRO_8_1

using System;
using System.Collections.Generic;
using Noesis;

namespace Noesis.UserControls
{

public class ImageItem : System.ComponentModel.INotifyPropertyChanged
{
    public ImageSource Source { get; set; }

    private float _scale;
    public float Scale
    {
        get { return _scale; }
        set
        {
            if (_scale != value)
            {
                _scale = value;
                OnPropertyChanged("Scale");
            }
        }
    }

    private float _dim;
    public float Dim
    {
        get { return _dim; }
        set
        {
            if (_dim != value)
            {
                _dim = value;
                OnPropertyChanged("Dim");
            }
        }
    }

    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string name)
    {
        System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new System.ComponentModel.PropertyChangedEventArgs(name));
        }
    }
}

[UserControlSource("Assets/NoesisGUI/UserControls/ImageViewer/ImageViewer.xaml")]
public class ImageViewer : UserControl
{
    public List<ImageSource> Images { get; set; }

    public List<ImageItem> ImageItems { get; private set; }

    public static DependencyProperty OffsetProperty = DependencyProperty.Register(
        "Offset", typeof(float), typeof(ImageViewer),
        new PropertyMetadata(0.0f, OnOffsetChanged));

    public float Offset
    {
        get { return (float)GetValue(OffsetProperty); }
        set { SetValue(OffsetProperty, value); }
    }

    TranslateTransform _trans;
    Storyboard _bounce;
    EasingDoubleKeyFrame _bounceFrom;
    EasingDoubleKeyFrame _bounceMid;
    EasingDoubleKeyFrame _bounceTo;

    float _totalWidth;

    public ImageViewer()
    {
    }

    public void OnPostInit()
    {
        ImageItems = new List<ImageItem>();
        foreach (ImageSource source in Images)
        {
            ImageItems.Add(new ImageItem { Source = source, Scale = 1.0f, Dim = 0.0f });
        }

        _totalWidth = UpdateScale(0.0f);

        var touchPanel = (Grid)FindName("TouchPanel");
        touchPanel.ManipulationStarting += OnManipulationStarting;
        touchPanel.ManipulationInertiaStarting += OnManipulationInertiaStarting;
        touchPanel.ManipulationDelta += OnManipulationDelta;
        touchPanel.ManipulationCompleted += OnManipulationCompleted;

        _trans = (TranslateTransform)touchPanel.RenderTransform;

        _bounce = (Storyboard)Resources["Bounce"];
        var anim = (DoubleAnimationUsingKeyFrames)_bounce.Children[0];
        _bounceFrom = (EasingDoubleKeyFrame)anim.KeyFrames[0];
        _bounceMid = (EasingDoubleKeyFrame)anim.KeyFrames[1];
        _bounceTo = (EasingDoubleKeyFrame)anim.KeyFrames[2];

        var root = (Grid)FindName("LayoutRoot");
        root.DataContext = this;
    }

    void OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
    {
        e.Mode = Noesis.ManipulationModes.TranslateX;
        e.Handled = true;
    }

    void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
    {
        e.TranslationBehavior.DesiredDeceleration = 0.001f;
        e.Handled = true;
    }

    void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
        var norm = (float)Math.Pow(0.6, Math.Max(0.0, -_trans.X) / 510.0);
        var delta = e.DeltaManipulation.Translation.X * norm;
        
        if (!e.IsInertial)
        {
            if (_trans.X < 500.0f * 0.2f &&
                _trans.X > -(_totalWidth + 500.0f * 0.05f))
            {
                _trans.X += delta;
            }
        }
        else
        {
            _trans.X += delta;

            if (_trans.X > 0.0f)
            {
                e.Complete();
            }
            else if (_trans.X < -_totalWidth)
            {
                e.Complete();
            }
        }

        UpdateScale(-_trans.X);

        e.Handled = true;
    }

    void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
        Grid panel = (Grid)e.ManipulationContainer;
        var trans = (TranslateTransform)panel.RenderTransform;

        if (trans.X > 0.0f)
        {
            _bounceFrom.Value = trans.X;
            _bounceMid.Value = trans.X - e.FinalVelocities.LinearVelocity.X;
            _bounceTo.Value = 0.0f;
            _bounce.Begin(this);
        }
        else if (trans.X < -_totalWidth)
        {
            _bounceFrom.Value = trans.X;
            _bounceMid.Value = trans.X - e.FinalVelocities.LinearVelocity.X;
            _bounceTo.Value = -_totalWidth;
            _bounce.Begin(this);
        }

        UpdateScale(-trans.X);
    }

    static void OnOffsetChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        ImageViewer imageViewer = sender as ImageViewer;
        if (imageViewer != null)
        {
            imageViewer._trans.X = (float)e.NewValue;
            imageViewer.UpdateScale(-imageViewer._trans.X);
        }
    }

    float UpdateScale(float offset)
    {
        float numImages = (float)ImageItems.Count;
        float imageSize = 500;
        float margin = 10;
        float imageWithMargin = imageSize + margin;
        float accumWidth = 0;
        float lastWidth = 0;
        int i = 0;
        foreach (ImageItem image in ImageItems)
        {
            if (i == 0 && offset < 0)
            {
                float distance = -offset / imageWithMargin;
                image.Scale = 1.0f + 3.0f * distance / numImages;
                image.Dim = 0.0f;
            }
            else if (i == (ImageItems.Count - 1) && offset > _totalWidth)
            {
                float distance = (offset - _totalWidth) / imageWithMargin;
                image.Scale = 1.0f + 8.0f * distance / numImages;
                image.Dim = 0.0f;
            }
            else
            {
                float distance = Math.Abs(offset - accumWidth) / imageWithMargin;
                image.Scale = Math.Max(0.1f, 1.0f - (3.0f * distance / numImages));
                image.Dim = 1.0f - (float)Math.Pow(image.Scale, 4.0f);
            }

            lastWidth = imageSize * image.Scale + margin;
            accumWidth += lastWidth;
            ++i;
        }

        return accumWidth - imageWithMargin / 2.7f;
    }
}

}

#else

namespace Noesis.UserControls
{
    public class ImageViewer : System.Windows.Controls.UserControl
    {
        public ImageViewer()
        {
            InitializeComponent();
        }
    }
}

#endif

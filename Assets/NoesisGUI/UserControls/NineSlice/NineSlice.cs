using System;
using System.Globalization;
using System.ComponentModel;
using UnityEngine;

namespace Noesis.UserControls
{

////////////////////////////////////////////////////////////////////////////////////////////////////
[Noesis.UserControlSource("Assets/NoesisGUI/UserControls/NineSlice/NineSlice.xaml")]
public class NineSlice : Noesis.UserControl, INotifyPropertyChanged
{
    #region Public properties

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public static Noesis.DependencyProperty ImageProperty = Noesis.DependencyProperty.Register(
        "Image", typeof(Noesis.ImageSource), typeof(NineSlice),
        new Noesis.PropertyMetadata(null, new Noesis.PropertyChangedCallback(OnImageChanged)));

    public Noesis.ImageSource Image
    {
        get { return (Noesis.ImageSource)GetValue(ImageProperty); }
        set { SetValue(ImageProperty, value); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public static Noesis.DependencyProperty LeftSliceProperty = Noesis.DependencyProperty.Register(
        "LeftSlice", typeof(float), typeof(NineSlice),
        new Noesis.PropertyMetadata(0.0f, new Noesis.PropertyChangedCallback(OnSliceChanged)));

    public float LeftSlice
    {
        get { return (float)GetValue(LeftSliceProperty); }
        set { SetValue(LeftSliceProperty, value); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public static Noesis.DependencyProperty RightSliceProperty = Noesis.DependencyProperty.Register(
        "RightSlice", typeof(float), typeof(NineSlice),
        new Noesis.PropertyMetadata(0.0f, new Noesis.PropertyChangedCallback(OnSliceChanged)));

    public float RightSlice
    {
        get { return (float)GetValue(RightSliceProperty); }
        set { SetValue(RightSliceProperty, value); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public static Noesis.DependencyProperty TopSliceProperty = Noesis.DependencyProperty.Register(
        "TopSlice", typeof(float), typeof(NineSlice),
        new Noesis.PropertyMetadata(0.0f, new Noesis.PropertyChangedCallback(OnSliceChanged)));

    public float TopSlice
    {
        get { return (float)GetValue(TopSliceProperty); }
        set { SetValue(TopSliceProperty, value); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public static Noesis.DependencyProperty BottomSliceProperty = Noesis.DependencyProperty.Register(
        "BottomSlice", typeof(float), typeof(NineSlice),
        new Noesis.PropertyMetadata(0.0f, new Noesis.PropertyChangedCallback(OnSliceChanged)));

    public float BottomSlice
    {
        get { return (float)GetValue(BottomSliceProperty); }
        set { SetValue(BottomSliceProperty, value); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public static Noesis.DependencyProperty ShowSlicesProperty = Noesis.DependencyProperty.Register(
        "ShowSlices", typeof(bool), typeof(NineSlice), new Noesis.PropertyMetadata(false));

    public bool ShowSlices
    {
        get { return (bool)GetValue(ShowSlicesProperty); }
        set { SetValue(ShowSlicesProperty, value); }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public Noesis.Rect TopLeftViewbox { get; private set; }
    public Noesis.Rect TopMiddleViewbox { get; private set; }
    public Noesis.Rect TopRightViewbox { get; private set; }
    public Noesis.Rect MiddleLeftViewbox { get; private set; }
    public Noesis.Rect MiddleMiddleViewbox { get; private set; }
    public Noesis.Rect MiddleRightViewbox { get; private set; }
    public Noesis.Rect BottomLeftViewbox { get; private set; }
    public Noesis.Rect BottomMiddleViewbox { get; private set; }
    public Noesis.Rect BottomRightViewbox { get; private set; }

    #endregion

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChange(string propertyName)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion

    #region Internal Logic

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public void OnPostInit()
    {
        UpdateSlices();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private static void OnSliceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        NineSlice nineSlice = d as NineSlice;
        if (nineSlice != null)
        {
            nineSlice.UpdateSlices();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        NineSlice nineSlice = d as NineSlice;
        if (nineSlice != null)
        {
            nineSlice.UpdateSlices();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    void UpdateSlices()
    {
        if (Image == null || Math.Abs(Image.Width) < Single.Epsilon || Math.Abs(Image.Height) < Single.Epsilon)
        {
            return;
        }

        float leftSlice = LeftSlice;
        float rightSlice = RightSlice;
        float topSlice = TopSlice;
        float bottomSlice = BottomSlice;

        float rightStart = Image.Width - rightSlice;
        float middleWidth = rightStart - leftSlice;

        float bottomStart = Image.Height - bottomSlice;
        float middleHeight = bottomStart - topSlice;

        // Update image brushes texture coordinates

        TopLeftViewbox = new Noesis.Rect(0, 0, leftSlice, topSlice);
        TopMiddleViewbox = new Noesis.Rect(leftSlice, 0, middleWidth, topSlice);
        TopRightViewbox = new Noesis.Rect(rightStart, 0, rightSlice, topSlice);

        MiddleLeftViewbox = new Noesis.Rect(0, topSlice, leftSlice, middleHeight);
        MiddleMiddleViewbox = new Noesis.Rect(leftSlice, topSlice, middleWidth, middleHeight);
        MiddleRightViewbox = new Noesis.Rect(rightStart, topSlice, rightSlice, middleHeight);

        BottomLeftViewbox = new Noesis.Rect(0, bottomStart, leftSlice, bottomSlice);
        BottomMiddleViewbox = new Noesis.Rect(leftSlice, bottomStart, middleWidth, bottomSlice);
        BottomRightViewbox = new Noesis.Rect(rightStart, bottomStart, rightSlice, bottomSlice);

        NotifyPropertyChange("TopLeftViewbox");
        NotifyPropertyChange("TopMiddleViewbox");
        NotifyPropertyChange("TopRightViewbox");

        NotifyPropertyChange("MiddleLeftViewbox");
        NotifyPropertyChange("MiddleMiddleViewbox");
        NotifyPropertyChange("MiddleRightViewbox");

        NotifyPropertyChange("BottomLeftViewbox");
        NotifyPropertyChange("BottomMiddleViewbox");
        NotifyPropertyChange("BottomRightViewbox");
    }

    #endregion
}

}

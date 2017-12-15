using UnityEngine;

namespace Noesis.Samples
{

public class Touch : MonoBehaviour
{
    private Noesis.Grid _root;
    private bool _completeManipulation = false;

    void Start()
    {
        _root = (Noesis.Grid)GetComponent<NoesisGUIPanel>().GetContent();

        var list = (Noesis.ListBox)_root.FindName("list");
        list.SelectionChanged += this.SelectionChanged;

        _root.ManipulationStarting += this.ManipulationStarting;
        _root.ManipulationInertiaStarting += this.ManipulationInertiaStarting;
        _root.ManipulationDelta += this.ManipulationDelta;
    }

    void SelectionChanged(object sender, Noesis.SelectionChangedEventArgs e)
    {
        var list = (Noesis.ListBox)sender;
        var image = (Noesis.Image)list.SelectedItem;
        var canvas = (Noesis.Image)_root.FindName("canvas");

        // Activate selected image and reset matrix
        canvas.Source = image.Source;
        var transform = (Noesis.MatrixTransform)canvas.RenderTransform;
        transform.Matrix = Noesis.Transform2.Identity;

        // Stop manipulation from last image
        _completeManipulation = true;

        e.Handled = true;
    }

    void ManipulationStarting(object sender, Noesis.ManipulationStartingEventArgs e)
    {
        _completeManipulation = false;
        e.Mode = Noesis.ManipulationModes.All;
        e.ManipulationContainer = (Noesis.UIElement)_root.FindName("root");
        e.Handled = true;
    }

    void ManipulationInertiaStarting(object sender, Noesis.ManipulationInertiaStartingEventArgs e)
    {
        e.TranslationBehavior.DesiredDeceleration = 100.0f / (1000.0f * 1000.0f);
        e.RotationBehavior.DesiredDeceleration = 360.0f / (1000.0f * 1000.0f);
        e.ExpansionBehavior.DesiredDeceleration = 300.0f / (1000.0f * 1000.0f);
        e.Handled = true;
    }

    void ManipulationDelta(object sender, Noesis.ManipulationDeltaEventArgs e)
    {
        if (_completeManipulation)
        {
            e.Complete();
            e.Handled = true;
            return;
        }

        var image = (Noesis.Image)e.Source;
        var transform = (Noesis.MatrixTransform)image.RenderTransform;
        var matrix = transform.Matrix;

        Noesis.Point origin = e.ManipulationOrigin;
        float originX = origin.X;
        float originY = origin.Y;

        Noesis.ManipulationDelta delta = e.DeltaManipulation;
        float rotation = delta.Rotation * Mathf.Deg2Rad;
        float scale = FilterScale(matrix, delta.Scale);
        float translationX = delta.Translation.X;
        float translationY = delta.Translation.Y;

        matrix.RotateAt(rotation, originX, originY);
        matrix.ScaleAt(scale, scale, originX, originY);
        matrix.Translate(translationX, translationY);

        transform.Matrix = matrix;
        e.Handled = true;
    }

    // Limit the scale and apply damping on the limits
    float FilterScale(Noesis.Transform2 matrix, float scale)
    {
        float currentScaleX = Noesis.Point.Length(matrix[0]);
        float currentScaleY = Noesis.Point.Length(matrix[1]);
        float currentScale = Mathf.Max(currentScaleX, currentScaleY);

        if (currentScale > 3.0f && scale > 1.0f)
        {
            return 1.0f + (scale - 1.0f) / Mathf.Pow(2.0f, currentScale - 3.0f);
        }
        else if (currentScale < 1.0f && scale < 1.0f)
        {
            return 1.0f + (scale - 1.0f) / Mathf.Pow(2.0f, 10.0f * (1.0f / currentScale - 1.0f));
        }
        else
        {
            return scale;
        }
    }
}

}

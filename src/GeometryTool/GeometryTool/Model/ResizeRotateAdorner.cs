using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GeometryTool;

/// <summary>
///     拖动边的Adorner
/// </summary>
public class ResizeRotateAdorner : Adorner
{
    private readonly ResizeRotateChrome chrome;
    private readonly VisualCollection visuals;

    public ResizeRotateAdorner(ContentControl borderWA)
        : base(borderWA)
    {
        SnapsToDevicePixels = true;
        chrome = new ResizeRotateChrome();
        chrome.DataContext = borderWA;
        visuals = new VisualCollection(this);
        visuals.Add(chrome);
    }

    protected override int VisualChildrenCount => visuals.Count;

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        chrome.Arrange(new Rect(arrangeBounds));
        return arrangeBounds;
    }

    protected override Visual GetVisualChild(int index)
    {
        return visuals[index];
    }
}
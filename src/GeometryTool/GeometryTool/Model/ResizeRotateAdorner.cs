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
    private readonly ResizeRotateChrome _chrome;
    private readonly VisualCollection _visuals;

    public ResizeRotateAdorner(ContentControl borderWithAdorner)
        : base(borderWithAdorner)
    {
        SnapsToDevicePixels = true;
        _chrome = new ResizeRotateChrome();
        _chrome.DataContext = borderWithAdorner;
        _visuals = new VisualCollection(this);
        _visuals.Add(_chrome);
    }

    protected override int VisualChildrenCount => _visuals.Count;

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        _chrome.Arrange(new Rect(arrangeBounds));
        return arrangeBounds;
    }

    protected override Visual GetVisualChild(int index)
    {
        return _visuals[index];
    }
}
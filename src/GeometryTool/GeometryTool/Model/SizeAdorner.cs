using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace GeometryTool;

public class SizeAdorner : Adorner
{
    private readonly SizeChrome _chrome;
    private readonly VisualCollection _visuals;

    public SizeAdorner(BorderWithAdorner borderWithAdorner)
        : base(borderWithAdorner)
    {
        SnapsToDevicePixels = true;
        _chrome = new SizeChrome();
        _chrome.DataContext = borderWithAdorner;
        _visuals = new VisualCollection(this);
        _visuals.Add(_chrome);
    }

    protected override int VisualChildrenCount => _visuals.Count;

    protected override Visual GetVisualChild(int index)
    {
        return _visuals[index];
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        _chrome.Arrange(new Rect(new Point(0.0, 0.0), arrangeBounds));
        return arrangeBounds;
    }
}
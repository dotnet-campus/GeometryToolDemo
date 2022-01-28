using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace GeometryTool;

public class SizeAdorner : Adorner
{
    private BorderWithAdorner borderWA;
    private readonly SizeChrome chrome;
    private readonly VisualCollection visuals;

    public SizeAdorner(BorderWithAdorner borderWA)
        : base(borderWA)
    {
        SnapsToDevicePixels = true;
        this.borderWA = borderWA;
        chrome = new SizeChrome();
        chrome.DataContext = borderWA;
        visuals = new VisualCollection(this);
        visuals.Add(chrome);
    }

    protected override int VisualChildrenCount => visuals.Count;

    protected override Visual GetVisualChild(int index)
    {
        return visuals[index];
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        chrome.Arrange(new Rect(new Point(0.0, 0.0), arrangeBounds));
        return arrangeBounds;
    }
}
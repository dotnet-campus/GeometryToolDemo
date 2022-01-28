using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace GeometryTool;

/// <summary>
///     选择框的Adorner
/// </summary>
public class GeometryAdorner : Adorner
{
    public GeometryChrome chrome; //选择框的真正样式
    private readonly VisualCollection visuals;

    /// <summary>
    ///     构造函数，主要是使chrome是DataContext为BorderWithAdorner
    /// </summary>
    /// <param name="borderWA"></param>
    public GeometryAdorner(BorderWithAdorner borderWA)
        : base(borderWA)
    {
        SnapsToDevicePixels = true;
        chrome = new GeometryChrome();
        chrome.DataContext = borderWA;
        visuals = new VisualCollection(this);
        visuals.Add(chrome);
    }

    /// <summary>
    ///     重写的VisualChildrenCount函数
    /// </summary>
    protected override int VisualChildrenCount => visuals.Count;

    /// <summary>
    ///     重写的ArrangeOverride
    /// </summary>
    /// <param name="arrangeBounds"></param>
    /// <returns></returns>
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        arrangeBounds.Height += 0.5;
        arrangeBounds.Width += 0.5;
        chrome.Arrange(new Rect(arrangeBounds));
        return arrangeBounds;
    }

    /// <summary>
    ///     重写GetVisualChild函数
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    protected override Visual GetVisualChild(int index)
    {
        return visuals[index];
    }
}
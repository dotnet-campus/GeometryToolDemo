using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace GeometryTool;

/// <summary>
///     融合点时锁的Adorner
/// </summary>
public class LockAdorner : Adorner
{
    public LockChrome chrome; //锁的样式
    private readonly VisualCollection visuals;

    /// <summary>
    ///     重写的构造函数
    /// </summary>
    /// <param name="borderWA"></param>
    public LockAdorner(UIElement borderWA)
        : base(borderWA)
    {
        SnapsToDevicePixels = true;
        chrome = new LockChrome();
        chrome.DataContext = borderWA;
        visuals = new VisualCollection(this);
        visuals.Add(chrome);
    }

    /// <summary>
    ///     重写的VisualChildrenCount
    /// </summary>
    protected override int VisualChildrenCount => visuals.Count;

    /// <summary>
    ///     重写的ArrangeOverride函数
    /// </summary>
    /// <param name="arrangeBounds"></param>
    /// <returns></returns>
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        chrome.Arrange(new Rect(arrangeBounds));
        return arrangeBounds;
    }

    /// <summary>
    ///     重写的GetVisualChild的函数
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    protected override Visual GetVisualChild(int index)
    {
        return visuals[index];
    }
}
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace GeometryTool;

/// <summary>
///     融合点时锁的Adorner
/// </summary>
public class LockAdorner : Adorner
{
    /// <summary>
    /// 锁的样式
    /// </summary>
    public LockChrome LockChrome { get; }
    private readonly VisualCollection _visuals;

    /// <summary>
    ///     重写的构造函数
    /// </summary>
    /// <param name="borderWithAdorner"></param>
    public LockAdorner(UIElement borderWithAdorner)
        : base(borderWithAdorner)
    {
        SnapsToDevicePixels = true;
        LockChrome = new LockChrome();
        LockChrome.DataContext = borderWithAdorner;
        _visuals = new VisualCollection(this);
        _visuals.Add(LockChrome);
    }

    /// <summary>
    ///     重写的VisualChildrenCount
    /// </summary>
    protected override int VisualChildrenCount => _visuals.Count;

    /// <summary>
    ///     重写的ArrangeOverride函数
    /// </summary>
    /// <param name="arrangeBounds"></param>
    /// <returns></returns>
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        LockChrome.Arrange(new Rect(arrangeBounds));
        return arrangeBounds;
    }

    /// <summary>
    ///     重写的GetVisualChild的函数
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    protected override Visual GetVisualChild(int index)
    {
        return _visuals[index];
    }
}
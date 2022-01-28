using System.Windows;
using System.Windows.Controls;

namespace GeometryTool;

/// <summary>
///     可拖动的选择框的边的外观
/// </summary>
public class ResizeRotateChrome : Control
{
    static ResizeRotateChrome()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeRotateChrome),
            new FrameworkPropertyMetadata(typeof(ResizeRotateChrome)));
    }
}
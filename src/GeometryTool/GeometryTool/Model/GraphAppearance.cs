using System.ComponentModel;
using System.Windows.Media;

namespace GeometryTool;

/// <summary>
///     图形的外观
/// </summary>
public class GraphAppearance
{
    /// <summary>
    /// 填充规则
    /// </summary>
    private FillRule _fillRule;

    /// <summary>
    /// 设置虚实
    /// </summary>
    private DoubleCollection _strokeDashArray;

    /// <summary>
    /// 设置图形的线条的大小
    /// </summary>
    private double _strokeThickness;

    public GraphAppearance()
    {
        StrokeThickness = 0.1;

        Stroke = Brushes.Black;
        Fill = Brushes.Transparent;
        FillRule = FillRule.EvenOdd;
        StrokeDashArray = new DoubleCollection();
        StrokeDashArray.Add(1);
        StrokeDashArray.Add(0);
    }

    public double StrokeThickness
    {
        get => _strokeThickness;
        set
        {
            _strokeThickness = value;
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StrokeThickness"));
            }
        }
    }

    public DoubleCollection StrokeDashArray
    {
        get => _strokeDashArray;
        set
        {
            _strokeDashArray = value;
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StrokeDashArray"));
            }
        }
    }

    /// <summary>
    /// 设置图形的颜色
    /// </summary>
    private Brush _stroke;

    /// <summary>
    /// 设置图形的颜色
    /// </summary>
    public Brush Stroke
    {
        get => _stroke;
        set
        {
            _stroke = value;
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Stroke"));
            }
        }
    }

    /// <summary>
    /// 设置图形填充的颜色
    /// </summary>
    private Brush _fill;

    /// <summary>
    /// 设置图形填充的颜色
    /// </summary>
    public Brush Fill
    {
        get => _fill;
        set
        {
            _fill = value;
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Fill"));
            }
        }
    }

    public FillRule FillRule
    {
        get => _fillRule;
        set
        {
            _fillRule = value;
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("FillRule"));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
using System.ComponentModel;
using System.Windows.Media;

namespace GeometryTool;

/// <summary>
///     图形的外观
/// </summary>
public class GraphAppearance
{
    public FillRule fillRule; //填充规则

    private DoubleCollection strokeDashArray; //设置虚实

    private double strokeThickness; //设置图形的线条的大小

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
        get => strokeThickness;
        set
        {
            strokeThickness = value;
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StrokeThickness"));
            }
        }
    }

    public DoubleCollection StrokeDashArray
    {
        get => strokeDashArray;
        set
        {
            strokeDashArray = value;
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("StrokeDashArray"));
            }
        }
    }

    private Brush stroke { get; set; } //设置图形的颜色

    public Brush Stroke
    {
        get => stroke;
        set
        {
            stroke = value;
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Stroke"));
            }
        }
    }

    private Brush fill { get; set; } //设置图形填充的颜色

    public Brush Fill
    {
        get => fill;
        set
        {
            fill = value;
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Fill"));
            }
        }
    }

    public FillRule FillRule
    {
        get => fillRule;
        set
        {
            fillRule = value;
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("FillRule"));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
using System.Windows;

namespace GeometryTool;

/// <summary>
///     坐标点的换算，实现自动吸附功能
/// </summary>
public class AutoPoints
{
    /// <summary>
    ///     坐标点的换算，实现自动吸附功能
    /// </summary>
    public Point GetAutoAdsorbPoint(Point oldPoint)
    {
        var p = new Point();
        if (oldPoint.X * 10 % 10 >= 5)
        {
            var b = (int)oldPoint.X; //计算离其最近的一个X坐标
            p.X = (int)oldPoint.X + 1;
        }
        else
        {
            p.X = (int)oldPoint.X;
        }

        if (oldPoint.Y * 10 % 10 >= 5) //计算离其最近的一个Y坐标
        {
            p.Y = (int)oldPoint.Y + 1;
        }
        else
        {
            p.Y = (int)oldPoint.Y;
        }

        return p;
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeometryTool;

/// <summary>
///     图形的外层拖动的Adorner
/// </summary>
public class GeometryChrome : ContentControl
{
    public bool isLock;

    public GeometryChrome()
    {
        ContextMenu = new ContextMenu();
        var CopyItem = new MenuItem();
        CopyItem.Header = "复制";
        CopyItem.Command = RoutedCommands.CopyCommand;
        ContextMenu.Items.Add(CopyItem);

        var CutItem = new MenuItem();
        CutItem.Header = "剪切";
        CutItem.Command = RoutedCommands.CutCommand;
        ContextMenu.Items.Add(CutItem);

        var DeleteItem = new MenuItem();
        DeleteItem.Header = "删除";
        DeleteItem.Command = RoutedCommands.DeleteCommand;
        ContextMenu.Items.Add(DeleteItem);

        var TopItem = new MenuItem();
        TopItem.Header = "至于顶层";
        TopItem.Click += TopItem_Click;
        ContextMenu.Items.Add(TopItem);
        var BottomItem = new MenuItem();

        BottomItem.Header = "至于底层";
        BottomItem.Click += BottomItem_Click;
        ContextMenu.Items.Add(BottomItem);
    }

    public static void Arrange(GeometryChrome vGC)
    {
        var border = vGC.DataContext as BorderWithAdorner;
        vGC.ArrangeOverride(new Size { Width = border.MaxX, Height = border.MaxY });
    }

    /// <summary>
    ///     绘制这个锁
    /// </summary>
    /// <param name="arrangeBounds"></param>
    /// <returns></returns>
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        var border = DataContext as BorderWithAdorner;
        border.MaxX = 0;
        border.MinX = (border.EllipseList[0].Data as EllipseGeometry).Center.X;
        border.MaxY = 0;
        border.MinY = (border.EllipseList[0].Data as EllipseGeometry).Center.Y;
        foreach (var path in border.EllipseList)
        {
            var item = path.Data as EllipseGeometry;
            var p = item.Center;
            if (border.MaxX < p.X)
            {
                border.MaxX = p.X;
            }

            if (border.MaxY < p.Y)
            {
                border.MaxY = p.Y;
            }

            if (border.MinX > p.X)
            {
                border.MinX = p.X;
            }

            if (border.MinY > p.Y)
            {
                border.MinY = p.Y;
            }
        }

        var pg = (border.Child as Path).Data as PathGeometry;
        if (pg.Figures[0].Segments.Count > 0)
        {
            var geometry = pg.GetFlattenedPathGeometry();
            var bound = geometry.GetRenderBounds(new Pen(null, (border.Child as Path).StrokeThickness));
            Width = bound.Width + 1;
            Height = bound.Height + 1;
            Margin = new Thickness(border.ActualWidth - bound.Width - 0.65, border.ActualHeight - bound.Height - 0.65,
                0, 0);
        }
        else
        {
            Width = border.MaxX - border.MinX + 1;
            Height = border.MaxY - border.MinY + 1;

            Margin = new Thickness(border.MinX - 0.5, border.MinY - 0.5, 0, 0);
        }

        return base.ArrangeOverride(arrangeBounds);
    }

    /// <summary>
    ///     复制图形
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void CopyItem_Click(object sender, RoutedEventArgs e)
    {
        var BorderWA = DataContext as BorderWithAdorner;
        MainWindow.CopyBorderWA = BorderWA; //记录要粘贴的图形
        MainWindow.PasteCount = 0; //把粘贴次数重置为0

        var rootCanvas = MainWindow.myRootCanvas;
        var rootCanvasParent = rootCanvas.Parent as Canvas;
        var CanvasBorder = rootCanvasParent.Parent as Border;
        var ScrollViewer = CanvasBorder.Parent as ScrollViewer;
        var rootGrid = ScrollViewer.Parent as Grid;
        var mainWindow = rootGrid.Parent as MainWindow; //获取MainWindow实例，为了修改CanPaste属性
        mainWindow.CanPaste = true;
    }

    /// <summary>
    ///     剪切图形
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void CutItem_Click(object sender, RoutedEventArgs e)
    {
        var BorderWA = DataContext as BorderWithAdorner;
        MainWindow.CopyBorderWA = BorderWA; //记录要粘贴的图形
        MainWindow.PasteCount = 0; //把粘贴次数重置为0

        var rootCanvas = MainWindow.myRootCanvas;
        var rootCanvasParent = rootCanvas.Parent as Canvas;
        var CanvasBorder = rootCanvasParent.Parent as Border;
        var ScrollViewer = CanvasBorder.Parent as ScrollViewer;
        var rootGrid = ScrollViewer.Parent as Grid;
        var mainWindow = rootGrid.Parent as MainWindow; //获取MainWindow实例，为了修改CanPaste属性
        mainWindow.CanPaste = true;
        rootCanvas.Children.Remove(BorderWA);
        foreach (var item in BorderWA.EllipseList)
        {
            var borderWD = item.Parent as BorderWithDrag;
            rootCanvas.Children.Remove(borderWD);
        }
    }

    /// <summary>
    ///     删除图形
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void DeleteItem_Click(object sender, RoutedEventArgs e)
    {
        var borderWA = DataContext as BorderWithAdorner;
        var rootCanvas = MainWindow.myRootCanvas;
        rootCanvas.Children.Remove(borderWA); //移除图形
        foreach (var item in borderWA.EllipseList) //移除图形上面的点
        {
            var borderWD = item.Parent as BorderWithDrag;
            rootCanvas.Children.Remove(borderWD);
        }
    }

    /// <summary>
    ///     把图形至于顶层
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void TopItem_Click(object sender, RoutedEventArgs e)
    {
        var borderWA = DataContext as BorderWithAdorner;
        Panel.SetZIndex(borderWA, MainWindow.HightestLevel++);
        for (var i = 0; i < borderWA.EllipseList.Count; ++i)
        {
            Panel.SetZIndex(borderWA.EllipseList[i].Parent as BorderWithDrag, MainWindow.HightestLevel);
        }
    }

    /// <summary>
    ///     把图形至于底层
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void BottomItem_Click(object sender, RoutedEventArgs e)
    {
        var borderWA = DataContext as BorderWithAdorner;
        Panel.SetZIndex(borderWA, MainWindow.LowestLevel--);
        for (var i = 0; i < borderWA.EllipseList.Count; ++i)
        {
            Panel.SetZIndex(borderWA.EllipseList[i].Parent as BorderWithDrag, MainWindow.LowestLevel);
        }
    }
}
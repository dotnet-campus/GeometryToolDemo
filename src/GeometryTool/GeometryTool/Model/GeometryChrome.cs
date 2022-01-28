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
    public GeometryChrome()
    {
        ContextMenu = new ContextMenu();
        var copyItem = new MenuItem();
        copyItem.Header = "复制";
        copyItem.Command = RoutedCommands.CopyCommand;
        ContextMenu.Items.Add(copyItem);

        var cutItem = new MenuItem();
        cutItem.Header = "剪切";
        cutItem.Command = RoutedCommands.CutCommand;
        ContextMenu.Items.Add(cutItem);

        var deleteItem = new MenuItem();
        deleteItem.Header = "删除";
        deleteItem.Command = RoutedCommands.DeleteCommand;
        ContextMenu.Items.Add(deleteItem);

        var topItem = new MenuItem();
        topItem.Header = "置于顶层";
        topItem.Click += TopItem_Click;
        ContextMenu.Items.Add(topItem);
        var bottomItem = new MenuItem();

        bottomItem.Header = "置于底层";
        bottomItem.Click += BottomItem_Click;
        ContextMenu.Items.Add(bottomItem);
    }

    public static void Arrange(GeometryChrome geometryChrome)
    {
        var border = geometryChrome.DataContext as BorderWithAdorner;
        geometryChrome.ArrangeOverride(new Size { Width = border.MaxX, Height = border.MaxY });
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

        var pathGeometry = (border.Child as Path).Data as PathGeometry;
        if (pathGeometry.Figures[0].Segments.Count > 0)
        {
            var geometry = pathGeometry.GetFlattenedPathGeometry();
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
        var borderWithAdorner = DataContext as BorderWithAdorner;
        MainWindow.CopyBorderWithAdorner = borderWithAdorner; //记录要粘贴的图形
        MainWindow.PasteCount = 0; //把粘贴次数重置为0

        var rootCanvas = MainWindow.MyRootCanvas;
        var rootCanvasParent = rootCanvas.Parent as Canvas;
        var canvasBorder = rootCanvasParent.Parent as Border;
        var scrollViewer = canvasBorder.Parent as ScrollViewer;
        var rootGrid = scrollViewer.Parent as Grid;
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
        var borderWithAdorner = DataContext as BorderWithAdorner;
        MainWindow.CopyBorderWithAdorner = borderWithAdorner; //记录要粘贴的图形
        MainWindow.PasteCount = 0; //把粘贴次数重置为0

        var rootCanvas = MainWindow.MyRootCanvas;
        var rootCanvasParent = rootCanvas.Parent as Canvas;
        var canvasBorder = rootCanvasParent.Parent as Border;
        var scrollViewer = canvasBorder.Parent as ScrollViewer;
        var rootGrid = scrollViewer.Parent as Grid;
        var mainWindow = rootGrid.Parent as MainWindow; //获取MainWindow实例，为了修改CanPaste属性
        mainWindow.CanPaste = true;
        rootCanvas.Children.Remove(borderWithAdorner);
        foreach (var item in borderWithAdorner.EllipseList)
        {
            var borderWithDrag = item.Parent as BorderWithDrag;
            rootCanvas.Children.Remove(borderWithDrag);
        }
    }

    /// <summary>
    ///     删除图形
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void DeleteItem_Click(object sender, RoutedEventArgs e)
    {
        var borderWithAdorner = DataContext as BorderWithAdorner;
        var rootCanvas = MainWindow.MyRootCanvas;
        rootCanvas.Children.Remove(borderWithAdorner); //移除图形
        foreach (var item in borderWithAdorner.EllipseList) //移除图形上面的点
        {
            var borderWithDrag = item.Parent as BorderWithDrag;
            rootCanvas.Children.Remove(borderWithDrag);
        }
    }

    /// <summary>
    ///     把图形至于顶层
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void TopItem_Click(object sender, RoutedEventArgs e)
    {
        var borderWithAdorner = DataContext as BorderWithAdorner;
        Panel.SetZIndex(borderWithAdorner, MainWindow.HightestLevel++);
        for (var i = 0; i < borderWithAdorner.EllipseList.Count; ++i)
        {
            Panel.SetZIndex(borderWithAdorner.EllipseList[i].Parent as BorderWithDrag, MainWindow.HightestLevel);
        }
    }

    /// <summary>
    ///     把图形至于底层
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void BottomItem_Click(object sender, RoutedEventArgs e)
    {
        var borderWithAdorner = DataContext as BorderWithAdorner;
        Panel.SetZIndex(borderWithAdorner, MainWindow.LowestLevel--);
        for (var i = 0; i < borderWithAdorner.EllipseList.Count; ++i)
        {
            Panel.SetZIndex(borderWithAdorner.EllipseList[i].Parent as BorderWithDrag, MainWindow.LowestLevel);
        }
    }
}
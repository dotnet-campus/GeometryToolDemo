using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GeometryTool
{
    /// <summary>
    /// 图形的外层拖动的Adorner
    /// </summary>
    public class GeometryChrome : ContentControl
    {

        public bool isLock;

        public GeometryChrome()
        {
            this.ContextMenu = new ContextMenu();
            MenuItem CopyItem = new MenuItem();
            CopyItem.Header = "复制";
            CopyItem.Command = RoutedCommands.CopyCommand;
            //CopyItem.Click+=CopyItem_Click;
            this.ContextMenu.Items.Add(CopyItem);

            MenuItem CutItem = new MenuItem();
            CutItem.Header = "剪切";
            CutItem.Command = RoutedCommands.CutCommand;
            //CutItem.Click+=CutItem_Click;
            this.ContextMenu.Items.Add(CutItem);

            MenuItem DeleteItem = new MenuItem();
            DeleteItem.Header = "删除";
            //DeleteItem.Click+=DeleteItem_Click;
            DeleteItem.Command = RoutedCommands.DeleteCommand;
            this.ContextMenu.Items.Add(DeleteItem);

        }

        public static void Arrange(GeometryChrome vGC)
        {
            BorderWithAdorner border = vGC.DataContext as BorderWithAdorner;
            vGC.ArrangeOverride(new Size() { Width = border.MaxX, Height = border.MaxY });
        }
        /// <summary>
        /// 绘制这个锁
        /// </summary>
        /// <param name="arrangeBounds"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            BorderWithAdorner border = this.DataContext as BorderWithAdorner;
            border.MaxX =0;
            border.MinX = (border.EllipseList[0].Data as EllipseGeometry).Center.X;
            border.MaxY = 0;
            border.MinY = (border.EllipseList[0].Data as EllipseGeometry).Center.Y;
            foreach(var path  in border.EllipseList)
            {
                EllipseGeometry item = path.Data as EllipseGeometry;
                Point p = item.Center;
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

            PathGeometry pg = (border.Child as Path).Data as PathGeometry;
            if (pg.Figures[0].Segments.Count>0)
            {
                var geometry = pg.GetFlattenedPathGeometry();
                var bound = geometry.GetRenderBounds(new Pen(null, (border.Child as Path).StrokeThickness));
                this.Width = bound.Width + 1;
                this.Height =bound.Height+1;
                this.Margin = new Thickness(border.ActualWidth - bound.Width - 0.65, border.ActualHeight - bound.Height - 0.65, 0, 0);
            }
            else
            {
                this.Width = border.MaxX - border.MinX + 1;
                this.Height = border.MaxY - border.MinY + 1;

                this.Margin = new Thickness(border.MinX - 0.5, border.MinY - 0.5, 0, 0);
            }
            return base.ArrangeOverride(arrangeBounds);
        }

        /// <summary>
        /// 复制图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CopyItem_Click(object sender, RoutedEventArgs e)
        {
            BorderWithAdorner BorderWA = this.DataContext as BorderWithAdorner;
            MainWindow.CopyBorderWA = BorderWA; //记录要粘贴的图形
            MainWindow.PasteCount = 0;          //把粘贴次数重置为0

            Canvas rootCanvas = MainWindow.myRootCanvas as Canvas;
            Canvas rootCanvasParent= rootCanvas.Parent as Canvas;
            Border CanvasBorder = rootCanvasParent.Parent as Border;
            ScrollViewer ScrollViewer = CanvasBorder.Parent as ScrollViewer;
            Grid rootGrid = ScrollViewer.Parent as Grid;
            MainWindow mainWindow = rootGrid.Parent as MainWindow;              //获取MainWindow实例，为了修改CanPaste属性
            mainWindow.CanPaste = true;
        }

        /// <summary>
        /// 剪切图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CutItem_Click(object sender, RoutedEventArgs e)
        {
            BorderWithAdorner BorderWA = this.DataContext as BorderWithAdorner;
            MainWindow.CopyBorderWA = BorderWA;     //记录要粘贴的图形
            MainWindow.PasteCount = 0;              //把粘贴次数重置为0

            Canvas rootCanvas = MainWindow.myRootCanvas as Canvas;
            Canvas rootCanvasParent = rootCanvas.Parent as Canvas;
            Border CanvasBorder = rootCanvasParent.Parent as Border;
            ScrollViewer ScrollViewer = CanvasBorder.Parent as ScrollViewer;
            Grid rootGrid = ScrollViewer.Parent as Grid;
            MainWindow mainWindow = rootGrid.Parent as MainWindow;  //获取MainWindow实例，为了修改CanPaste属性
            mainWindow.CanPaste = true;
            rootCanvas.Children.Remove(BorderWA);
            foreach (var item in BorderWA.EllipseList)
            {
                BorderWithDrag borderWD = item.Parent as BorderWithDrag;
                rootCanvas.Children.Remove(borderWD);
            }
        }

        /// <summary>
        /// 删除图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            BorderWithAdorner borderWA = this.DataContext as BorderWithAdorner;
            Canvas rootCanvas = MainWindow.myRootCanvas as Canvas;
            Canvas rootCanvasParent = rootCanvas.Parent as Canvas;
            Border canvasBorder = rootCanvasParent.Parent as Border;
            ScrollViewer ScrollViewer = canvasBorder.Parent as ScrollViewer;
            Grid rootGrid = ScrollViewer.Parent as Grid;
            MainWindow mainWindow = rootGrid.Parent as MainWindow;      //获取MainWindow实例，为了修改CanPaste属性

            rootCanvas.Children.Remove(borderWA);       //移除图形
            foreach (var item in borderWA.EllipseList)  //移除图形上面的点
            {
                BorderWithDrag borderWD = item.Parent as BorderWithDrag;
                rootCanvas.Children.Remove(borderWD);
            }
        }

        /// <summary>
        /// 添加删除/复制/剪切的路由事件
        /// </summary>
   
    }
}

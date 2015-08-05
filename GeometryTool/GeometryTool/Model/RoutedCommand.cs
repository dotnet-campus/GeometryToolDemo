using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GeometryTool
{
    /// <summary>
    /// 定义快捷键的路由事件
    /// </summary>
    public static class RoutedCommands
    {
        private static RoutedUICommand pasteCommand;    //粘贴的路由事件
        public static RoutedUICommand PasteCommand
        {
            get
            {
                if (pasteCommand == null)
                {
                    pasteCommand = new RoutedUICommand(
                        "Paste",
                        "Paste",
                        typeof(MainWindow),
                        new InputGestureCollection { new KeyGesture(Key.V, ModifierKeys.Control) });
                }

                return pasteCommand;
            }
        }

        private static RoutedUICommand copyCommand;    //复制的路由事件
        public static RoutedUICommand CopyCommand
        {
            get
            {
                if (copyCommand == null)
                {
                    copyCommand = new RoutedUICommand(
                        "Copy",
                        "Copy",
                        typeof(GeometryChrome),
                        new InputGestureCollection { new KeyGesture(Key.C, ModifierKeys.Control) });
                }

                return copyCommand;
            }
        }

        private static RoutedUICommand cutCommand;     //剪切的路由事件
        public static RoutedUICommand CutCommand
        {
            get
            {
                if (cutCommand == null)
                {
                    cutCommand = new RoutedUICommand(
                        "Cut",
                        "Cut",
                        typeof(MainWindow),
                        new InputGestureCollection { new KeyGesture(Key.X, ModifierKeys.Control) });
                }

                return cutCommand;
            }
        }

        private static RoutedUICommand deleteCommand;     //剪切的路由事件
        public static RoutedUICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                {
                    deleteCommand = new RoutedUICommand(
                        "Delete",
                        "Delete",
                        typeof(MainWindow),
                        new InputGestureCollection { new KeyGesture(Key.X, ModifierKeys.Control) });
                }

                return deleteCommand;
            }
        }
    }
}

using System.Windows.Input;

namespace GeometryTool;

/// <summary>
///     定义快捷键的路由事件
/// </summary>
public static class RoutedCommands
{
    private static RoutedUICommand pasteCommand; //粘贴的路由事件

    private static RoutedUICommand copyCommand; //复制的路由事件

    private static RoutedUICommand cutCommand; //剪切的路由事件

    private static RoutedUICommand deleteCommand; //删除的路由事件

    private static RoutedUICommand saveCommand; //保存的路由事件

    private static RoutedUICommand newCommand; //新建的路由事件

    private static RoutedUICommand openCommand; //打开的路由事件

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

    public static RoutedUICommand CopyCommand
    {
        get
        {
            if (copyCommand == null)
            {
                copyCommand = new RoutedUICommand(
                    "Copy",
                    "Copy",
                    typeof(MainWindow),
                    new InputGestureCollection { new KeyGesture(Key.C, ModifierKeys.Control) });
            }

            return copyCommand;
        }
    }

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
                    new InputGestureCollection { new KeyGesture(Key.Delete) });
            }

            return deleteCommand;
        }
    }

    public static RoutedUICommand SaveCommand
    {
        get
        {
            if (saveCommand == null)
            {
                saveCommand = new RoutedUICommand(
                    "Save",
                    "Save",
                    typeof(MainWindow),
                    new InputGestureCollection { new KeyGesture(Key.S, ModifierKeys.Control) });
            }

            return saveCommand;
        }
    }

    public static RoutedUICommand NewCommand
    {
        get
        {
            if (newCommand == null)
            {
                newCommand = new RoutedUICommand(
                    "New",
                    "New",
                    typeof(MainWindow),
                    new InputGestureCollection { new KeyGesture(Key.N, ModifierKeys.Control) });
            }

            return newCommand;
        }
    }

    public static RoutedUICommand OpenCommand
    {
        get
        {
            if (openCommand == null)
            {
                openCommand = new RoutedUICommand(
                    "open",
                    "open",
                    typeof(MainWindow),
                    new InputGestureCollection { new KeyGesture(Key.O, ModifierKeys.Control) });
            }

            return openCommand;
        }
    }
}
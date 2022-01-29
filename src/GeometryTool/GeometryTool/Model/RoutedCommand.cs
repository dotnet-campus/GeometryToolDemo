using System.Windows.Input;

namespace GeometryTool;

/// <summary>
///     定义快捷键的路由事件
/// </summary>
public static class RoutedCommands
{
    /// <summary>
    /// 粘贴的路由事件
    /// </summary>
    private static RoutedUICommand _pasteCommand;

    /// <summary>
    /// 复制的路由事件
    /// </summary>
    private static RoutedUICommand _copyCommand;

    /// <summary>
    /// 剪切的路由事件
    /// </summary>
    private static RoutedUICommand _cutCommand;

    /// <summary>
    /// 删除的路由事件
    /// </summary>
    private static RoutedUICommand _deleteCommand;

    /// <summary>
    /// 保存的路由事件
    /// </summary>
    private static RoutedUICommand _saveCommand;

    /// <summary>
    /// 新建的路由事件
    /// </summary>
    private static RoutedUICommand _newCommand;

    /// <summary>
    /// 打开的路由事件
    /// </summary>
    private static RoutedUICommand _openCommand;

    public static RoutedUICommand PasteCommand
    {
        get
        {
            if (_pasteCommand == null)
            {
                _pasteCommand = new RoutedUICommand
                (
                    "Paste",
                    "Paste",
                    typeof(MainWindow),
                    new InputGestureCollection { new KeyGesture(Key.V, ModifierKeys.Control) }
                );
            }

            return _pasteCommand;
        }
    }

    public static RoutedUICommand CopyCommand
    {
        get
        {
            if (_copyCommand == null)
            {
                _copyCommand = new RoutedUICommand
                (
                    "Copy",
                    "Copy",
                    typeof(MainWindow),
                    new InputGestureCollection { new KeyGesture(Key.C, ModifierKeys.Control) }
                );
            }

            return _copyCommand;
        }
    }

    public static RoutedUICommand CutCommand
    {
        get
        {
            if (_cutCommand == null)
            {
                _cutCommand = new RoutedUICommand
                (
                    "Cut",
                    "Cut",
                    typeof(MainWindow),
                    new InputGestureCollection { new KeyGesture(Key.X, ModifierKeys.Control) }
                );
            }

            return _cutCommand;
        }
    }

    public static RoutedUICommand DeleteCommand
    {
        get
        {
            if (_deleteCommand == null)
            {
                _deleteCommand = new RoutedUICommand
                (
                    "Delete",
                    "Delete",
                    typeof(MainWindow),
                    new InputGestureCollection { new KeyGesture(Key.Delete) }
                );
            }

            return _deleteCommand;
        }
    }

    public static RoutedUICommand SaveCommand
    {
        get
        {
            if (_saveCommand == null)
            {
                _saveCommand = new RoutedUICommand
                (
                    "Save",
                    "Save",
                    typeof(MainWindow),
                    new InputGestureCollection { new KeyGesture(Key.S, ModifierKeys.Control) }
                );
            }

            return _saveCommand;
        }
    }

    public static RoutedUICommand NewCommand
    {
        get
        {
            if (_newCommand == null)
            {
                _newCommand = new RoutedUICommand
                (
                    "New",
                    "New",
                    typeof(MainWindow),
                    new InputGestureCollection { new KeyGesture(Key.N, ModifierKeys.Control) }
                );
            }

            return _newCommand;
        }
    }

    public static RoutedUICommand OpenCommand
    {
        get
        {
            if (_openCommand == null)
            {
                _openCommand = new RoutedUICommand
                (
                    "Open",
                    "Open",
                    typeof(MainWindow),
                    new InputGestureCollection { new KeyGesture(Key.O, ModifierKeys.Control) }
                );
            }

            return _openCommand;
        }
    }
}
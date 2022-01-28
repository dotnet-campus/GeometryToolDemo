﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GeometryTool;

/// <summary>
///     锁真正的外观
/// </summary>
public class LockChrome : Image
{
    public bool isLock;

    public LockChrome()
    {
        MouseDown += Element_MouseLeftButtonDown; //给这个Image添加点击事件，用于解开融合
        Source = new BitmapImage(new Uri("Image/lock.png", UriKind.Relative));
        isLock = true;
    }

    /// <summary>
    ///     绘制这个锁
    /// </summary>
    /// <param name="arrangeBounds"></param>
    /// <returns></returns>
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        Width = 1.2;
        Height = 1.2;

        HorizontalAlignment = HorizontalAlignment.Right; //放在点的右下角
        VerticalAlignment = VerticalAlignment.Bottom;

        var tt = new TranslateTransform(); //做一个旋转的变换
        tt.X = 1.2;
        tt.Y = -0.3;
        RenderTransform = tt;

        var binding = new Binding("HasOtherPoint") { Converter = new ImageVisibilityConverter() };
        SetBinding(VisibilityProperty, binding); //当没有重合点的时候，隐藏锁

        return base.ArrangeOverride(arrangeBounds);
    }

    /// <summary>
    ///     用于解开融合
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var Chrome = sender as LockChrome;
        if (Chrome != null)
        {
            if (isLock) //融合变不融合
            {
                var _border = DataContext as BorderWithDrag;
                var _borderLock = new BorderLock();
                _borderLock.unLock(_border);
            }

            e.Handled = true;
        }
    }
}
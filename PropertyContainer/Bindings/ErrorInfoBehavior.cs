namespace PropertyContainer.Bindings
{
  using System;
  using System.ComponentModel;
  using System.Windows;
  using System.Windows.Controls;

  public partial class ErrorInfoBehavior
  {
    #region TargetProperty
    public static readonly DependencyProperty TargetPropertyProperty = DependencyProperty.RegisterAttached(
      "DependencyProperty", typeof(DependencyProperty), typeof(ErrorInfoBehavior));

    public static DependencyProperty GetTargetPropertyProperty(DependencyObject obj)
    {
      return (DependencyProperty) obj.GetValue(TargetPropertyProperty); 
    }

    public static void SetTargetPropertyProperty(DependencyObject obj, object value)
    {
      obj.SetValue(TargetPropertyProperty, value);
    }
    #endregion TargetProperty

    #region Context
    public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached(
      "Context", typeof(INotifyDataErrorInfo), typeof(ErrorInfoBehavior), new PropertyMetadata(OnContextChanged));

    public static INotifyDataErrorInfo GetContext(DependencyObject obj)
    {
      return (INotifyDataErrorInfo) obj.GetValue(ContextProperty); 
    }

    public static void SetContext(DependencyObject obj, object value)
    {
      obj.SetValue(ContextProperty, value);
    }

    private static void OnContextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      if (obj is FrameworkElement element && e.NewValue is INotifyDataErrorInfo errorInfo)
      {
        var handler = new ErrorInfoHandler(element, errorInfo, GetProperty(element));
        handler.Start();
      }
    }
    #endregion Context

    #region Private Methods
    private static DependencyProperty GetProperty(FrameworkElement element)
    {
      var dp = GetTargetPropertyProperty(element);
      if (dp != null)
      {
        return dp;
      }

      if (element is TextBox)
      {
        return TextBox.TextProperty;
      }

      throw new NotImplementedException($"Please specify TargetProperty for {element}");
    }
    #endregion Private Methods
  }
}
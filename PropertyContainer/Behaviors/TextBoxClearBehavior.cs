namespace PropertyContainer.Behaviors
{
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// TextBoxClearBehavior - Set TextBox.Text to null when it's cleared in these scenarios
  /// 1. Press Delete/Backspace
  /// 2. Select all and press Ctrl-X to cut
  /// </summary>
  public partial class TextBoxClearBehavior
  {
    #region IsEnabled
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
      "IsEnabled", typeof(bool), typeof(TextBoxClearBehavior), new PropertyMetadata(OnIsEnabledChanged));

    public static bool GetIsEnabled(DependencyObject obj)
    {
      return (bool) obj.GetValue(IsEnabledProperty); 
    }

    public static void SetIsEnabled(DependencyObject obj, bool value)
    {
      obj.SetValue(IsEnabledProperty, value);
    }

    private static void OnIsEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      var textBox = (TextBox) obj;
      if ((bool) e.NewValue)
      {
        var behavior = new TextBoxClearBehaviorImpl(textBox);
        behavior.Start();
      }
    }
    #endregion IsEnabled
  }
}
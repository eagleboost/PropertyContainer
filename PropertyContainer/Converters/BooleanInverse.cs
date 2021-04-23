namespace PropertyContainer.Converters
{
  using System;
  using System.Globalization;
  using System.Windows.Data;

  /// <summary>
  /// BooleanInverse
  /// </summary>
  public sealed class BooleanInverse : MarkupBase<BooleanInverse>, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var v = (bool) value;
      return !v;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
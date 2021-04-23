namespace CoreLib.Core
{
  using System.ComponentModel;

  /// <summary>
  /// IPropertyChangeNotifiable
  /// </summary>
  public interface IPropertyChangeNotifiable
  {
    void NotifyPropertyChanged(PropertyChangedEventArgs args);
  }
}
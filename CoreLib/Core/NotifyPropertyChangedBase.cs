namespace CoreLib.Core
{
  using System.ComponentModel;

  /// <summary>
  /// NotifyPropertyChangedBase
  /// </summary>
  public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged, IPropertyChangeNotifiable
  {
    public event PropertyChangedEventHandler PropertyChanged;
    
    public void NotifyPropertyChanged(PropertyChangedEventArgs args)
    {
      PropertyChanged?.Invoke(this, args);
    }
  }
}
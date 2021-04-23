namespace CoreLib.UnityExt
{
  using System.ComponentModel;
  using CoreLib.Core;

  /// <summary>
  /// IPropertyContainerMarker
  /// </summary>
  public interface IPropertyContainerMarker : IPropertyContainer, IPropertyChangeNotifiable, INotifyPropertyChanged
  {
  }
}
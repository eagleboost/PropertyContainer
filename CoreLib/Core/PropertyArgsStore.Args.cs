namespace CoreLib.Core
{
  using System.ComponentModel;

  /// <summary>
  /// PropertyChangedEventArgs
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public sealed class PropertyChangedEventArgs<T> : PropertyChangedEventArgs
  {
    public PropertyChangedEventArgs(string name) : base(name)
    {
    }
  }
}
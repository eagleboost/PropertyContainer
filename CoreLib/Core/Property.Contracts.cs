namespace CoreLib.Core
{
  using System;
  using System.ComponentModel;

  /// <summary>
  /// IProperty
  /// </summary>
  public interface IProperty
  {
    object Parent { get; }
    
    PropertyChangedEventArgs NameArgs { get; }
    
    Type Type { get; }
    
    object Value { get; set; }
    
    event EventHandler<ValueChangedArgs<object>> ValueChanged;
  }

  /// <summary>
  /// IProperty
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public interface IProperty<T> : IProperty
  {
    new T Value { get; set; }

    new event EventHandler<ValueChangedArgs<T>> ValueChanged;
  }
}
namespace CoreLib.Core
{
  using System;
  using System.ComponentModel;
  using System.Diagnostics;
  using CoreLib.Extensions;

  /// <summary>
  ///   ValueChangedArgs
  /// </summary>
  /// <typeparam propertyArgs="T"></typeparam>
  /// <typeparam name="T"></typeparam>
  [DebuggerDisplay("{PropertyArgs.PropertyName}: {OldValue}=>{NewValue}")]
  public sealed class ValueChangedArgs<T> : EventArgs
  {
    public readonly PropertyChangedEventArgs PropertyArgs;

    public readonly T NewValue;

    public readonly T OldValue;

    public ValueChangedArgs(PropertyChangedEventArgs propertyArgs, T oldValue, T newValue)
    {
      PropertyArgs = propertyArgs;
      OldValue = oldValue;
      NewValue = newValue;
    }

    public override string ToString()
    {
      return $"{PropertyArgs.PropertyName}: {OldValue.ToStringEx()}=>{NewValue.ToStringEx()}";
    }
  }
}
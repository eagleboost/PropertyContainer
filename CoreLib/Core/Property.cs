namespace CoreLib.Core
{
  using System;
  using System.ComponentModel;
  using System.Diagnostics;
  using CoreLib.Extensions;

  /// <summary>
  ///   NameArgsProperty
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <typeparam name="TParent"></typeparam>
  [DebuggerDisplay("{Parent} {NameArgs}:{Value}")]
  public sealed class Property<TParent, T> : IProperty<T> where TParent : class
  {
    private static readonly Type PropertyType = typeof(T);
    private EventHandler<ValueChangedArgs<object>> _valueChanged;
    private T _value;

    public Property(TParent parent, PropertyChangedEventArgs name, T defValue = default)
    {
      Parent = parent;
      NameArgs = name;
      _value = defValue;
    }

    object IProperty.Parent => Parent;
    
    public TParent Parent { get; }

    public PropertyChangedEventArgs NameArgs { get; }

    public Type Type => PropertyType;

    object IProperty.Value
    {
      get => Value;
      set => Value = (T)value;
    }

    event EventHandler<ValueChangedArgs<object>> IProperty.ValueChanged
    {
      add => _valueChanged += value;
      remove => _valueChanged -= value;
    }

    public T Value
    {
      get => _value;
      set
      {
        if (value.SetField(ref _value, out var oldValue))
        {
          OnValueChanged(oldValue, value);
        }
      }
    }

    public event EventHandler<ValueChangedArgs<T>> ValueChanged;

    private void OnValueChanged(T oldValue, T newValue)
    {
      ValueChanged?.Invoke(this, new ValueChangedArgs<T>(NameArgs, oldValue, newValue));
      _valueChanged?.Invoke(this, new ValueChangedArgs<object>(NameArgs, oldValue, newValue));
    }

    public override string ToString()
    {
      return $"{Parent} {NameArgs.PropertyName}:{Value.ToStringEx()}";
    }
  }
}
namespace CoreLib.Core
{
  /// <summary>
  /// PropertyWrapper
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public partial class PropertyStore<T>
  {
    private interface IPropertyWrapper
    {
      IProperty Property { get; }
    }
    
    private class PropertyWrapper<TProperty> : IPropertyWrapper
    {
      private readonly T _notifier;

      public PropertyWrapper(T notifier, PropertyChangedEventArgs<TProperty> name, TProperty defValue = default)
      {
        _notifier = notifier;

        Property = new Property<T, TProperty>(notifier, name, defValue);
        Property.ValueChanged += HandleValueChanged;
      }

      IProperty IPropertyWrapper.Property => Property;
      
      public Property<T, TProperty> Property { get; }

      private void HandleValueChanged(object sender, ValueChangedArgs<TProperty> e)
      {
        _notifier.NotifyPropertyChanged(e.PropertyArgs);
      }
    }
  }
}
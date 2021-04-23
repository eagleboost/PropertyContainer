namespace CoreLib.Core
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  /// PropertyStore
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public sealed partial class PropertyStore<T> : IPropertyStore where T : class, IPropertyChangeNotifiable
  {
    private readonly T _notifier;
    private readonly Dictionary<string, IPropertyWrapper> _properties = new Dictionary<string, IPropertyWrapper>();

    public PropertyStore(T notifier)
    {
      _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
    }

    public IReadOnlyCollection<IProperty> Properties => _properties.Values.Select(i => i.Property).ToArray();

    public IProperty GetProperty(string name)
    {
      var wrapper = _properties.GetValueOrDefault(name);
      return wrapper?.Property;
    }
    
    public void Create<TProperty>(string name, out IProperty<TProperty> property, TProperty defValue = default)
    {
      var args = PropertyArgsStore<T>.GetArgs<TProperty>(name);
      var wrapper = new PropertyWrapper<TProperty>(_notifier, args, defValue);
      property = wrapper.Property;
      AddProperty(name, wrapper);
    }
    
    public IProperty<TProperty> CreateImplicit<TProperty>(string name, TProperty defValue = default)
    {
      Create(name, out var property, defValue);
      return property;
    }
    
    private void AddProperty(string name, IPropertyWrapper wrapper)
    {
      _properties.Add(name, wrapper);
    }
  }
}
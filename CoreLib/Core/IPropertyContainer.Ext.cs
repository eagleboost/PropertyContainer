namespace CoreLib.Core
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq.Expressions;
  using CoreLib.Extensions;

  /// <summary>
  /// PropertyContainerExt
  /// </summary>
  public static class PropertyContainerExt
  {
    public static IDisposable HookChange<T, TProperty>(this T container,
      Expression<Func<T, TProperty>> expr, EventHandler<ValueChangedArgs<TProperty>> handler)
    {
      var property = container.GetProperty(expr);
      return property.HookChange(handler);
    }

    public static IProperty<TProperty> GetProperty<T, TProperty>(this T container, Expression<Func<T, TProperty>> expr)
    {
      var propertyStore = container.GetPropertyStore();
      var property = propertyStore.GetProperty(expr.GetMember());
      return (IProperty<TProperty>)property;
    }

    public static IReadOnlyCollection<IProperty> GetProperties<T>(this T container)
    {
      var propertyStore = container.GetPropertyStore();
      return propertyStore.Properties;
    }
    
    public static IDisposable HookChange<T>(this T container, PropertyChangedEventHandler handler)
    {
      var notifier = (INotifyPropertyChanged) container;
      notifier.PropertyChanged += handler;

      return new ActionDisposable(() => notifier.PropertyChanged -= handler);
    }
    
    private static IPropertyStore GetPropertyStore<T>(this T container)
    {
      var c = (IPropertyContainer) container;
      return c.PropertyStore;
    }
  }
}
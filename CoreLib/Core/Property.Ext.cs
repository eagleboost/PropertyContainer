namespace CoreLib.Core
{
  using System;

  /// <summary>
  ///   PropertyExt
  /// </summary>
  public static class PropertyExt
  {
    public static IDisposable HookChange<T>(this IProperty<T> property, EventHandler<ValueChangedArgs<T>> handler)
    {
      property.ValueChanged += handler;

      return new ActionDisposable(() => property.ValueChanged -= handler);
    }
    
    public static IDisposable HookChange(this IProperty property, EventHandler<ValueChangedArgs<object>> handler)
    {
      property.ValueChanged += handler;

      return new ActionDisposable(() => property.ValueChanged -= handler);
    }
  }
}
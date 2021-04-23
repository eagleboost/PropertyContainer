namespace CoreLib.Core
{
  using System;
  using System.Linq.Expressions;
  using CoreLib.Extensions;

  /// <summary>
  /// PropertyArgsStore
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public static partial class PropertyArgsStore<T>
  {
    public static PropertyChangedEventArgs<TProperty> GetArgs<TProperty>(string name)
    {
      return PropertyChangedArgsStore<TProperty>.GetArgs(name);
    }
    
    public static PropertyChangedEventArgs<TProperty> GetArgs<TProperty>(Expression<Func<T, TProperty>> expr)
    {
      var name = expr.GetMember();
      return PropertyChangedArgsStore<TProperty>.GetArgs(name);
    }
  }
}
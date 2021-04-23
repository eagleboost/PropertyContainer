namespace CoreLib.Core
{
  using System.Collections.Concurrent;

  public static partial class PropertyArgsStore<T>
  {
    private static class PropertyChangedArgsStore<TProperty>
    {
      private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs<TProperty>> Cache = new ConcurrentDictionary<string, PropertyChangedEventArgs<TProperty>>();

      public static PropertyChangedEventArgs<TProperty> GetArgs(string name)
      {
        return Cache.GetOrAdd(name, CreateArgs);
      }
    
      private static PropertyChangedEventArgs<TProperty> CreateArgs(string name)
      {
        return new PropertyChangedEventArgs<TProperty>(name);
      }
    }
  }
}
namespace CoreLib.Extensions
{
  using System.Collections.Generic;

  public static class ListExt
  {
    public static void AddRange<T>(IList<T> list, IEnumerable<T> toAdd)
    {
      foreach (var i in toAdd)
      {
        list.Add(i);
      }
    }
  }
}
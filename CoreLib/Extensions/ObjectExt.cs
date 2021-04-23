namespace CoreLib.Extensions
{
  using System.Collections.Generic;

  /// <summary>
  /// ObjectExt
  /// </summary>
  public static class ObjectExt
  {
    public static bool SetField<T>(this T value, ref T field, out T oldValue)
    {
      oldValue = field;
      if (!EqualityComparer<T>.Default.Equals(field, value))
      {
        field = value;
        return true;
      }

      return false;
    }

    public static string ToStringEx<T>(this T value)
    {
      if (typeof(T).IsClass && EqualityComparer<T>.Default.Equals(value, default(T)))
      {
        return "<null>";
      }

      return value.ToString();
    }

    public static T CastTo<T>(this object obj)
    {
      return (T)obj;
    }
  }
}
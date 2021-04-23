namespace CoreLib.Extensions
{
  public static class StringExt
  {
    public static bool HasValue(this string str)
    {
      return !string.IsNullOrEmpty(str);
    }
    
    public static bool IsNullOrEmpty(this string str)
    {
      return string.IsNullOrEmpty(str);
    }
  }
}
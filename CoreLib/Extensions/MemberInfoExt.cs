namespace CoreLib.Extensions
{
  using System;
  using System.Reflection;

  /// <summary>
  /// MemberInfoExt
  /// </summary>
  public static class MemberInfoExt
  {
    public static bool IsDefined<T>(this MemberInfo member, bool inherit = true) where T : Attribute
    {
      return member.IsDefined(typeof(T), inherit);
    }
  }
}
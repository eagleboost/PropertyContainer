namespace CoreLib.Extensions
{
  using System.Linq.Expressions;

  /// <summary>
  /// ExpressionExt
  /// </summary>
  public static class ExpressionExt
  {
    public static string GetMember(this LambdaExpression expr)
    {
      var member = (MemberExpression) expr.Body;
      var name = member.Member.Name;
      return name;
    }
  }
}
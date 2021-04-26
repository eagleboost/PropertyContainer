namespace CoreLib.Extensions
{
  using System;

  public static class TypeExt
  {
    public static bool IsSubclassOf<T>(this Type type)
    {
      return typeof(T).IsAssignableFrom(type);
    }
    
    public static bool MatchOpenType(this Type type, Type openType)
    {
      EnsureOpenType(openType);
      
      var argumentType = GetGenericArgumentType(type, openType);
      if (argumentType == type)
      {
        return true;
      }
      
      foreach (var interfaceType in type.GetInterfaces())
      {
        argumentType = GetGenericArgumentType(interfaceType, openType);
        if (argumentType == type)
        {
          return true;
        }
      }

      return false;
    }

    public static Type GetGenericArgumentType(this Type type, Type openType)
    {
      EnsureOpenType(openType);
      
      var argumentType = GetGenericArgumentTypeCore(type, openType);
      if (argumentType == null)
      {
        foreach (var interfaceType in type.GetInterfaces())
        {
          argumentType = GetGenericArgumentType(interfaceType, openType);
          if (argumentType != null)
          {
            break;
          }
        }
      }

      return argumentType;
    }

    
    private static void EnsureOpenType(Type openType)
    {
      if (!openType.IsGenericTypeDefinition)
      {
        throw new ArgumentException($"{openType} is not open generic type");
      }
    }
    
    private static Type GetGenericArgumentTypeCore(Type type, Type openType)
    {
      if (type.IsGenericType && type.GetGenericTypeDefinition() == openType)
      {
        return type.GetGenericArguments()[0];
      }
      
      return null;
    }

  }
}
namespace CoreLib.UnityExt
{
  using System;
  using CoreLib.Core;
  using CoreLib.Extensions;
  using Unity.Builder;
  using Unity.Strategies;

  /// <summary>
  /// PropertyErrorsContainerStrategy
  /// </summary>
  public class PropertyErrorsContainerStrategy : BuilderStrategy
  {
    public override void PreBuildUp(ref BuilderContext context)
    {
      base.PreBuildUp(ref context);

      if (context.Type.IsSubclassOf<IPropertyErrorsContainer>())
      {
        ////Use the parent Type
        var sourceType = context.DeclaringType;
        if (context.Type != typeof(IPropertyErrorsContainer))
        {
          ////Use T of IPropertyErrorsContainer<T> if the context.Type implements it 
          sourceType = GetSourceType(context.Type);
        }

        if (sourceType != null)
        {
          var containerType = typeof(PropertyErrorsContainer<>).MakeGenericType(sourceType);
          context.Type = containerType;
        }
      }
    }
    
    private static Type GetSourceType(Type type)
    {
      var result = GetSourceTypeCore(type);
      if (result == null)
      {
        foreach (var intf in type.GetInterfaces())
        {
          result = GetSourceTypeCore(intf);
          if (result != null)
          {
            return result;
          }
        }
      }

      return result;
    }
    
    private static Type GetSourceTypeCore(Type type)
    {
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IPropertyErrorsContainer<>))
      {
        return type.GetGenericArguments()[0];
      }
      
      return null;
    }
  }
}
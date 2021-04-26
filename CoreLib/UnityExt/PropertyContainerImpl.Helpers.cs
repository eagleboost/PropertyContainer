namespace CoreLib.UnityExt
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Reflection;
  using System.Runtime.CompilerServices;
  using CoreLib.Core;
  using CoreLib.Extensions;
  using DependencyAttribute = Unity.DependencyAttribute;

  /// <summary>
  /// PropertyContainerImpl
  /// </summary>
  public static partial class PropertyContainerImpl
  {
    private static readonly MethodAttributes DefaultMethodAttributes =
      MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

    private static readonly MethodAttributes AddRemoveMethodAttributes =
      MethodAttributes.Public |
      MethodAttributes.SpecialName |
      MethodAttributes.NewSlot |
      MethodAttributes.HideBySig |
      MethodAttributes.Virtual |
      MethodAttributes.Final;
    
    private static readonly MethodImplAttributes EventMethodImplFlags = MethodImplAttributes.Synchronized;

    private static readonly Type EventHandlerType = typeof(PropertyChangedEventHandler);
    
    private static readonly MethodInfo AddPropertyChangedMethod = typeof(INotifyPropertyChanged).GetMethod("add_PropertyChanged");
    
    private static readonly MethodInfo RemovePropertyChangedMethod = typeof(INotifyPropertyChanged).GetMethod("remove_PropertyChanged");

    private static string GetImplTypeName(Type type)
    {
      return $"{type.Name}_<PropertyContainer>_Impl";
    }

    /// <summary>
    /// Get the properties need to be override with Property<> as backing field
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static IEnumerable<PropertyInfo> GetTargetProperties(Type type)
    {
      return type.GetProperties().Where(IsTargetProperty);
    }

    /// <summary>
    /// 1. Make sure it's virtual auto property, i.e. no method body for getter and setter
    /// 2. Make sure it does not have the [Dependency] attribute
    /// 3. Make sure it's not the PropertyStore property 
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    private static bool IsTargetProperty(PropertyInfo property)
    {
      var accessors = property.GetAccessors();
      if (accessors.Any(a => !a.IsVirtual || a.IsFinal))
      {
        return false;
      }
      
      if (!IsCompilerGenerated(property.GetGetMethod()))
      {
        return false;
      }

      if (IsDependency(property))
      {
        return false;
      }

      if (property.PropertyType.IsSubclassOf<IPropertyStore>())
      {
        return false;
      }
      
      var backingFieldName = GetAutoBackingFieldName(property.Name);
      var backingField = property.DeclaringType.GetField(backingFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
      return backingField != null && IsCompilerGenerated(backingField);
    }

    private static bool IsCompilerGenerated(MemberInfo memberInfo)
    {
      var result = memberInfo.IsDefined<CompilerGeneratedAttribute>();
      return result;
    }

    private static bool IsDependency(PropertyInfo property)
    {
      var result = property.IsDefined<DependencyAttribute>();
      return result;
    }

    private static string GetAutoBackingFieldName(string name)
    {
      return $"<{name}>k__BackingField";
    }
    
    /// <summary>
    /// PropertyName => _name;
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static string ToFieldName(string name)
    {
      var first = char.ToLower(name[0]);
      var result = $"_{first}{name.Substring(1, name.Length - 1)}";
      return result;
    }
  }
}
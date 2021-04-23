namespace CoreLib.UnityExt
{
  using System;
  using System.Diagnostics;
  using System.Reflection.Emit;

  public static partial class PropertyContainerImpl
  {
    [DebuggerDisplay("{PropertyName}")]
    private class PropertyFieldInfo
    {
      public PropertyFieldInfo(string propertyName, Type propertyType, FieldBuilder builder)
      {
        PropertyName = propertyName;
        PropertyType = propertyType;
        Builder = builder;
      }
      
      public readonly string PropertyName;

      /// <summary>
      /// string for instance
      /// </summary>
      public readonly Type PropertyType;

      /// <summary>
      /// Property<string> for instance
      /// </summary>
      public readonly FieldBuilder Builder;
    }
  }
}
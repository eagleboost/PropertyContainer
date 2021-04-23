namespace PropertyContainer.Bindings
{
  using System;
  using System.Linq;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Controls;

  public partial class ErrorInfoBehavior
  {
    public static class ValidationEx
    {
      public static readonly Action<ValidationError, DependencyObject, bool> AddValidationError;

      public static readonly Action<ValidationError, DependencyObject, bool> RemoveValidationError;

      static ValidationEx()
      {
        var methods = typeof(Validation).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);

        var addErrorMethod = methods.Single(i => i.Name == "AddValidationError" && i.GetParameters().Length == 3);
        AddValidationError = addErrorMethod.CreateDelegate<Action<ValidationError, DependencyObject, bool>>();

        var removeErrorMethod = methods.Single(i => i.Name == "RemoveValidationError" && i.GetParameters().Length == 3);
        RemoveValidationError = removeErrorMethod.CreateDelegate<Action<ValidationError, DependencyObject, bool>>();
      }
    }
  }
}
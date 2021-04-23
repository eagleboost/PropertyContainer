namespace PropertyContainer.ViewModels
{
  using System;
  using System.Linq.Expressions;
  using CoreLib.Core;
  using CoreLib.Extensions;

  public static class ViewModelWithErrorInfoValidations
  {
    public static IDisposable SetupNameValidation(this ViewModelWithErrorInfo vm)
    {
      return vm.SetupValidation<ViewModelWithErrorInfo, string>(vm.ErrorInfo, o => o.Name, ValidateName);
    }
    
    public static IDisposable SetupAgeValidation(this ViewModelWithErrorInfo vm)
    {
      return vm.SetupValidation<ViewModelWithErrorInfo, int>(vm.ErrorInfo, o => o.Age, ValidateAge);
    }
    
    public static void ClearValidations(this ViewModelWithErrorInfo vm)
    {
      vm.ErrorInfo.ClearValidations();
    }
    
    public static IDisposable SetupValidation<TProperty>(this ViewModelWithErrorInfo vm, Expression<Func<ViewModelWithErrorInfo, TProperty>> expr,
      Func<ViewModelWithErrorInfo, TProperty, string> validateFunc)
    {
      var name = expr.GetMember();
      var errorInfoType = typeof(IPropertyErrorsContainer<>).MakeGenericType(vm.GetType());
      var setupValidationMethod = errorInfoType.GetMethod("SetupValidation").MakeGenericMethod(typeof(TProperty));
      var result = (IDisposable) setupValidationMethod.Invoke(vm.ErrorInfo, new object[] {name, validateFunc});
      return result;
    }
    
    public static string ValidateName(ViewModelWithErrorInfo vm, string name)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        return "Name cannot be empty.";
      }

      if (name?.Length <= 5)
      {
        return "Name must be at least 6 characters long.";
      }

      return null;
    }

    public static string ValidateAge(ViewModelWithErrorInfo vm, int age)
    {
      if (age <= 0)
      {
        return "Age must be positive";
      }

      return null;
    }
  }
}
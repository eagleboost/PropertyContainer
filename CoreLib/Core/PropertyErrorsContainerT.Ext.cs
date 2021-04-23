namespace CoreLib.Core
{
  using System;
  using System.Linq.Expressions;
  using CoreLib.Extensions;

  /// <summary>
  /// PropertyErrors
  /// </summary>
  public static class PropertyErrorsContainerExt
  {
    public static IDisposable SetupValidation<T, TProperty>(this IPropertyErrorsContainer<T> vm, Expression<Func<T, TProperty>> expr, Func<T, TProperty, string> validateFunc)
    {
      var name = expr.GetMember();
      return vm.SetupValidation(name, validateFunc);
    }
    
    public static IDisposable SetupValidation<T, TProperty>(this object viewModel, IPropertyErrorsContainer errorsContainer, Expression<Func<T, TProperty>> expr, Func<T, TProperty, string> validateFunc)
    {
      var vm = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
      var name = expr.GetMember();
      var type = typeof(IPropertyErrorsContainer<>).MakeGenericType(vm.GetType());
      // ReSharper disable once PossibleNullReferenceException
      var setupValidationMethod = type.GetMethod("SetupValidation").MakeGenericMethod(typeof(TProperty));
      var result = (IDisposable) setupValidationMethod.Invoke(errorsContainer, new object[] {name, validateFunc});
      return result;
    }
  }
}
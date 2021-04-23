using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnitTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace CoreLib.Core
{
  using System;
  using System.Diagnostics;

  /// <summary>
  /// PropertyErrorsContainer
  /// </summary>
  [DebuggerDisplay("{Source}")]
  public sealed partial class PropertyErrorsContainer<T> : PropertyErrorsContainer, IPropertyErrorsContainer<T>, IPropertyErrorsContainerInternal<T>, ISubComponent
  {
    #region Declarations
    private IPropertyContainer _container;
    #endregion Declarations

    #region ISubComponent
    public void SetParent(object parent)
    {
      Source = (T)parent;
      _container = (IPropertyContainer) parent;
    }
    #endregion ISubComponent

    #region IPropertyErrorsContainer
    public IDisposable SetupValidation<TProperty>(string name, Func<T, TProperty, string> validateFunc)
    {
      var property = (IProperty<TProperty>)_container.PropertyStore.GetProperty(name);
      var validator = new PropertyValidator<TProperty>(this, property, validateFunc);
      validator.Validate();

      return validator;
    }
    #endregion IPropertyErrorsContainer
    
    #region IPropertyErrorsContainerInternal
    public T Source { get; private set; }

    void IPropertyErrorsContainerInternal<T>.RemoveValidator(IPropertyValidator validator)
    {
      RemoveValidator(validator);
    }

    void IPropertyErrorsContainerInternal<T>.AddValidator(IPropertyValidator validator)
    {
      AddValidator(validator);
    }
    #endregion IPropertyErrorsContainerInternal
  }
}
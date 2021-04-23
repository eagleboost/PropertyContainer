namespace CoreLib.Core
{
  using System;
  using System.ComponentModel;

  /// <summary>
  /// IPropertyErrorsContainerInternal
  /// </summary>
  /// <typeparam name="T"></typeparam>
  internal interface IPropertyErrorsContainerInternal<out T> : IPropertyErrorsContainer
  {
    T Source { get; }
    
    void AddValidator(PropertyErrorsContainer.IPropertyValidator validator);
    
    void RemoveValidator(PropertyErrorsContainer.IPropertyValidator validator);

    void RaiseErrorsChanged(PropertyChangedEventArgs args);
  }

  /// <summary>
  /// IPropertyErrorsContainer
  /// </summary>
  public interface IPropertyErrorsContainer<out T> : IPropertyErrorsContainer
  {
    IDisposable SetupValidation<TProperty>(string name, Func<T, TProperty, string> validateFunc);
  }
}
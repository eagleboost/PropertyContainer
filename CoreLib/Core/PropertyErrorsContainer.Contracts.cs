namespace CoreLib.Core
{
  using System;
  using System.Collections;
  using System.Collections.Generic;

  /// <summary>
  /// IPropertyErrorsContainer
  /// </summary>
  public interface IPropertyErrorsContainer
  {
    #region Properties
    IEnumerable Errors { get; }
    #endregion Properties

    #region Methods
    IPropertyErrors GetPropertyErrors(string name);

    void ClearValidations();
    #endregion Methods

    #region Events
    event EventHandler<PropertyErrorsChangedEventArgs> PropertyErrorsChanged;
    #endregion Events
  }
  
  /// <summary>
  /// IPropertyErrors
  /// </summary>
  public interface IPropertyErrors
  {
    string Name { get; }
    
    IReadOnlyCollection<string> Errors { get; }

    event EventHandler ErrorsChanged;
  }

  public abstract partial class PropertyErrorsContainer
  {
    /// <summary>
    /// IPropertyValidator
    /// </summary>
    internal interface IPropertyValidator : IDisposable
    {
      PropertyErrors PropertyErrors { get; }
    }
  }
}
namespace CoreLib.Core
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Linq;

  /// <summary>
  /// PropertyErrorsContainer
  /// </summary>
  public abstract partial class PropertyErrorsContainer : IPropertyErrorsContainer, INotifyPropertyChanged, INotifyDataErrorInfo
  {
    #region Statics
    public static readonly string[] EmptyErrors = new string[0];
    #endregion Statics
    
    #region Declarations
    private readonly Dictionary<string, IPropertyValidator> _propertyValidator = new Dictionary<string, IPropertyValidator>();
    private readonly Dictionary<string, IPropertyErrors> _propertyErrors = new Dictionary<string, IPropertyErrors>();
    private ObservableCollection<string> _errors;
    #endregion Declarations

    #region INotifyDataErrorInfo
    public IEnumerable GetErrors(string propertyName)
    {
      return _propertyErrors.TryGetValue(propertyName, out var propertyErrors) ? propertyErrors.Errors : EmptyErrors;
    }

    public bool HasErrors
    {
      get
      {
        if (_propertyErrors.Count == 0)
        {
          return false;
        }

        foreach (var errors in _propertyErrors.Values)
        {
          if (errors.Errors.Count > 0)
          {
            return true;
          }
        }

        return false;
      }
    }
    
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public void RaiseErrorsChanged(PropertyChangedEventArgs args)
    {
      ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(args.PropertyName));
      NotifyErrorsChanged();
    }
    #endregion INotifyDataErrorInfo

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyErrorsChanged()
    {
      _errors = null;
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Errors)));
    }
    #endregion INotifyPropertyChanged
    
    #region IPropertyErrorsContainer
    public IEnumerable Errors => _errors ??= CreateErrors();

    public IPropertyErrors GetPropertyErrors(string name)
    {
      return _propertyErrors.GetValueOrDefault(name);
    }

    public void ClearValidations()
    {
      foreach (var validator in _propertyValidator.Values.ToArray())
      {
        validator.Dispose();
      }
    }

    public event EventHandler<PropertyErrorsChangedEventArgs> PropertyErrorsChanged;

    private void RaisePropertyErrorsChanged(IPropertyErrors propertyErrors, PropertyErrorsChangeType type)
    {
      PropertyErrorsChanged?.Invoke(this, new PropertyErrorsChangedEventArgs(propertyErrors, type));
      NotifyErrorsChanged();
    }
    #endregion IPropertyErrorsContainer

    #region Protected Methods
    internal void RemoveValidator(IPropertyValidator validator)
    {
      _propertyValidator.Remove(validator.PropertyErrors.Name);
      RemoveError(validator.PropertyErrors);
    }

    internal void AddValidator(IPropertyValidator validator)
    {
      AddError(validator.PropertyErrors);
      _propertyValidator.Add(validator.PropertyErrors.Name, validator);
    }

    #endregion Protected Methods
    
    #region Private Methods
    private void RemoveError(IPropertyErrors propertyErrors)
    {
      if (_propertyErrors.Remove(propertyErrors.Name))
      {
        RaisePropertyErrorsChanged(propertyErrors, PropertyErrorsChangeType.Remove);
      }
    }

    private void AddError(IPropertyErrors propertyErrors)
    {
      _propertyErrors.Add(propertyErrors.Name, propertyErrors);
      RaisePropertyErrorsChanged(propertyErrors, PropertyErrorsChangeType.Add);
    }

    private ObservableCollection<string> CreateErrors()
    {
      var result = new ObservableCollection<string>();
      foreach (var p in _propertyErrors.Values)
      {
        foreach (var error in p.Errors)
        {
          result.Add(error);
        }
      }

      return result;
    }
    #endregion Private Methods
  }
}
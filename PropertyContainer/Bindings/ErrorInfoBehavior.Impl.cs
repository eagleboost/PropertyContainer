namespace PropertyContainer.Bindings
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using CoreLib.Core;
  using static System.Windows.WeakEventManager<CoreLib.Core.IPropertyErrors, System.EventArgs>;
  using static ErrorInfoBehavior.ValidationEx;

  /// <summary>
  /// ErrorInfoBehavior
  /// </summary>
  public partial class ErrorInfoBehavior
  {
    private class ErrorInfoHandler
    {
      private readonly FrameworkElement _element;
      private readonly BindingExpression _bindingExpr;
      private readonly string _propertyName;
      private readonly INotifyDataErrorInfo _errorInfo;
      private readonly IPropertyErrorsContainer _errorsContainer;
      private ValidationError _validationError;

      public ErrorInfoHandler(FrameworkElement element, INotifyDataErrorInfo errorInfo, DependencyProperty dp)
      {
        _element = element;
        _bindingExpr = element.GetBindingExpression(dp);
        // ReSharper disable once PossibleNullReferenceException
        _propertyName = _bindingExpr.ResolvedSourcePropertyName;
        _errorInfo = errorInfo;
        _errorsContainer = errorInfo as IPropertyErrorsContainer;
      }

      public void Start()
      {
        if (_errorsContainer != null)
        {
          var propertyErrors = _errorsContainer.GetPropertyErrors(_propertyName);
          if (propertyErrors != null)
          {
            HookErrorsChanged(propertyErrors);
          }
          else
          {
            _errorsContainer.PropertyErrorsChanged += HandlePropertyErrorsChanged;
          }
        }
        else
        {
          _errorInfo.ErrorsChanged += HandlerAllErrorsChanged;
        }  
      }

      private void HandlePropertyErrorsChanged(object sender, PropertyErrorsChangedEventArgs e)
      {
        if (e.Type == PropertyErrorsChangeType.Add)
        {
          HookErrorsChanged(e.PropertyErrors);
        }
        else if (e.Type == PropertyErrorsChangeType.Remove)
        {
          UnHookErrorsChanged(e.PropertyErrors);
        }
      }
      
      private void HookErrorsChanged(IPropertyErrors propertyErrors)
      {
        if (propertyErrors.Name == _propertyName)
        {
          AddHandler(propertyErrors, nameof(IPropertyErrors.ErrorsChanged), HandlePropertyErrorsChanged);
        }
      }
      
      private void UnHookErrorsChanged(IPropertyErrors propertyErrors)
      {
        if (propertyErrors.Name == _propertyName)
        {
          RemoveHandler(propertyErrors, nameof(IPropertyErrors.ErrorsChanged), HandlePropertyErrorsChanged);
          ClearValidationError();
        }
      }
      
      private void HandlePropertyErrorsChanged(object sender, EventArgs e)
      {
        var propertyErrors = (IPropertyErrors) sender;
        UpdateValidationError(propertyErrors.Errors);
      }
      
      private void HandlerAllErrorsChanged(object sender, DataErrorsChangedEventArgs e)
      {
        var name = e.PropertyName;
        if (name == _propertyName)
        {
          UpdateValidationError(_errorInfo.GetErrors(name).Cast<string>());
        }
      }

      private void ClearValidationError()
      {
        if (_validationError != null)
        {
          RemoveValidationError(_validationError, _element, false);
        }
      }
      
      private void UpdateValidationError(IEnumerable<string> errors)
      {
        ClearValidationError();
        
        if (errors.Any())
        {
          _validationError = new ValidationError(new NotifyDataErrorValidationRule(), _bindingExpr, errors, null);
          AddValidationError(_validationError, _element, false);
        }
      }
    }
  }
}
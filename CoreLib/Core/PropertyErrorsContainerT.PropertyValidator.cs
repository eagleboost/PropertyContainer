namespace CoreLib.Core
{
  using System;
  using System.Diagnostics;
  using System.Threading;
  using CoreLib.Extensions;

  /// <summary>
  /// PropertyValidator
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public sealed partial class PropertyErrorsContainer<T>
  {
    [DebuggerDisplay("{PropertyErrors}")]
    internal class PropertyValidator<TProperty> : IPropertyValidator
    {
      #region Declarations
      private readonly Func<T, TProperty, string> _validateFunc;
      private IDisposable _cleanup;
      #endregion Declarations

      public PropertyValidator(IPropertyErrorsContainerInternal<T> container, IProperty<TProperty> property, Func<T, TProperty, string> validateFunc)
      {
        Container = container;
        _validateFunc = validateFunc;
        Property = property;
        property.ValueChanged += HandlePropertyValueChanged;

        PropertyErrors = new PropertyErrors(property.NameArgs.PropertyName);
        Container.AddValidator(this);

        _cleanup = new ActionDisposable(() =>
        {
          Container.RemoveValidator(this);
          property.ValueChanged -= HandlePropertyValueChanged;
        });
      }

      public readonly IPropertyErrorsContainerInternal<T> Container;

      public readonly IProperty<TProperty> Property;
      
      public PropertyErrors PropertyErrors { get; }
      
      public void Dispose()
      {
        var cleanup = Interlocked.Exchange(ref _cleanup, null);
        cleanup?.Dispose();
      }

      public void Validate()
      {
        ValidateCore(Property.Value);
      }
      
      private void HandlePropertyValueChanged(object sender, ValueChangedArgs<TProperty> e)
      {
        ValidateCore(e.NewValue);
      }

      private void ValidateCore(TProperty value)
      {
        var hasChange = false;

        var error = _validateFunc(Container.Source, value);
        if (error.HasValue())
        {
          if (!PropertyErrors.Contains(error))
          {
            PropertyErrors.Clear();
            PropertyErrors.Set(error);
            hasChange = true;
          }
        }
        else
        {
          hasChange = PropertyErrors.Clear();
        }

        if (hasChange)
        {
          PropertyErrors.NotifyErrorsChanged();
          Container.RaiseErrorsChanged(Property.NameArgs);
        }
      }
    }
  }
}
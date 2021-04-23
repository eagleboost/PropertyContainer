namespace CoreLib.Core
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using CoreLib.Extensions;

  /// <summary>
  /// PropertyErrors
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public abstract partial class PropertyErrorsContainer
  {
    [DebuggerDisplay("{Name}")]
    internal class PropertyErrors : IPropertyErrors
    {
      private readonly List<string> _errors = new List<string>();

      public PropertyErrors(string name)
      {
        Name = name;
      }

      #region IPropertyErrors
      public string Name { get; }
      
      public IReadOnlyCollection<string> Errors => _errors;
      
      public event EventHandler ErrorsChanged;
      
      public void NotifyErrorsChanged()
      {
        ErrorsChanged?.Invoke(this, EventArgs.Empty);
      }
      #endregion IPropertyErrors
      
      #region Public Methods
      public bool Contains(string error)
      {
        return _errors.Contains(error);
      }
      
      public void Set(string error)
      {
        if (error.HasValue() && !_errors.Contains(error))
        {
          _errors.Add(error);
        }
      }

      public bool Clear()
      {
        if (_errors.Any())
        {
          _errors.Clear();
          return true;
        }

        return false;
      }
    }
    #endregion Public Methods
  }
}
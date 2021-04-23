namespace CoreLib.Core
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;

  /// <summary>
  /// PropertyErrors
  /// </summary>
  [DebuggerDisplay("{PropertyErrors}")]
  public sealed class PropertyErrorsChangedEventArgs : EventArgs
  {
    public PropertyErrorsChangedEventArgs(IPropertyErrors propertyErrors, PropertyErrorsChangeType type)
    {
      PropertyErrors = propertyErrors;
      Type = type;
    }

    public readonly IPropertyErrors PropertyErrors;

    public readonly PropertyErrorsChangeType Type;
  }


  /// <summary>
  /// PropertyErrorsChangeType
  /// </summary>
  public enum PropertyErrorsChangeType
  {
    Add,
    Remove
  }
}
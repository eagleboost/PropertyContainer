namespace CoreLib.Core
{
  using System.Collections.Generic;

  /// <summary>
  /// IPropertyStore
  /// </summary>
  public interface IPropertyStore
  {
    IReadOnlyCollection<IProperty> Properties { get; }

    IProperty GetProperty(string name);
  }
}
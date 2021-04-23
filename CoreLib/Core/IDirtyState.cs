namespace CoreLib.Core
{
  using System.Collections;

  /// <summary>
  /// IDirtyState
  /// </summary>
  public interface IDirtyState
  {
    bool IsDirty { get; }
  }
  
  /// <summary>
  /// IDirtyItems
  /// </summary>
  public interface IDirtyItems : IDirtyState
  {
    IEnumerable DirtyItems { get; }
  }
}
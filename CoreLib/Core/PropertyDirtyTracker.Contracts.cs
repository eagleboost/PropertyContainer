namespace CoreLib.Core
{
  /// <summary>
  /// IPropertyDirtyTracker
  /// </summary>
  public interface IPropertyDirtyTracker : IDirtyItems
  {
    void MarkInitialStates();
  }
}
namespace CoreLib.Core
{
  using System.Collections;
  using static PropertyArgsStore<IDirtyItems>;
  
  /// <summary>
  /// DirtyStateArgs
  /// </summary>
  public static class DirtyStateArgs
  {
    public static readonly PropertyChangedEventArgs<bool> IsDirty = GetArgs(o => o.IsDirty);
    public static readonly PropertyChangedEventArgs<IEnumerable> DirtyItems = GetArgs(o => o.DirtyItems);
  }
}
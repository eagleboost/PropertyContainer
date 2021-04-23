namespace UnitTests
{
  using CoreLib.Core;
  using Unity;

  public class ViewModelWithDirtyTracker
  {
    [Dependency]
    public PropertyDirtyTracker DirtyTracker { get; set; }
  }
}
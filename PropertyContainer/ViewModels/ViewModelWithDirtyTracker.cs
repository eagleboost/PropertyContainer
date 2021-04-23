namespace PropertyContainer.ViewModels
{
  using CoreLib.Core;
  using Unity;

  public class ViewModelWithDirtyTracker
  {
    [Dependency]
    public IPropertyDirtyTracker DirtyTracker { get; set; }

    public virtual string Name { get; set; }
    
    public virtual int Age { get; set; }
    
    
    public override string ToString()
    {
      return "ViewModelWithErrorInfo";
    }
  }
}
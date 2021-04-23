namespace PropertyContainer.ViewModels
{
  using System.ComponentModel;
  using System.Diagnostics;
  using CoreLib.Core;
  using Unity;
  
  /// <summary>
  /// ViewModel
  /// </summary>
  public class ViewModel
  {
    public virtual string Name { get; set; }
    
    public virtual int Age { get; set; }
    
    [InjectionMethod]
    public void Init()
    {
      this.HookChange(o => o.Name, HandleNameChanged);
      this.HookChange(HandlePropertyChanged);
    }

    private void HandleNameChanged(object sender, ValueChangedArgs<string> e)
    {
      Trace.WriteLine($"[Name Changed] {e}");
    }
    
    private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      Trace.WriteLine($"[Property Changed] {e.PropertyName}");
    }

    public override string ToString()
    {
      return "ViewModel";
    }
  }
}
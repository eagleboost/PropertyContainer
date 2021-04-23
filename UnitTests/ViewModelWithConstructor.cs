namespace UnitTests
{
  using Unity;

  public class ViewModelWithConstructor
  {
    [InjectionConstructor]
    public ViewModelWithConstructor()
    {
    }
      
    public ViewModelWithConstructor(string name)
    {
    }
      
    public virtual string Name { get; set; }
  }
}
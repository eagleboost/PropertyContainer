namespace UnitTests
{
  using Unity;

  public class ViewModelWithDependencies
  {
    [Dependency]
    public virtual ViewModelWithConstructor SubViewModel { get; set; }
  }
}
namespace UnitTests
{
  public class ViewModel
  {
    public virtual string Name { get; set; }
      
    public string NonVirtualName { get; set; }
    
    public virtual int Age { get; set; }
  }
}
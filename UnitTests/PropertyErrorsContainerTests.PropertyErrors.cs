namespace UnitTests
{
  using System.Linq;
  using NUnit.Framework;

  public partial class PropertyErrorsContainerTests
  {
    public class PropertyErrorsTests
    {
      [Test]
      public void Task_01_Creation()
      {
        var propertyErrors = CreatePropertyErrors("ABC");
        Assert.That(propertyErrors.Name == "ABC");
        Assert.That(!propertyErrors.Errors.Any());
      }
      
      [Test]
      public void Task_02_Add()
      {
        var propertyErrors = CreatePropertyErrors("ABC");
        propertyErrors.Set("Error");
        Assert.That(propertyErrors.Contains("Error"));
        Assert.That(propertyErrors.Errors.Count()==1);
        Assert.That(propertyErrors.Errors.First() == "Error");
      }
      
      [Test]
      public void Task_03_Clear()
      {
        var propertyErrors = CreatePropertyErrors("ABC");
        propertyErrors.Clear();
        Assert.That(!propertyErrors.Errors.Any());
        
        propertyErrors.Set("Error");
        propertyErrors.Clear();
        Assert.That(!propertyErrors.Errors.Any());
      }
    }
  }
}